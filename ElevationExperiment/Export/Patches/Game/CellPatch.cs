using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace ElevationExperiment.Patches
{
    
    [HarmonyPatch(typeof(Cell))]
    [HarmonyPatch("Center", PropertyMethod.Getter)]
    public class CellCenterPatch
    {
        static void Postfix(Cell __instance, ref Vector3 __result)
        {
            try
            {
                CellMark mark = ElevationManager.GetCellMark(__instance);
                if (mark != null)
                {
                    __result = new Vector3((float)__instance.x + 0.5f, mark.Elevation, (float)__instance.z + 0.5f);
                }
            }
            catch (Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }
    }
    
}
