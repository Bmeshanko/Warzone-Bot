using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class EarlyGameExpansion
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;
        public List<GameOrder> Moves;
        public List<BonusIDType> Bonuses;
        public OrderManager Parent;

        public EarlyGameExpansion(Main.PrimeBot bot, OrderManager parent)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = new List<GameOrder>();
            this.Parent = parent;
        }
        public void DeployRest(BonusIDType bonus, int armiesDeployed)
        {
            Armies armiesLeft = new Armies(IncomeTracker.FreeArmiesUndeployed);
            foreach (var terr in Bot.Map.Bonuses[bonus].Territories)
            {
                if (Bot.Standing.Territories[terr].OwnerPlayerID == Bot.PlayerID && armiesLeft.NumArmies > 0)
                {
                    Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, armiesLeft.NumArmies - armiesDeployed, terr, false));
                    AILog.Log("MakeMoves", "Deploying " + (armiesLeft.NumArmies - armiesDeployed) + " to " + Bot.Map.Territories[terr].Name);
                    armiesLeft = new Armies(0);
                }
            }
        }
        public void AttackRest(List<TerritoryIDType> ourTerritories)
        {
            
            // We should submit attacks from everywhere else we deployed.
            foreach(var terr in ourTerritories)
            {

                var attackTargets = Bot.ConnectedToInBonusNeutral(terr);
                Parent.Attack(terr, attackTargets);
            }
        }

        public void Go()
        {
            List<BonusIDType> bonuses = new List<BonusIDType>();
            List<TerritoryIDType> territoriesOwned = new List<TerritoryIDType>(); // Owned by us.

            foreach (var bonus in Bot.Map.Bonuses.Values)
            {
                foreach (var terr in bonus.Territories)
                {
                    if (Bot.Standing.Territories[terr].OwnerPlayerID == Bot.PlayerID)
                    {
                        bonuses.Add(bonus.ID);
                        territoriesOwned.Add(terr);
                    }
                }
            }

            int fewestRemaining = 10000;
            BonusIDType fewestRemainingBonus = bonuses.First();
            int secondFewestRemaining = 10000;
            BonusIDType secondFewestRemainingBonus = bonuses.First();
            foreach(var bonus in bonuses)
            {
                int count = Bot.leftToComplete(bonus);

                if (count > 0 && count < fewestRemaining)
                {
                    fewestRemaining = count;
                    fewestRemainingBonus = bonus;
                }
            }

            foreach (var bonus in bonuses)
            {
                int count = 0;
                foreach (var terr in Bot.Map.Bonuses[bonus].Territories)
                {
                    if (Bot.Standing.Territories[terr].OwnerPlayerID != Bot.PlayerID && Bot.whatBonus(terr) != fewestRemainingBonus)
                    {
                        count++;
                    }
                }

                if (count > 0 && count < secondFewestRemaining)
                {
                    secondFewestRemaining = count;
                    secondFewestRemainingBonus = bonus;
                }
            }

            var territoriesToTake = Bot.Map.Territories.Keys.Where(o => Bot.Map.Bonuses[fewestRemainingBonus].Territories.Contains(o) && !territoriesOwned.Contains(o)).ToList();

            TerritoryIDType deployOn = new TerritoryIDType();
            int highestConnects = -1;
            List<TerritoryIDType> attackTargets = new List<TerritoryIDType>();
            foreach (var terr in Bot.Map.Territories.Keys.Where(o => Bot.Map.Bonuses[fewestRemainingBonus].Territories.Contains(o) && territoriesOwned.Contains(o)).ToList())
            {

                var targets = Bot.ConnectedToInBonusNeutral(terr);
                int numConnects = targets.Count;
                if (numConnects > highestConnects)
                {
                    highestConnects = numConnects;
                    deployOn = terr;
                    attackTargets = targets;
                } else if (numConnects == highestConnects)
                {
                    if (Bot.Standing.Territories[terr].NumArmies.NumArmies > Bot.Standing.Territories[deployOn].NumArmies.NumArmies)
                    {
                        deployOn = terr;
                        attackTargets = targets;
                    }
                }
            }

            Armies armiesLeft = new Armies(IncomeTracker.FreeArmiesUndeployed);
            int armies = Bot.Standing.Territories[deployOn].NumArmies.NumArmies - 1;
            int deploysNeeded = (highestConnects * 3) - (armies);

            if (armiesLeft.NumArmies < deploysNeeded)
            {
                // Highest multiple of 3 less than FreeArmies.
                deploysNeeded = armiesLeft.NumArmies;
                deploysNeeded -= deploysNeeded % 3;
            }

            if (deploysNeeded > 0)
            {
                Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, deploysNeeded, deployOn, false));
                AILog.Log("MakeMoves", "Deploying " + deploysNeeded + " to " + Bot.Map.Territories[deployOn].Name);
                armiesLeft = new Armies(armiesLeft.NumArmies - deploysNeeded);
            }

            Parent.Attack(deployOn, attackTargets);

            if (armiesLeft.NumArmies > 0)
            {
                DeployRest(secondFewestRemainingBonus, deploysNeeded);
            }
            AttackRest(territoriesOwned);
        }
    }
}
