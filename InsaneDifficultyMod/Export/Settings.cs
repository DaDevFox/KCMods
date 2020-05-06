using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace InsaneDifficultyMod
{
    public class Settings
    {
        public static int mode = 1;
        public static bool debug = false;

        #region Mod Settings

        public static bool randomEvents = true;

        public static float earthquakeChance = 0.15f;
        public static MinMax earthquakeStrength = new MinMax(1f, 4f);
        public static MinMax earthquakeVariance = new MinMax(0.1f, 0.9f);
        public static MinMax earthquakeLandElevation = new MinMax(0f, 0.3f);
        public static MinMax earthquakeWaterElevation = new MinMax(-2f, -0.25f);

        public static float droughtChance = 5;
        public static int droughtLength = 2;
        public static Assets.Code.ResourceAmount droughtFoodPenalty = new Assets.Code.ResourceAmount();

        public static bool happinessMods = true;

        public static float foodPenalty = 2f;

        public static int vikingRaidYearSpan = 3;
        public static int vikingRaidYearRange = 1;

        public static MinMax fireBurnoutTimeRange = new MinMax(8f, 15f);
        public static float firePersistance = 1f;

        public static int minArmyAmountForOppression = 3;
        public static int oppresssionRamp = 8;

        public static int riotMaxSize = 20;
        public static int riotStartSize = 10;

        #endregion

        public Settings()
        {
            droughtFoodPenalty.Set(FreeResourceType.Wheat, 4);
            droughtFoodPenalty.Set(FreeResourceType.Apples, 4);
        }

        public static void Update()
        {
            switch (mode)
            {
                case 0:
                    randomEvents = true;
                    happinessMods = true;
                    foodPenalty = 1;
                    earthquakeChance = 0.05f;
                    earthquakeStrength = new MinMax(1f, 4f);
                    droughtChance = 3;
                    vikingRaidYearRange = 2;
                    vikingRaidYearSpan = 7;
                    fireBurnoutTimeRange = new MinMax(8f, 15f);
                    firePersistance = 1f;
                    break;
                case 1:
                    randomEvents = true;
                    happinessMods = true;
                    earthquakeChance = 0.10f;
                    earthquakeStrength = new MinMax(2f, 5f);
                    droughtChance = 7;
                    droughtLength = 1;
                    foodPenalty = 1.2f;
                    vikingRaidYearRange = 1;
                    vikingRaidYearSpan = 6;
                    fireBurnoutTimeRange = new MinMax(5f, 15f);
                    firePersistance = 1f;
                    break;
                case 2:
                    randomEvents = true;
                    happinessMods = true;
                    earthquakeChance = 0.125f;
                    earthquakeStrength = new MinMax(3f, 6f);
                    droughtChance = 12;
                    droughtLength = 1;
                    foodPenalty = 1.5f;
                    vikingRaidYearRange = 2;
                    vikingRaidYearSpan = 5;
                    fireBurnoutTimeRange = new MinMax(5f, 13f);
                    firePersistance = 2f;
                    break;
                case 3:
                    randomEvents = true;
                    happinessMods = true;
                    earthquakeChance = 0.14f;
                    earthquakeStrength = new MinMax(2f, 7f);
                    droughtChance = 14;
                    droughtLength = 2;
                    foodPenalty = 1.8f;
                    vikingRaidYearRange = 2;
                    vikingRaidYearSpan = 4;
                    fireBurnoutTimeRange = new MinMax(5f, 12f);
                    firePersistance = 4f;
                    break;
                case 4:
                    randomEvents = true;
                    happinessMods = true;
                    earthquakeChance = 0.2f;
                    earthquakeStrength = new MinMax(2f, 9f);
                    droughtChance = 16;
                    droughtLength = 4;
                    foodPenalty = 2.5f;
                    vikingRaidYearRange = 2;
                    vikingRaidYearSpan = 4;
                    fireBurnoutTimeRange = new MinMax(2f, 10f);
                    firePersistance = 10f;
                    break;
            }
            ApplyGameVars();
        }

        public static void ApplyGameVars() 
        {
            Player.inst.SecondsPerEat = 150f / foodPenalty;

            RaiderSystem.inst.MinAttackYearSpan = vikingRaidYearSpan - vikingRaidYearRange;
            RaiderSystem.inst.MaxAttackYearSpan = vikingRaidYearSpan + vikingRaidYearRange;

        }

        [HarmonyPatch(typeof(Fire), "Init")]
        public class FireInitPatch
        {
            static void Prefix(Fire __instance)
            {
                __instance.BurnoutTimeRange = Settings.fireBurnoutTimeRange;
                __instance.life = Settings.firePersistance;


            }

        }


    }
}
