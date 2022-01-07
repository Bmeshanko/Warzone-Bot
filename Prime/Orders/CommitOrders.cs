using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class CommitOrders
    {
        public Main.PrimeBot Bot;

        public CommitOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
        }

        public List<GameOrder> Commit()
        {
            OrderManager manager = new OrderManager(Bot);
            manager.Go();

            List<GameOrder> orders = manager.Deploys;
            foreach(var move in manager.Moves)
            {
                orders.Add(move);
            }
            Bot.NumberOfTurns++;
            return orders;
        }

    }
}
