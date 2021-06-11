using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WarLight.Shared.AI.Prime.Main
{

    public class PrimeBot : IWarLightAI
    {
        public GameIDType GameID;
        public PlayerIDType PlayerID;
        public Dictionary<PlayerIDType, GamePlayer> Players;
        public MapDetails Map;
        public GameStanding DistributionStanding;
        public GameSettings Settings;
        public int NumberOfTurns;
        public Dictionary<PlayerIDType, PlayerIncome> Incomes;
        public PlayerIncome BaseIncome;
        public GameOrder[] prevTurn;
        public GameStanding Standing;
        public GameStanding previousTurnStanding;
        public Dictionary<PlayerIDType, TeammateOrders> TeammatesOrders;
        public List<CardInstance> Cards;
        public int CardsMustPlay;
        public Stopwatch timer;
        public List<String> directives;
        public List<GamePlayer> Opponents;


        public String Name()
        {
            return "Prime Version 1.0";
        }

        public String Description()
        {
            return "Bot developed by Benjamin628 in Summer 2021.";
        }

        public bool SupportsSettings(GameSettings settings, out string whyNot)
        {
            whyNot = null;
            return true; // Come back later - No way I make a bot that understands all settings.
        }

        public bool RecommendsSettings(GameSettings settings, out string whyNot)
        {
            whyNot = null;
            return true; // Come back later - No way I make a bot that understands all settings.
        }
        public void Init(GameIDType gameID, PlayerIDType myPlayerID, Dictionary<PlayerIDType, GamePlayer> players, MapDetails map, GameStanding distributionStanding, GameSettings gameSettings, int numberOfTurns, Dictionary<PlayerIDType, PlayerIncome> incomes, GameOrder[] prevTurn, GameStanding latestTurnStanding, GameStanding previousTurnStanding, Dictionary<PlayerIDType, TeammateOrders> teammatesOrders, List<CardInstance> cards, int cardsMustPlay, Stopwatch timer, List<string> directives)
        {
            this.DistributionStanding = distributionStanding;
            this.Standing = latestTurnStanding;
            this.PlayerID = myPlayerID;
            this.Players = players;
            this.Map = map;
            this.Settings = gameSettings;
            this.TeammatesOrders = teammatesOrders;
            this.Cards = cards;
            this.CardsMustPlay = cardsMustPlay;
            this.Incomes = incomes;
            this.BaseIncome = Incomes[PlayerID];
        }
        public List<TerritoryIDType> GetPicks()
        {
            Picks.MakePicks picks = new Picks.MakePicks(this);
            return picks.Commit(this);
        }

        public List<GameOrder> GetOrders()
        {
            Orders.CommitOrders co = new Orders.CommitOrders(this);
            return co.Commit();
        }

        public GamePlayer GamePlayerReference
        {
            get { return Players[PlayerID]; }
        }

        public int BonusValue(BonusIDType bonusID)
        {
            if (Settings.OverriddenBonuses.ContainsKey(bonusID))
                return Settings.OverriddenBonuses[bonusID];
            else
                return Map.Bonuses[bonusID].Amount;
        }

        public int NumTerritories(BonusIDType bonusID)
        {
            return Map.Bonuses[bonusID].Territories.ToArray().Length;
        }

        public string TerrString(TerritoryIDType terrID)
        {
            return Map.Territories[terrID].Name;
        }

        public String BonusString(BonusIDType bonusID)
        {
            return Map.Bonuses[bonusID].Name;
        }

        public List<TerritoryIDType> ConnectedToInBonus(TerritoryIDType terrID)
        {
            BonusIDType bonus = Map.Territories[terrID].PartOfBonuses.First();
            return Map.Territories.Keys.Where(o => Map.Territories[o].PartOfBonuses.First() == bonus && o != terrID && Map.Territories[o].ConnectedTo.Keys.Contains(terrID)).ToList();
        }

        public List<TerritoryIDType> ConnectedToInBonusNeutral(TerritoryIDType terrID)
        {
            BonusIDType bonus = Map.Territories[terrID].PartOfBonuses.First();
            return ConnectedToInBonus(terrID).Where(o => Standing.Territories[o].IsNeutral).ToList();
        }

        public int armiesToTakeNeutrals()
        {
            return 3;
        }

        public int leftovers()
        {
            return 1;
        }

        public int leftToComplete(BonusIDType bonusID)
        {
            int count = 0;
            foreach(var territory in Map.Bonuses[bonusID].Territories)
            {
                if (Standing.Territories[territory].OwnerPlayerID != PlayerID)
                {
                    count++;
                }
            }
            return count;
        }

        public BonusIDType whatBonus(TerritoryIDType terrID)
        {
            return Map.Territories[terrID].PartOfBonuses.First();
        }

        public List<TerritoryIDType> OurTerritories()
        {
            return Standing.Territories.Keys.Where(o => Standing.Territories[o].OwnerPlayerID == PlayerID).ToList();
        }

        public bool FoundEnemy()
        {
            foreach (var territory in OurTerritories())
            {
                var borders = Map.Territories[territory].ConnectedTo.Keys.ToList();
                foreach (var terr in borders)
                {
                    if (Standing.Territories[terr].OwnerPlayerID != PlayerID && !Standing.Territories[terr].IsNeutral)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public HashSet<BonusIDType> bonusNeighbors(Main.PrimeBot bot, BonusIDType bonusID)
        {
            BonusDetails bonus = bot.Map.Bonuses[bonusID];


            var terrs = bonus.Territories.ToHashSet(true);
            var neighbors = new HashSet<BonusIDType>();

            foreach (var terr in terrs)
            {
                var connections = terrs.Where(o => bot.Map.Territories[o].ConnectedTo.Keys.Any(z => terrs.Contains(z))).ToHashSet(true);
                foreach (var territory in connections)
                {
                    TerritoryDetails details = bot.Map.Territories[territory];
                    BonusIDType partOf = details.PartOfBonuses.First(); // Will not work with superbonuses.
                    if (partOf.GetHashCode() != bonusID.GetHashCode() && !neighbors.Contains(partOf))
                    {
                        neighbors.Add(partOf);
                    }
                }
            }
            return neighbors;
        }

        public List<BonusIDType> Wastelands()
        {
            List<BonusIDType> bonuses = new List<BonusIDType>();
            foreach(var bonus in Map.Bonuses)
            {
                BonusDetails details = bonus.Value;
                foreach(var terr in details.Territories)
                {
                    if (DistributionStanding.Territories[terr].NumArmies.NumArmies != Settings.InitialNonDistributionArmies && DistributionStanding.Territories[terr].NumArmies.NumArmies != Settings.InitialNeutralsInDistribution)
                    {
                        bonuses.Add(bonus.Key);
                        break;
                    }
                }
            }
            return bonuses;
        }

    }
}
