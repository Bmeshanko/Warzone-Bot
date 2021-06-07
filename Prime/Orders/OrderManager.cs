using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    class OrderManager
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
            this.BonusesCompleted = EvaluateBonusesCompleted();
        }

        public List<BonusIDType> EvaluateBonusesCompleted()
        {
            return Bot.Map.Bonuses.Keys.Where(o => Bot.leftToComplete(o) == 0).ToList();
        }

        public void Go()
        {
            if (BonusesCompleted.Count < 3)
            {
                EarlyGameExpansion ege = new EarlyGameExpansion(Bot);
                ege.Go();
                Deploys = ege.Deploys;
                Moves = ege.Moves;
            }
        }

    }
}
