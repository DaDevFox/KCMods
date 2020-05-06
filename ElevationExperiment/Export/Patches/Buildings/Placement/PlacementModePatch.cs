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
    public class PlacementModePatch
    {
        [HarmonyPatch(typeof(PlacementMode), "UpdateBuildingAtPosition")]
        class UpdateBuildingAtPositionPatch
        {
            static void Postfix(Building b, Vector3 newPos)
            {
                DebugExt.dLog("patch");
                Cell cell = World.inst.GetCellData(b.transform.position);
                CellMark mark = ElevationManager.GetCellMark(cell);
                float leveling = mark.Elevation;
                if (cell != null && mark != null)
                {
                    DebugExt.dLog(leveling.ToString());
                    b.transform.position = new Vector3(b.transform.position.x, b.transform.position.y + leveling, b.transform.position.z);
                }
            }
        }

        [HarmonyPatch(typeof(PlacementMode), "Update")]
        class PlacementModeUpdatePatch
        {
            static void Postfix(PlacementMode __instance)
            {
                Mod.helper.Log("test");
                if (__instance.IsPlacing()) {
                    Mod.dLog("placing");
                    bool dragging = (bool)typeof(PlacementMode)
                        .GetField("attemptedDrag", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(__instance);
                    if (!dragging)
                    {
                        Building b = __instance.GetHoverBuilding();
                        Cell cell = World.inst.GetCellData(b.transform.position);
                        if(cell != null)
                        {
                            Cell other = FindClosestGroundTile(cell);
                            if (other != null)
                                DebugExt.dLog("found close ground-level tile", false, other.Center);
                        }
                    }
                }
            }

            static Cell FindClosestGroundTile(Cell cell)
            {
                return World.inst.FindBestSuitedCell(cell, true, 30, 
                    (_cell) =>
                    {
                        CellMark mark = ElevationManager.GetCellMark(_cell);
                        if(mark != null)
                            if(mark.elevationTier == 0)
                                return Vector3.Distance(cell.Center.xz(), _cell.Center.xz());
                        return 0f;
                    });
            }

        }

    }
}
