using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class MakeMoves
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;
        public List<GameOrder> Moves;
        public List<BonusIDType> Bonuses;

        public MakeMoves(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = new List<GameOrder>();
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

        public void Attack(TerritoryIDType from, List<TerritoryIDType> to)
        {
            int freeArmies = Bot.Standing.Territories[from].NumArmies.NumArmies - 1;
            foreach(GameOrderDeploy deploy in Deploys)
            {
                if (deploy.DeployOn == from)
                {
                    freeArmies += deploy.NumArmies;
                }
            }

            while (to.Count > 0 && freeArmies >= 3)
            {
                if (freeArmies < 6 || to.Count == 1)
                {
                    Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, from, to.First(), AttackTransferEnum.AttackTransfer, false, new Armies(freeArmies), false));
                    to.Remove(to.First());
                    freeArmies = 0;
                } 
                else
                {
                    Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, from, to.First(), AttackTransferEnum.AttackTransfer, false, new Armies(3), false));
                    to.Remove(to.First());
                    freeArmies -= 3;
                }
            }
        }

        public void AttackRest(BonusIDType notHere, List<TerritoryIDType> ourTerritories)
        {
            // We should submit attacks from everywhere else we deployed.
            foreach(var terr in ourTerritories)
            {
                if (Bot.Map.Bonuses[notHere].Territories.Contains(terr)) continue;
                var attackTargets = Bot.Map.Territories.Keys.Where(o => Bot.Map.Territories[o].ConnectedTo.Keys.Contains(terr)).ToList();

                Attack(terr, attackTargets);
            }
        }

        public void TakeFirstBonuses()
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
            int highestConnects = 0;
            foreach (var terr in Bot.Map.Territories.Keys.Where(o => Bot.Map.Bonuses[fewestRemainingBonus].Territories.Contains(o) && territoriesOwned.Contains(o)).ToList())
            {
                int numConnects = Bot.ConnectedToInBonus(terr).Where(o => Bot.Standing.Territories[o].IsNeutral).ToList().Count;
                if (numConnects > highestConnects)
                {
                    highestConnects = numConnects;
                    deployOn = terr;
                }
            }

            Armies armiesLeft = new Armies(IncomeTracker.FreeArmiesUndeployed);
            int armies = Bot.Standing.Territories[deployOn].NumArmies.NumArmies - 1;
            int deploysNeeded = (highestConnects * 3) - (armies);
            
            var attackTargets = territoriesToTake.Where(o => Bot.Map.Territories[o].ConnectedTo.Keys.Contains(deployOn)).ToList();
            
            if (IncomeTracker.FreeArmiesUndeployed + armies < deploysNeeded)
            {
                // Highest multiple of 3 less than FreeArmies.
                deploysNeeded = IncomeTracker.FreeArmiesUndeployed + armies;
                deploysNeeded -= deploysNeeded % 3;
            }
            
            if (attackTargets.Count == 1)
            {
                deploysNeeded = IncomeTracker.FreeArmiesUndeployed;
            }
            if (deploysNeeded > 0)
            {
                Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, deploysNeeded, deployOn, false));
                AILog.Log("MakeMoves", "Deploying " + deploysNeeded + " to " + Bot.Map.Territories[deployOn].Name);
                armiesLeft = new Armies(armiesLeft.NumArmies - deploysNeeded);
            }

           
            if (attackTargets.Count == 1)
            {
                Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, deployOn, attackTargets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(armies + deploysNeeded), false));
            } 
            else
            {
                Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, deployOn, attackTargets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(3), false));
                attackTargets.Remove(attackTargets.First());
                Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, deployOn, attackTargets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(3), false));

            }
            if (IncomeTracker.FreeArmiesUndeployed - deploysNeeded > 0)
            {
                DeployRest(secondFewestRemainingBonus, deploysNeeded);
                AttackRest(fewestRemainingBonus, territoriesOwned);
            }
        }

        public void Go() // Expansion.
        {
            List<BonusIDType> bonuses = new List<BonusIDType>();
            Dictionary<TerritoryIDType, TerritoryIDType> fromTo = new Dictionary<TerritoryIDType, TerritoryIDType>(); // Bordered, unowned, and in bonuses.
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

            var borders = Bot.Map.Territories.Keys.Where(o => Bot.Map.Territories[o].ConnectedTo.Keys.Any(z => territoriesOwned.Contains(z))).ToHashSet(true);

            Armies armiesLeft = new Armies(IncomeTracker.FreeArmiesUndeployed);

            List<TerritoryDetails> terrDetails = Bot.Map.Territories.Values.ToList();
            var ourTerr = terrDetails.Where(o => Bot.Standing.Territories[o.ID].OwnerPlayerID == Bot.PlayerID).ToList();
            Dictionary<TerritoryIDType, int> deployedTo = new Dictionary<TerritoryIDType, int>();

            TerritoryIDType priority = new TerritoryIDType();
            int lowest = 1000; // Arbitrary high number - no bonus this bot will play will have that many.
            foreach (var terr in ourTerr)
            {
                var targets = Bot.ConnectedToInBonus(terr.ID).Where(o => Bot.Standing.Territories[o].IsNeutral).ToList();

                int left = Bot.leftToComplete(Bot.whatBonus(terr.ID));
                if (left < lowest && targets.Count > 0)
                {
                    lowest = left;
                    priority = terr.ID;
                }
            }


            // Deploy and Attack for Priority.
            List<TerritoryIDType> priorityTargets = Bot.ConnectedToInBonus(priority).Where(o => Bot.Standing.Territories[o].IsNeutral).ToList();

            int priorityArmies = Bot.Standing.Territories[priority].NumArmies.NumArmies;
            int deploysNeeded = priorityArmies - (lowest * 3) - 1;
            if (deploysNeeded > 0 && deploysNeeded <= armiesLeft.NumArmies)
            {
                Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, deploysNeeded, priority, false));
                AILog.Log("MakeMoves", "Deploying " + deploysNeeded + " to " + Bot.Map.Territories[priority].Name);
                armiesLeft = new Armies(armiesLeft.NumArmies - deploysNeeded);
            }
            else if (deploysNeeded > 0)
            {
                Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, armiesLeft.NumArmies, priority, false));
                AILog.Log("MakeMoves", "Deploying " + armiesLeft.NumArmies + " to " + Bot.Map.Territories[priority].Name);
                armiesLeft = new Armies(0);

            }

            while (priorityTargets.Count > 0) {
                Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, priority, priorityTargets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(3), false));
                priorityTargets.Remove(priorityTargets.First());
            }

            foreach (var terr in ourTerr)
            {
                if (terr.ID == priority) continue; 
                var targets = Bot.ConnectedToInBonus(terr.ID).Where(o => Bot.Standing.Territories[o].IsNeutral).ToList();
                // We know it's neutral, in our bonus, and we border it. Now we need to deploy and attack.
                
                int currentArmies = Bot.Standing.Territories[terr.ID].NumArmies.NumArmies;

                foreach (var opponent in Bot.Map.Territories.Values)
                {
                    if (opponent.ConnectedTo.ContainsKey(terr.ID) && Bot.Standing.Territories[opponent.ID].OwnerPlayerID != Bot.PlayerID)
                    {
                        targets.Add(opponent.ID);
                    }
                }

                if (armiesLeft.NumArmies >= 3 && !deployedTo.Keys.Contains(terr.ID) && targets.Count > 0)
                {
                    currentArmies += 3;
                    Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, 3, terr.ID, false));
                    deployedTo.Add(terr.ID, currentArmies);
                    armiesLeft = new Armies(armiesLeft.NumArmies - 3);
                    AILog.Log("MakeMoves", "Deploying 3 to " + terr.Name);
                } 
                else if (armiesLeft.NumArmies > 0 && !deployedTo.Keys.Contains(terr.ID) && targets.Count > 0)
                {
                    currentArmies += armiesLeft.NumArmies;
                    Deploys.Add(GameOrderDeploy.Create(Bot.PlayerID, armiesLeft.NumArmies, terr.ID, false));
                    deployedTo.Add(terr.ID, armiesLeft.NumArmies);
                    armiesLeft = new Armies(0);
                }

                while (targets.Count > 0 && currentArmies > 3)
                {
                    int numAttack = 3;
                    Moves.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, terr.ID, targets.First(), AttackTransferEnum.AttackTransfer, false, new Armies(numAttack), false));
                    targets.Remove(targets.First());
                    currentArmies -= numAttack;
                }
            }
        }

        public TerritoryIDType RandomNeutralInBonus(Main.PrimeBot bot, TerritoryIDType from)
        {
            var bonus = bot.Map.Territories[from].PartOfBonuses.First(); // This will not work with SuperBonuses.
            var bonusDetails = bot.Map.Bonuses[bonus];
            var territoriesInBonus = bonusDetails.Territories.ToHashSet(true); // Territories in the Bonus.

            var connections = territoriesInBonus.Where(o => bot.Map.Territories[o].ConnectedTo.Keys.Any(z => z.Equals(from))).ToHashSet(true);
            var neutralConnects = connections.Where(o => bot.Standing.Territories[o].IsNeutral);

            return neutralConnects.Random();
        }


    }
}
