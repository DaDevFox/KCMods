using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace Elevation.Patches
{
    [HarmonyPatch(typeof(RadiusBonus), "OnBuilt")]
    public class RadiusBonusPatch
    {
        public static Dictionary<string, float> elevationMultipliers = new Dictionary<string, float>();
        public static Dictionary<string, float> elevationAdditiveMultipliers = new Dictionary<string, float>();

        public static float defaultElevationMultiplier = 5f;
        public static float defaultElevationAdditiveMultiplier = 10f;

        //static void Postfix(RadiusBonus __instance)
        //{
        //    float multiplier = elevationMultipliers.ContainsKey(__instance.b.UniqueName) ? elevationMultipliers[__instance.b.UniqueName] : defaultElevationMultiplier;
        //    float additiveMultiplier = elevationAdditiveMultipliers.ContainsKey(__instance.b.UniqueName) ? elevationAdditiveMultipliers[__instance.b.UniqueName] : defaultElevationAdditiveMultiplier;

        //    CellMeta meta = Grid.Cells.Get(__instance.b.GetCell());

        //    if (meta && meta.elevationTier > 0)
        //    {
        //        __instance.radiusMax = __instance.radiusMax * meta.elevationTier * multiplier + additiveMultiplier * meta.elevationTier;
        //    }
        //}
    }
}
