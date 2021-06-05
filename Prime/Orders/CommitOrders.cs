using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    class CommitOrders
    {

        public Main.PrimeBot Bot;

        public CommitOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
        }

        public List<GameOrder> Commit()
        {
            MakeMoves makeMoves = new MakeMoves(Bot);
            makeMoves.TakeFirstBonuses();

            List<GameOrder> orders = makeMoves.Deploys;
            foreach(var move in makeMoves.Moves)
            {
                orders.Add(move);
            }
            return orders;
        }

    }
}
