using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Orders
{
    public class AttackOrders
    {
        public Main.PrimeBot Bot;

        public AttackOrders(Main.PrimeBot bot)
        {
            this.Bot = bot;
        }

        public List<GameOrder> expansionAttacks(TerritoryIDType deployOn, int num) 
        {
            int attacksPossible = num / 3;
            int firstBoost = num % 3; // Example, if 8 armies, we wanna attack 3 and 5.
            List<GameOrder> attacks = new List<GameOrder>();
            List<TerritoryIDType> targets = Bot.ConnectedToInBonusNeutral(deployOn);
            if (targets.Count < attacksPossible)
            {
                firstBoost += 3 * (attacksPossible - targets.Count);
                attacksPossible = targets.Count;
            }

            attacks.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, deployOn, targets.First(), AttackTransferEnum.Attack, false, new Armies(3 + firstBoost), false));
            targets.Remove(targets.First());
            for (int i = 1; i < attacksPossible; i++)
            {
                attacks.Add(GameOrderAttackTransfer.Create(Bot.PlayerID, deployOn, targets.First(), AttackTransferEnum.Attack, false, new Armies(3), false));
                targets.Remove(targets.First());
            }
            return attacks;
        }
    }
}
