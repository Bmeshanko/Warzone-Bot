using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class OrderManager
    {
        public Main.PrimeBot Bot;
        public PlayerIncomeTracker IncomeTracker;
        public List<GameOrder> Deploys;
        public List<GameOrder> Moves;
        public List<BonusIDType> BonusesCompleted;

        public OrderManager(Main.PrimeBot bot)
        {
            this.Bot = bot;
            this.IncomeTracker = new PlayerIncomeTracker(Bot.Incomes[Bot.PlayerID], Bot.Map);
            this.Moves = new List<GameOrder>();
            this.Deploys = new List<GameOrder>();
            this.BonusesCompleted = EvaluateBonusesCompleted();
        }

        public List<BonusIDType> EvaluateBonusesCompleted()
        {
            return Bot.Map.Bonuses.Keys.Where(o => Bot.leftToComplete(o) == 0).ToList();
        }

        public void Go()
        {
            // Find Enemy: Attack.

            // Completed our bonuses: MidGameExpansion.

            if (BonusesCompleted.Count < 3)
            {
                EarlyGameExpansion ege = new EarlyGameExpansion(Bot, this);
                ege.Go();
            }
            else
            {
                MidGameExpansion mge = new MidGameExpansion(Bot, this);
                mge.Go();
            }
        }
        public void Attack(TerritoryIDType from, List<TerritoryIDType> to)
        {
            int freeArmies = Bot.Standing.Territories[from].NumArmies.NumArmies - 1;
            foreach (GameOrderDeploy deploy in Deploys)
            {
                if (deploy.DeployOn == from)
                {
                    freeArmies += deploy.NumArmies;
                    break;
                }
            }

            foreach (GameOrderAttackTransfer attack in Moves)
            {
                if (attack.From == from) return;
            }

            while (to.Count > 0 && freeArmies >= 3)
            {
                AILog.Log("Attack", "Attacking from " + Bot.Map.Territories[from].Name + " to " + Bot.Map.Territories[to.First()].Name);
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

    }
}
