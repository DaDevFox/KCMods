using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Diagnostics;

namespace Elevation.Patches
{
    public static class YInterpolation
    {
        public static float GetMidpointSlopedY(Vector3 position, float slopingRadius = -1f)
        {
            float radius = slopingRadius == -1f ? ElevationManager.slopingRadius : slopingRadius;

            Cell cell = World.inst.GetCellDataClamped(position);
            if (cell == null)
                return 0f;

            if (cell.Type == ResourceType.Water)
            {
                bool floated = false;
                if (cell.OccupyingStructure.Count > 0)
                {
                    Building building = cell.OccupyingStructure[0];
                    floated = ((building.categoryHash == World.pathHash || building.uniqueNameHash == World.dockHash || building.uniqueNameHash == World.fishinghutHash) && building.IsBuilt());
                }
                if (!floated)
                {
                    return -0.3f;
                }
            }

            Vector3 difference = (position - cell.Center).xz();

            CellMeta current = Grid.Cells.Get(position);
            CellMeta other = null;

            if (other != null || current != null)
            {
                if (difference.z >= radius)
                {
                    other = Grid.Cells.Get(new Vector3(position.x, position.y, position.z + 1f));
                    return Mathf.Lerp(current != null ? current.Elevation : 0f, other != null ? other.Elevation : 0f,
                        (difference.z - (0.5f - radius)) / (2f * radius));
                }
                else if (difference.z <= -radius)
                {
                    other = Grid.Cells.Get(new Vector3(position.x, position.y, position.z - 1f));
                    return Mathf.Lerp(other != null ? other.Elevation : 0f, current != null ? current.Elevation : 0f,
                        1f - (-difference.z - (0.5f - radius)) / (2f * radius));
                }
                else if (difference.x >= radius)
                {
                    other = Grid.Cells.Get(new Vector3(position.x + 1f, position.y, position.z));
                    return Mathf.Lerp(current != null ? current.Elevation : 0f, other != null ? other.Elevation : 0f,
                        (difference.x - (0.5f - radius)) / (2f * radius));
                }
                else if (difference.x <= -radius)
                {
                    other = Grid.Cells.Get(new Vector3(position.x - 1f, position.y, position.z));
                    return Mathf.Lerp(other.Elevation, current.Elevation,
                        1f - (-difference.x - (0.5f - radius)) / (2f * radius));
                }

            }

            if (current != null)
                return current.Elevation;

            return position.y;
        }
    }

    [HarmonyPatch(typeof(Villager), "GetPosWithOffset")]
    public class VillagerYCorrectionPatch
    {
        static void Postfix(Villager __instance, ref Vector3 __result)
        {
            Cell cell = __instance.cell;
            if (cell == null)
                return;
            
            CellMeta meta = cell.GetMeta();
            if (meta != null && cell.Type != ResourceType.Water)
            {
                //if (__instance.travelPath.Count > 0 && Pathing.IsDiagonalXZ(__result, __instance.travelPath.data[0]))
                //    __result.y = GetYSloped(__result, __result, __instance.travelPath.data[0]);
                //else
                    __result.y = YInterpolation.GetMidpointSlopedY(__result);
            }
        }

        //// TODO: Test optimization of this
        //public static float GetYSloped(Vector3 position, Vector3? diagonalFrom = null, Vector3? diagonalTo = null)
        //{
            
        //}
    }
}
