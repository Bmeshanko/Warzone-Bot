using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class DeployOrders
    {
        public Main.PrimeBot Bot;

        public DeployOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
        }
        
        public List<TerritoryIDType> evaluateDeploys()
        {
            var territories = Bot.OurTerritories();
            int easiest = 100; // Arbitrarily large number
            foreach(var terr in territories)
            {
                int armiesNeeded = Bot.leftToComplete(Bot.whatBonus(terr));
                if (armiesNeeded < easiest)
                {
                    easiest = armiesNeeded;
                }
            }
            territories.RandomizeOrder();
            return territories;
        }
    }
}
