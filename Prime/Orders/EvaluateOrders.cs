using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class EvaluateOrders
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker Income;

        public EvaluateOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.Income = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
        }

        public List<GameOrder> expansion(List<BonusIDType> sortedBonuses)
        {
            List<GameOrder> deploys = new List<GameOrder>();

            int income = Income.FreeArmiesUndeployed;
            int armies = income;
            foreach (var bonus in sortedBonuses)
            {
                Main.BonusPath bp = new Main.BonusPath(bonus, Bot);
                bp.shortestPath(Bot.OurTerritoriesInBonus(bonus));
                List<TerritoryIDType> where = bp.deployWhereThisTurn;
                List<int> howMuch = bp.deployThisTurn;
                if (where != null) {
                    for (int i = 0; i < where.Count; i++)
                    {
                        int toDeploy = howMuch.ElementAt(i);
                        if (toDeploy == 0 && armies > 0)
                        {
                            deploys.Add(Deploy.Go(where.ElementAt(i), armies, Bot));
                            armies = 0;
                        }
                        if (armies > toDeploy)
                        {
                            deploys.Add(Deploy.Go(where.ElementAt(i), toDeploy, Bot));
                            armies -= howMuch.ElementAt(i);
                        }
                    }
                }
            }
            return deploys;
        }
    }
}
