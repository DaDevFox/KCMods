using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;

namespace ElevationExperiment.Patches
{
    [HarmonyPatch(typeof(CastleBlock), "PlaceDoor")]
    class CastleBlockDoorPatch
    {
        static void Prefix(CastleBlock __instance, ref Vector3 position, ref Vector3 faceDirection)
        {
            CellMark mark = ElevationManager.GetCellMark(__instance.GetComponent<Building>().GetCell());
            if (mark != null)
            {
                position.y = __instance.transform.localPosition.y - mark.Elevation;
                faceDirection.y = 0f;
            }
        }
    }


    [HarmonyPatch(typeof(CastleBlock), "UpdateBlock")]
    class CastleBlockUpdateDoorPatch
    {
        static void Postfix(CastleBlock __instance)
        {
            Cell c = __instance.GetComponent<Building>().GetCell();
            CellMark mark = ElevationManager.GetCellMark(c);
            if (mark != null)
            {
                Cell[] neighborCells = new Cell[4];

                Building b = __instance.GetComponent<Building>();
                World.inst.GetNeighborCells(c, ref neighborCells);


                int idx = -1;
                for (int n = 0; n < c.OccupyingStructure.Count; n++)
                {
                    if (c.OccupyingStructure[n] == b)
                    {
                        idx = n;
                        break;
                    }
                }

                float selfHeight = BuildingPlacePatch.GetAbsoluteStackHeightOfBuildingAtIndex(c, idx);
                DebugExt.dLog(" -- " + idx.ToString() + " -- ");
                DebugExt.dLog(selfHeight.ToString());

                typeof(CastleBlock).GetMethod("ClearDoors", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[] { });
                for (int m = 0; m < neighborCells.Length; m++)
                {
                    float otherHeight = BuildingPlacePatch.GetAbsoluteStackHeightTotal(neighborCells[m]);

                    if (otherHeight > 0f)
                        DebugExt.dLog(otherHeight.ToString());

                    Cell cell = neighborCells[m];
                    if (cell != null)
                    {
                        if (Mathf.Approximately(selfHeight - 0.5f, otherHeight) && otherHeight > 0)
                        {
                            DebugExt.dLog("Connection!");
                            typeof(CastleBlock).GetMethod("VisibleDoors", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[] { true });
                        }
                    }
                }
            }
        }
    }
}
