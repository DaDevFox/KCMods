using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elevation
{
    public static class BuildingBonuses
    {
        public static HappinessBonuses happinessBonuses { get; } = new HappinessBonuses();
    }

    public class HappinessBonuses
    {
        public static Dictionary<string, float> originalPrefabs_radiusMins = new Dictionary<string, float>();
        public static Dictionary<string, float> originalPrefabs_radiusMaxs = new Dictionary<string, float>();

        public static Dictionary<string, float> elevationMultipliers = new Dictionary<string, float>();
        public static Dictionary<string, float> elevationAdditiveMultipliers = new Dictionary<string, float>()
        {
            { "tavern", 0f },
            { "largetavern", 0f },
            { "theater", 0f },
            { "joustingarena", 0f }
        };

        public static float defaultElevationMultiplier = 0f;
        public static float defaultElevationAdditiveMultiplier = 1f;

        public static void Init()
        {
            originalPrefabs_radiusMaxs.Clear();
            originalPrefabs_radiusMins.Clear();

            foreach (Building building in GameState.inst.internalPrefabs)
            {
                RadiusBonus bonus = building.GetComponent<RadiusBonus>();
                if (bonus)
                {
                    originalPrefabs_radiusMaxs.Add(building.UniqueName, bonus.radiusMax);
                    originalPrefabs_radiusMins.Add(building.UniqueName, bonus.radiusMin);
                }
            }
        }

        public static void Update(Building building)
        {
            RadiusBonus bonus = building.GetComponent<RadiusBonus>();
            if (!bonus)
                return;

            float original_radiusMax = HappinessBonuses.originalPrefabs_radiusMaxs[building.UniqueName];
            float original_radiusMin = HappinessBonuses.originalPrefabs_radiusMins[building.UniqueName];

            float multiplier = elevationMultipliers.ContainsKey(building.UniqueName) ? elevationMultipliers[building.UniqueName] : defaultElevationMultiplier;
            float additiveMultiplier = elevationAdditiveMultipliers.ContainsKey(building.UniqueName) ? elevationAdditiveMultipliers[building.UniqueName] : defaultElevationAdditiveMultiplier;

            Cell cell = World.inst.GetCellDataClamped(building.transform.position);
            if (cell == null)
                return;

            CellMeta meta = Grid.Cells.Get(cell);

            if (meta && meta.elevationTier > 0)
            {
                bonus.radiusMax = original_radiusMax + meta.elevationTier * additiveMultiplier;
                bonus.radiusMin = original_radiusMin + meta.elevationTier * additiveMultiplier;
            }
            else
            {
                bonus.radiusMax = original_radiusMax;
                bonus.radiusMin = original_radiusMin;
            }
        }


        [HarmonyPatch(typeof(RadiusBonus), "DrawOverlay")]
        public class WhilePlacingCorrectionPatch
        {

            static void Prefix(RadiusBonus __instance)
            {
                Building building = __instance.b;
                if (building == null || GameUI.inst.CurrPlacementMode.GetHoverBuilding() != building)
                    return;

                Update(building);
            }
        }
    }

    public class WindmillBonuses
    {
        [HarmonyPatch(typeof(Field), "RefreshBonuses")]
        public class FieldBonusesPatch
        {
            static bool Prefix(Field __instance, ref float ___irrigationBonus, ref int ___windmillsNearby)
            {
                Building component = __instance.GetComponent<Building>();
                Cell cell = component.GetCell();
                ___irrigationBonus = (float)Mathff.Clamp(cell.IrrigationCoverage, 0, 1);
                ___windmillsNearby = 0;

                // slightly more optimized but doesn't leave room for change of windmill effect radius
                // by future game versions (without according adjustment here)

                Cell[] neighbors = new Cell[8];
                World.inst.GetNeighborCellsExtended(cell, ref neighbors);

                foreach(Cell neighbor in neighbors)
                {
                    Building mill = neighbor.StructureFind<Windmill>();
                    if (mill && mill.IsBuilt() && mill.IsOpen() && mill.WorkersAllocated > 0)
                    {
                        int value = Grid.Cells.Get(mill.GetCell()).elevationTier + 1;
                        if (___windmillsNearby < value)
                            ___windmillsNearby = value;
                    }
                }

                return false;
            }
        }
    }

    //[HarmonyPatch(typeof(RadiusBonus), "OnBuilt")]
    //public class RadiusBonusPatch
    //{
    //    public static Dictionary<string, float> elevationMultipliers = new Dictionary<string, float>()
    //    {
    //        { "tavern", 25f }
    //    };
    //    public static Dictionary<string, float> elevationAdditiveMultipliers = new Dictionary<string, float>();

    //    public static float defaultElevationMultiplier = 5f;
    //    public static float defaultElevationAdditiveMultiplier = 10f;

    //    static void Postfix(RadiusBonus __instance)
    //    {
    //        float multiplier = elevationMultipliers.ContainsKey(__instance.b.UniqueName) ? elevationMultipliers[__instance.b.UniqueName] : defaultElevationMultiplier;
    //        float additiveMultiplier = elevationAdditiveMultipliers.ContainsKey(__instance.b.UniqueName) ? elevationAdditiveMultipliers[__instance.b.UniqueName] : defaultElevationAdditiveMultiplier;

    //        CellMeta meta = Grid.Cells.Get(__instance.b.GetCell());

    //        if (meta && meta.elevationTier > 0)
    //        {
    //            __instance.radiusMax = __instance.radiusMax * meta.elevationTier * multiplier + additiveMultiplier * meta.elevationTier;
    //        }
    //    }
    //}

}
