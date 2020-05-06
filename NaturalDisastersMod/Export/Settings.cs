using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace NaturalDisastersMod
{
    public class Settings
    {
        public static bool debug = true;

        #region Mod Settings

        public static float earthquakeChance = 0.15f;
        public static MinMax earthquakeStrength = new MinMax(1f, 4f);
        public static MinMax earthquakeVariance = new MinMax(0.1f, 0.9f);
        public static MinMax earthquakeLandElevation = new MinMax(0f, 0.3f);
        public static MinMax earthquakeWaterElevation = new MinMax(-2f, -0.25f);

        public static float droughtChance = 5;
        public static int droughtLength = 2;
        public static Assets.Code.ResourceAmount droughtFoodPenalty = new Assets.Code.ResourceAmount();

        public static float tornadoChance = 0.1f;

        public static bool happinessMods = true;

        #endregion

        public Settings()
        {
            droughtFoodPenalty.Set(FreeResourceType.Wheat, 4);
            droughtFoodPenalty.Set(FreeResourceType.Apples, 4);
        }

        


    }
}
