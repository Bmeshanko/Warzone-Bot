using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class Deploy
    {
        public static GameOrder Go(TerritoryIDType terr, int amount, Main.PrimeBot bot)
        {
            AILog.Log("Deploy", "Deploying " + amount + " on " + bot.Map.Territories[terr].Name);
            return GameOrderDeploy.Create(bot.PlayerID, amount, terr, true);
        }
    }
}

