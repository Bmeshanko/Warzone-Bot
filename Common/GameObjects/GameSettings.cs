using System.Collections.Generic;

namespace WarLight.Shared.AI
{
    public enum RoundingModeEnum
    {
        WeightedRandom, StraightRound
    }

    public enum GameFogLevel : byte
    {
        NoFog,
        LightFog,
        ModerateFog, //Dense fog
        Foggy, //Normal Fog
        VeryFoggy, //Heavy fog
        ExtremeFog //Complete fog
    }
    public class GameSettings
    {
        public double OffenseKillRate;
        public double DefenseKillRate;
        public bool OneArmyStandsGuard;
        public int MinimumArmyBonus;
        public int InitialPlayerArmiesPerTerritory;
        public int InitialNeutralsInDistribution;
        public int InitialNonDistributionArmies;
        public int LimitDistributionTerritories;
        public DistributionIDType DistributionModeID;
        public Dictionary<BonusIDType, int> OverriddenBonuses;
        public bool Commanders;
        public bool AllowAttackOnly;
        public bool AllowTransferOnly;
        public RoundingModeEnum RoundingMode;
        public double LuckModifier;
        public bool MultiAttack;
        public bool AllowPercentageAttacks;
        public Dictionary<CardIDType, object> Cards;
        public bool LocalDeployments;
        public GameFogLevel FogLevel;
        public bool NoSplit;

        public int? CommerceCityBaseCost;
        public bool CommerceGame;

        public GameSettings(double offensiveKillRate, double defensiveKillRate, bool oneArmyMustStandGuard, int baseIncome, int initialNeutralsInDistribution, int initialNonDistributionArmies, int limitDistributionTerritories, DistributionIDType distributionModeID, Dictionary<BonusIDType, int> overriddenBonuses, bool commanders, bool allowAttackOnly, bool allowTransferOnly, int initialPlayerArmiesPerTerritory, RoundingModeEnum roundingMode, double luckModifier, bool multiAttack, bool allowPercentageAttacks, Dictionary<CardIDType, object> cards, bool localDeployments, GameFogLevel fog, bool nosplit)
        {

            this.OffenseKillRate = offensiveKillRate;
            this.DefenseKillRate = defensiveKillRate;
            this.OneArmyStandsGuard = oneArmyMustStandGuard;
            this.MinimumArmyBonus = baseIncome;
            this.InitialPlayerArmiesPerTerritory = initialPlayerArmiesPerTerritory;
            this.InitialNeutralsInDistribution = initialNeutralsInDistribution;
            this.InitialNonDistributionArmies = initialNonDistributionArmies;
            this.LimitDistributionTerritories = limitDistributionTerritories;
            this.DistributionModeID = distributionModeID;
            this.OverriddenBonuses = overriddenBonuses;
            this.Commanders = commanders;
            this.AllowAttackOnly = allowAttackOnly;
            this.AllowTransferOnly = allowTransferOnly;
            this.RoundingMode = roundingMode;
            this.LuckModifier = luckModifier;
            this.MultiAttack = multiAttack;
            this.AllowPercentageAttacks = allowPercentageAttacks;
            this.Cards = cards;
            this.LocalDeployments = localDeployments;
            this.FogLevel = fog;
            this.NoSplit = nosplit;
        }

        internal int HowManyArmiesCanAfford(int goldForArmies)
        {
            return goldForArmies; //not implemented
        }

        public int OneArmyMustStandGuardOneOrZero
        {
            get { return OneArmyStandsGuard ? 1 : 0; }
        }

        public int CostOfBuyingArmies(int totalArmiesDeployed)
        {
            return totalArmiesDeployed; //not implemented
        }
    }
}
