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
        public List<GameOrder> Moves;
        public List<GameOrder> Deploys;

        public MakeMoves(Main.PrimeBot bot, List<GameOrder> deploys)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = deploys;
        }

        public void Go()
        {
            foreach (GameOrderDeploy order in Deploys)
            {
                var from = order.DeployOn;
                var to = RandomNeutralInBonus(Bot, from);

                AILog.Log("MakeMoves", "Attack from " + Bot.Map.Territories[from].Name + " to " + Bot.Map.Territories[to].Name);
                Armies armies = Bot.Standing.Territories[from].NumArmies;
                Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, from, to, AttackTransferEnum.Attack, false, armies, true));
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
