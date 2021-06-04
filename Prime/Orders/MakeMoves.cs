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
            List<TerritoryIDType> deployedTo = new List<TerritoryIDType>();

            foreach (var terr in Bot.Standing.Territories.Values)
            {
                var terrDetails = Bot.Map.Territories[terr.ID];
                if (terr.IsNeutral && borders.Contains(terr.ID) && bonuses.Contains(terrDetails.PartOfBonuses.First()))
                {
                    // We know it's neutral, in our bonus, and we border it. Now we need to deploy and attack.
                    var ourTerr = terrDetails.ConnectedTo.Keys.Where(o => Bot.Standing.Territories[o].OwnerPlayerID == Bot.PlayerID).First();
                    if (armiesLeft.NumArmies >= 3 && !deployedTo.Contains(ourTerr))
                    {
                        Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, 3, ourTerr, false));
                        armiesLeft = new Armies(armiesLeft.NumArmies - 3);
                        deployedTo.Add(ourTerr);
                    } 
                    else if (armiesLeft.NumArmies > 0 && !deployedTo.Contains(ourTerr))
                    {
                        Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, armiesLeft.NumArmies, ourTerr, false));
                        armiesLeft = new Armies(0);
                        deployedTo.Add(ourTerr);
                    }
                    
                    Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, ourTerr, terr.ID, 
                        AttackTransferEnum.AttackTransfer, false, Bot.Standing.Territories[ourTerr].NumArmies, false));
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
