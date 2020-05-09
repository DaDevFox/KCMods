using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace ElevationExperiment.Patches
{
    [HarmonyPatch(typeof(PlacementMode), "UpdateBuildingAtPosition")]
    class BuildingPlacementPatch
    {
        static void Postfix(Building b)
        {
            Cell cell = World.inst.GetCellData(b.transform.position);
            CellMark mark = ElevationManager.GetCellMark(cell);
            float leveling = GetLevellingForBuilding(b);
            if (cell != null && mark != null)
            {
                b.transform.position = new Vector3(b.transform.position.x, leveling, b.transform.position.z);
            }
        }

        public static float GetLevellingForBuilding(Building b)
        {
            float max = 0f;
            b.ForEachTileInBounds(delegate(int x, int z, Cell cell)
            {
                float num = 0f;
                CellMark mark = ElevationManager.GetCellMark(cell);
                if(mark != null)
                {
                    num = mark.Elevation;
                }
                if(num > max)
                {
                    max = num;
                }
            });
            return max;
        }

    }
}
