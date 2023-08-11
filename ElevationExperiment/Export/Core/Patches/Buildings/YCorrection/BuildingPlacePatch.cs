using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Elevation
{
    public static class BuildingFormatter
    {
        public static Dictionary<string, float> offsets = new Dictionary<string, float>()
        {
            { "road", 0.05f },
            { "stoneroad", 0.05f },
            { "garden", 0.1f },
            { "farm", 0.05f },
            { "townsquare", 0.01f },
            { "largefountain", 0.01f },
            { "cemetery", 0.01f },
            { "cemetery44", 0.01f },
            { "cemeteryCircle", 0.01f },
            { "cemeteryDiamond", 0.01f }
        };

        public static float defaultOffset = 0f;

        public static bool UnevenTerrain(this Building building)
        {
            Cell firstCell = building.GetCell();
            bool flag = false;
            CellMeta firstMeta = Grid.Cells.Get(firstCell);
            if (firstCell != null && firstMeta != null)
            {

                int elevationTier = firstMeta.elevationTier;

                building.ForEachTileInBounds(delegate (int x, int y, Cell cell)
                {
                    CellMeta meta = Grid.Cells.Get(cell);
                    if (meta != null)
                    {
                        if (meta.elevationTier != elevationTier)
                        {
                            flag = true;
                        }
                    }
                });
            }

            return flag;
        }



        public static int GetStackPosOfBuildingAtIndex(this Cell cell, int idx)
        {
            int count = 0;
            int i = 0;
            while (count < cell.OccupyingStructure.Count)
            {
                if (count == idx)
                {
                    return i;
                }

                i += cell.OccupyingStructure[i].StackHeight;
                count++;
            }
            return -1;
        }

        /// <summary>
        /// Returns the stack height of a building at an index on the cell, relative to the elevation of the building's position
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static float GetRelativeHeightOfBuildingAtIndex(this Cell cell, int idx)
        {
            int count = 0;
            float i = 0;
            while (count < cell.OccupyingStructure.Count)
            {
                float stackHeight = 0f;


                if (cell.OccupyingStructure[count].Stackable)
                    stackHeight = cell.OccupyingStructure[count].StackHeight * 0.25f;


                if (count == idx)
                {
                    return i;
                }


                i += stackHeight;
                count++;
            }
            return 0;
        }

        /// <summary>
        /// Gets the actual height of the building at the index on a cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static float GetAbsoluteHeightOfBuildingAtIndex(this Cell cell, int idx)
        {
            if (cell == null)
                return 0f;

            CellMeta meta = Grid.Cells.Get(cell);
            int count = 0;
            float i = 0;
            if (meta != null)
                i = meta.Elevation;

            DebugExt.dLog("absolute stack height total");

            while (count < cell.OccupyingStructure.Count)
            {
                float stackRealHeight = 0f;

                if (cell.OccupyingStructure[count].Stackable)
                    stackRealHeight += cell.OccupyingStructure[count].StackHeight * 0.25f;

                i += stackRealHeight;


                if (count == idx)
                {
                    DebugExt.dLog(i);
                    return i;
                }

                count++;
            }
            return 0;
        }

        /// <summary>
        /// Gets the height of the cell relative to its position on an elevation block
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static float GetRelativeHeightTotal(this Cell cell)
        {
            int count = 0;
            float i = 0;
            while (count < cell.OccupyingStructure.Count)
            {
                float stackRealHeight = 0f;

                if (cell.OccupyingStructure[count].Stackable)
                {
                    stackRealHeight = (float)cell.OccupyingStructure[count].StackHeight * 0.25f;
                }

                i += stackRealHeight;
                count++;
            }
            return i;
        }

        /// <summary>
        /// Gets the real height of a cell
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static float GetAbsoluteHeightTotal(this Cell cell)
        {
            CellMeta meta = Grid.Cells.Get(cell);

            float i = 0;
            if (meta != null)
                i = meta.Elevation;

            bool flag = false;

            for (int count = 0; count < cell.OccupyingStructure.Count; count++)
            {
                float stackRealHeight = 0f;



                if (cell.OccupyingStructure[count].Stackable)
                {
                    stackRealHeight += cell.OccupyingStructure[count].StackHeight * 0.25f;
                    flag = true;
                }

                i += stackRealHeight;
            }
            if (flag)
                return i;
            else
                return 0;
        }

        public static void UpdateBuilding(Building building)
        {
            Vector3 pos = building.transform.localPosition;
            Cell cell = building.GetCell();
            CellMeta meta = Grid.Cells.Get(cell);

            float stackHeight = 0;
            if (building.Stackable)
            {
                stackHeight = GetRelativeHeightOfBuildingAtIndex(cell, cell.OccupyingStructure.IndexOf(building));
            }
            if (building.CategoryName == "projectiletopper")
            {
                stackHeight = GetRelativeHeightTotal(cell);
            }

            if (meta != null)
            {
                // Better solution for [Experimental Elevation] required in this case; different buildings will be on different levels; perhaps need a building meta?
                float offset = offsets.ContainsKey(building.UniqueName) ? offsets[building.UniqueName] : defaultOffset;
                
                building.transform.localPosition = new Vector3(pos.x, meta.Elevation + stackHeight + offset, pos.z);
                building.UpdateShaderHeight();

                HappinessBonuses.Update(building);
            }
        }

        public static float GetBuildingVisualOffset(string buildingUniqueName) => offsets.ContainsKey(buildingUniqueName) ? offsets[buildingUniqueName] : defaultOffset;

        public static void UpdateBuildingsOnCell(Cell cell)
        {
            foreach(Building building in cell.OccupyingStructure)
                UpdateBuilding(building);
        }
    }
}

namespace Elevation.Patches
{
    



    [HarmonyPatch(typeof(World), "PlaceInternal")]
    public class BuildingPlacePatch
    {
        static void Postfix(Building PendingObj, bool undo)
        {
            if (!undo)
                Grid.Buildings.Add(PendingObj);
            else
                Grid.Buildings.Remove(PendingObj);

            //if (!BuildingFormatter.UnevenTerrain(PendingObj))
            //{
            //    BuildingFormatter.UpdateBuilding(PendingObj);
            //}
        }

        
    }

    //[HarmonyPatch(typeof(Building), "OnPlacementPuff")]
    //public class BuildFXPatch
    //{
    //    static bool Prefix(Building __instance, List<OneOffEffect> ___buildEffects)
    //    {
    //        CellMeta meta = Grid.Cells.Get(__instance.GetCell());
    //        if (!meta)
    //            return true;

    //        if (__instance.IsVisibleForFog())
    //        {
    //            __instance.ForEachTileInBounds(delegate (int x, int z, Cell cell)
    //            {
    //                OneOffEffect oneOffEffect = EffectsMan.inst.BuildEffect.CreateAndPlay(new Vector3((float)x, 0f, (float)z));
    //                oneOffEffect.AllowRelease = true;
    //                ___buildEffects.Add(oneOffEffect);
    //            });

    //            return false;
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(Building), "UpdateConstruction")]
    //public class BuildFXConstructionPatch
    //{
    //    static void Postfix(Building __instance, List<OneOffEffect> ___buildEffects)
    //    {
    //        CellMeta meta = Grid.Cells.Get(__instance.GetCell());
    //        if (!meta)
    //            return;

    //        if (__instance.constructionProgress >= 1f)
    //        {
    //            foreach (OneOffEffect effect in ___buildEffects)
    //            {
    //                effect.transform.position = new Vector3(effect.transform.position.x, meta.Elevation, effect.transform.position.z);
    //            }
    //        }
    //    }
    //}
}