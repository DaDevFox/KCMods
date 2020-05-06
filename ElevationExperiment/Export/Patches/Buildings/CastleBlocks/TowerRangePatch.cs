using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace ElevationExperiment.Patches
{
    //[HarmonyPatch(typeof(ProjectileDefense),"GetHeight")]
    class ProjectileDefenseRangePatch
    {
        static void Postfix(ProjectileDefense __instance, ref int __result)
        {
            try
            {
                Cell cell = World.inst.GetCellData(__instance.transform.position);
                if (cell != null)
                {
                    CellMark mark = ElevationManager.GetCellMark(cell);
                    if (mark != null)
                    {
                        __result += mark.elevationTier;
                    }
                }
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }
    }

    //[HarmonyPatch(typeof(ArcherTower), "GetHeight")]
    class ArcherTowerRangePatch
    {
        static void Postfix(ArcherTower __instance, ref int __result, int maxHeight)
        {
            try
            {
                Cell cell = World.inst.GetCellData(__instance.transform.position);
                if (cell != null)
                {
                    CellMark mark = ElevationManager.GetCellMark(cell);
                    if (mark != null)
                    {
                        __result = Mathff.Clamp(__result + mark.elevationTier,0,maxHeight);
                    }
                }
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
}
    }

}
