using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLight.Shared.AI.Prime.Picks
{
    public class MakePicks
    {

        private static float GetWeight(Main.PrimeBot bot, TerritoryIDType terrID)
        {
            return 0.0f;
        }

        public static List<TerritoryIDType> Commit(Main.PrimeBot bot)
        {
            int maxPicks = bot.Settings.LimitDistributionTerritories == 0 ? bot.Map.Territories.Count : (bot.Settings.LimitDistributionTerritories * bot.Players.Values.Count(o => o.State == GamePlayerState.Playing));
            var allAvailable = bot.DistributionStanding.Territories.Values.Where(o => o.OwnerPlayerID == TerritoryStanding.AvailableForDistribution).Select(o => o.ID).ToHashSet(true);


            var expansionWeights = allAvailable.ToDictionary(o => o, o => GetWeight(bot, o));
            var ordered = expansionWeights.OrderByDescending(o => o.Value).ToList();

            var top = ordered.Take(maxPicks * 2);
            var sub = top.Min(o => o.Value) - 1;
            var normalized = top.ToDictionary(o => o.Key, o => o.Value - sub);

            var picks = new List<TerritoryIDType>();
            while (picks.Count < maxPicks && normalized.Count > 0)
            {
                var pick = RandomUtility.WeightedRandom(normalized.Keys, o => normalized[o]);
                picks.Add(pick);
                normalized.Remove(pick);
            }
            return picks;

        }

    }
}
