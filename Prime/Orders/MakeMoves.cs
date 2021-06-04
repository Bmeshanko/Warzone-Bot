using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class MakeMoves
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;
        public List<GameOrder> Moves;
        public List<BonusIDType> Bonuses;

        public MakeMoves(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = new List<GameOrder>();
        }

        public void Go()
        {
            List<BonusIDType> bonuses = new List<BonusIDType>();
            Dictionary<TerritoryIDType, TerritoryIDType> fromTo = new Dictionary<TerritoryIDType, TerritoryIDType>(); // Bordered, unowned, and in bonuses.
            List<TerritoryIDType> territoriesOwned = new List<TerritoryIDType>(); // Owned by us.
            foreach (var bonus in Bot.Map.Bonuses.Values)
            {
                foreach (var terr in bonus.Territories)
                {
                    if (Bot.Standing.Territories[terr].OwnerPlayerID == Bot.PlayerID)
                    {
                        bonuses.Add(bonus.ID);
                        territoriesOwned.Add(terr);
                    }
                }
            }

            var borders = Bot.Map.Territories.Keys.Where(o => Bot.Map.Territories[o].ConnectedTo.Keys.Any(z => territoriesOwned.Contains(z))).ToHashSet(true);

            Armies armiesLeft = new Armies(IncomeTracker.FreeArmiesUndeployed);

            List<TerritoryDetails> terrDetails = Bot.Map.Territories.Values.ToList();
            var ourTerr = terrDetails.Where(o => Bot.Standing.Territories[o.ID].OwnerPlayerID == Bot.PlayerID).ToList();
            Dictionary<TerritoryIDType, int> deployedTo = new Dictionary<TerritoryIDType, int>();

            foreach (var terr in ourTerr)
            {
                var targets = Bot.ConnectedToInBonus(terr.ID).Where(o => Bot.Standing.Territories[o].IsNeutral).ToList();
                // We know it's neutral, in our bonus, and we border it. Now we need to deploy and attack.
                
                int currentArmies = Bot.Standing.Territories[terr.ID].NumArmies.NumArmies;

                if (armiesLeft.NumArmies >= 3 && !deployedTo.Keys.Contains(terr.ID))
                {
                    currentArmies += 3;
                    Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, 3, terr.ID, false));
                    deployedTo.Add(terr.ID, currentArmies);
                    armiesLeft = new Armies(armiesLeft.NumArmies - 3);
                } 
                else if (armiesLeft.NumArmies > 0 && !deployedTo.Keys.Contains(terr.ID) && targets.Count > 0)
                {
                    currentArmies += armiesLeft.NumArmies;
                    Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, armiesLeft.NumArmies, terr.ID, false));
                    deployedTo.Add(terr.ID, armiesLeft.NumArmies);
                    armiesLeft = new Armies(0);
                    
                }

                foreach(var opponent in Bot.Map.Territories.Values)
                {
                    if (opponent.ConnectedTo.ContainsKey(terr.ID) && Bot.Standing.Territories[opponent.ID].OwnerPlayerID != Bot.PlayerID) {
                        //targets.Add(opponent.ID);
                    }
                }

                while (targets.Count > 0 && currentArmies > 3)
                {
                    int numAttack = 3;
                    Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, terr.ID, targets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(numAttack), false));
                    targets.Remove(targets.First());
                    currentArmies -= numAttack;
                }
            }
        }

        public TerritoryIDType RandomNeutralInBonus(Main.PrimeBot bot, TerritoryIDType from)
        {
            var bonus = bot.Map.Territories[from].PartOfBonuses.First(); // This will not work with SuperBonuses.
            var bonusDetails = bot.Map.Bonuses[bonus];
            var territoriesInBonus = bonusDetails.Territories.ToHashSet(true); // Territories in the Bonus.

            var connections = territoriesInBonus.Where(o => bot.Map.Territories[o].ConnectedTo.Keys.Any(z => z.Equals(from))).ToHashSet(true);
            var neutralConnects = connections.Where(o => bot.Standing.Territories[o].IsNeutral);

            return neutralConnects.Random();
        }


    }
}
