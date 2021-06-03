﻿using System;
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
            Picks.MakePicks picks = new Picks.MakePicks();
            return Picks.MakePicks.Commit(this);
        }

        public List<GameOrder> GetOrders()
        {
            return null;
        }

        public GamePlayer GamePlayerReference
        {
            get { return Players[PlayerID]; }
        }

    }
}