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
        public MakeDeploys Deploys;
        public MakeMoves Moves;

        public CommitOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
        }

        public List<GameOrder> Commit()
        {
            MakeDeploys makeDeploys = new MakeDeploys(Bot);
            makeDeploys.Go();
            List<GameOrder> orders = new List<GameOrder>();

            foreach (var deploy in makeDeploys.Deploys.ToList())
            {
                orders.Add(deploy);
            }

            MakeMoves makeMoves = new MakeMoves(Bot, orders);

            foreach (var move in makeMoves.Moves.ToList())
            {
                orders.Add(move);
            }

            return orders;
        }
    }
}
