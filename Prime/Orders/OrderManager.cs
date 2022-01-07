using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class OrderManager
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;
        public List<GameOrder> Moves;
        public List<BonusIDType> BonusesCompleted;

        public OrderManager(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = new List<GameOrder>();
        }

        public List<BonusIDType> sortedBonuses()
        {
            Dictionary<BonusIDType, int> bonuses = new Dictionary<BonusIDType, int>();

            foreach (var bonus in Bot.BonusesWeHave())
            {
                Main.BonusPath bp = new Main.BonusPath(bonus, Bot);
                bp.shortestPath(Bot.OurTerritoriesInBonus(bonus));
                AILog.Log("BonusPath", bp.armiesNeeded + " armies needed to complete " + Bot.Map.Bonuses[bonus].Name);
                bonuses.Add(bonus, bp.armiesNeeded);
            }
            bonuses.OrderBy(o => o.Value);
            return bonuses.Keys.ToList();
        }
        public void Go()
        {
            EvaluateOrders e = new EvaluateOrders(Bot);
            List<BonusIDType> bestBonuses = sortedBonuses();
            Deploys = e.expansion(bestBonuses);
        }
    }
}
