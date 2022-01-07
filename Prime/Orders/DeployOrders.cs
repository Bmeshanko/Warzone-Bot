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
            int hardest = Bot.leftToComplete(Bot.whatBonus(Bot.OurTerritories().First()));
            var hardestTerr = Bot.OurTerritories().First();
            foreach(var terr in territories)
            {
                int armiesNeeded = Bot.leftToComplete(Bot.whatBonus(terr));
                if (armiesNeeded > hardest)
                {
                    hardest = armiesNeeded;
                    hardestTerr = terr;
                }
            }


            return territories;
        }

        public List<TerritoryIDType> turnOneDeploys()
        {
            var territories = Bot.OurTerritories();
            int hardest = Bot.leftToComplete(Bot.whatBonus(Bot.OurTerritories().First()));
            var hardestTerr = Bot.OurTerritories().First();
            var secondHardestTerr = Bot.OurTerritories().ExceptOne(hardestTerr).First(); // Starts as Second
            foreach (var terr in territories)
            {
                int armiesNeeded = Bot.leftToComplete(Bot.whatBonus(terr));
                if (armiesNeeded > hardest)
                {
                    hardest = armiesNeeded;
                    secondHardestTerr = hardestTerr;
                    hardestTerr = terr;
                }
            }
            List<TerritoryIDType> deploys = new List<TerritoryIDType>();
            deploys.Add(hardestTerr);
            deploys.Add(secondHardestTerr);
            return deploys;
        }
    }
}
