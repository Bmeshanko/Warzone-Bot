using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Picks
{
    class EfficiencyCalculator
    {

        public int TurnsForBonus(Main.PrimeBot bot, TerritoryIDType starting, BonusIDType bonusID)
        {
            GameStanding distribution = bot.DistributionStanding;

            BonusDetails bonus = bot.Map.Bonuses[bonusID];
            var territoriesInBonus = bonus.Territories.ToHashSet(true); // Territories in the Bonus.
            var territoriesOwned = new HashSet<TerritoryIDType>();
            territoriesOwned.Add(starting);

            bool flag = true;
            int numTurns = 0;

            while (flag)
            {
                numTurns++;
                flag = false;
                territoriesOwned = ExpandInBonus(bot, territoriesOwned, bonusID);

                foreach(var territory in territoriesInBonus)
                {
                    if (!territoriesOwned.Contains(territory))
                    {
                        flag = true;
                    }
                }
            }

            return numTurns;
        }

        /*
         * Add All Territories to the HashSet that:
         * 1. Are inside the bonus.
         * 2. Border a territory in the HashSet.
         */
        public HashSet<TerritoryIDType> ExpandInBonus(Main.PrimeBot bot, HashSet<TerritoryIDType> territoriesOwned, BonusIDType bonusID)
        {
            var newTerritoriesOwned = territoriesOwned;

            BonusDetails bonus = bot.Map.Bonuses[bonusID];
            var territoriesInBonus = bonus.Territories.ToHashSet(true); // Territories in the Bonus.

            var connections = territoriesInBonus.Where(o => bot.Map.Territories[o].ConnectedTo.Keys.Any(z => territoriesOwned.Contains(z))).ToHashSet(true);
            foreach(var territory in connections)
            {
                newTerritoriesOwned.Add(territory);
            }
            return newTerritoriesOwned;
        }


    }
}
