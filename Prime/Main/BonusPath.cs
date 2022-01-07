using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarLight.Shared.AI.Prime.Main
{
    public class BonusPath
    {
        public int armiesNeeded;
        public List<int> deployThisTurn;
        public List<TerritoryIDType> deployWhereThisTurn;
        public BonusIDType Bonus;
        public PrimeBot Bot;

        public BonusPath(BonusIDType bonus, PrimeBot bot)
        {
            this.Bonus = bonus;
            this.Bot = bot;
        }
        public void shortestPath(List<TerritoryIDType> from)
        {
            int toDeploy = 0;
            List<TerritoryIDType> queue = from;
            List<int> armiesQueue = new List<int>();
            List<TerritoryIDType> dump = new List<TerritoryIDType>();
            foreach(var terr in queue)
            {
                armiesQueue.Add(Bot.armiesOnTerritory(terr) - 1);
                dump.Add(terr);
            }
            while (queue.Count > 0)
            {
                AILog.Log("BonusPath", "Armies Needed: " + toDeploy);
                var terr = queue.Last();
                int armies = armiesQueue.Last();
                queue.Remove(terr);
                armiesQueue.Remove(armies);
                var borders = Bot.ConnectedToInBonusNeutral(terr).Where(o => !dump.Contains(o)).ToList();
                if (borders.Count == 1 && armies > 0)
                {
                    queue.Insert(0, borders.ElementAt(0));
                    dump.Add(borders.ElementAt(0));
                    armiesQueue.Insert(0, 1);
                    toDeploy += 3 - armies;
                    if (from.Contains(terr))
                    {
                        deployThisTurn.Add(3 - armies);
                        deployWhereThisTurn.Add(terr);
                    }
                }
                else if (borders.Count > 1 && armies > 0)
                {
                    queue.Insert(0, borders.ElementAt(0));
                    queue.Insert(0, borders.ElementAt(1));
                    dump.Add(borders.ElementAt(0));
                    dump.Add(borders.ElementAt(1));
                    armiesQueue.Insert(0, 1);
                    armiesQueue.Insert(0, 1);
                    toDeploy += 6 - armies;
                    if (from.Contains(terr))
                    {
                        deployThisTurn.Add(6 - armies);
                        deployWhereThisTurn.Add(terr);
                    }
                }
            }
            armiesNeeded = toDeploy;
        }
    }
}
