using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
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

        public static float defaultOffset = 0.005f;

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

        public static void UpdateBuilding(Building buidling)
        {
            Vector3 pos = buidling.transform.localPosition;
            Cell cell = buidling.GetCell();
            CellMeta meta = Grid.Cells.Get(cell);

            float stackHeight = 0;
            if (buidling.Stackable)
            {
                stackHeight = GetRelativeHeightOfBuildingAtIndex(cell, cell.OccupyingStructure.IndexOf(buidling));
            }
            if (buidling.CategoryName == "projectiletopper")
            {
                stackHeight = GetRelativeHeightTotal(cell);
            }

            if (meta != null)
            {
                // Better solution for [Experimental Elevation] required in this case; different buildings will be on different levels; perhaps need a building meta?
                float offset = offsets.ContainsKey(buidling.UniqueName) ? offsets[buidling.UniqueName] : 0f;
                
                buidling.transform.localPosition = new Vector3(pos.x, meta.Elevation + stackHeight + offset, pos.z);
                buidling.UpdateShaderHeight();
            }
        }

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

    [HarmonyPatch(typeof(Building), "OnPlacement")]
    public class BuildingOnPlacementPatch
    {
        static void Postfix(Building __instance)
        {
            List<OneOffEffect> buildFX = 
                typeof(Building)
                .GetField("buildEffects", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(__instance) as List<OneOffEffect>;
            foreach(OneOffEffect effect in buildFX)
            {
                Cell cell = World.inst.GetCellDataClamped(__instance.transform.position);
                if (cell == null)
                    continue;

                CellMeta meta = Grid.Cells.Get(cell);
                if (meta == null)
                    continue;

                // Better solution for [Experimental Elevation] required in this case; different buildings will be on different levels; perhaps need a building meta?
                effect.transform.position -= new Vector3(0f, meta.Elevation, 0f);
            }
        }
    }
}