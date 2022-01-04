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
        public void Go()
        {
            DeployOrders d = new DeployOrders(Bot);
            var terr = d.evaluateDeploys().First();
            var deploy = GameOrderDeploy.Create(Bot.PlayerID, Bot.BaseIncome.FreeArmies, terr, true);
            Deploys.Add(deploy);


        }
    }
}
