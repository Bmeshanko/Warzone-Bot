using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class MakeDeploys
    {

        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;

        public MakeDeploys(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Deploys = new List<GameOrder>();
        }

        public void Go()
        {
            var count = IncomeTracker.FreeArmiesUndeployed;
            GameStanding standing = Bot.Standing;
            var territories = standing.Territories.ToList();

            PlayerIDType playerID = Bot.PlayerID;

            HashSet<TerritoryIDType> ourTerritories = new HashSet<TerritoryIDType>();
            foreach(var terr in territories)
            {
                if (terr.Value.OwnerPlayerID.GetHashCode() == playerID.GetHashCode())
                {
                    ourTerritories.Add(terr.Key);
                }
            }

            
        }

    }
}
