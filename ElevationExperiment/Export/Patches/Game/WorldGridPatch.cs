using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace ElevationExperiment.Patches
{
    [HarmonyPatch(typeof(World),"SnapToGrid2D")]
    class WorldGridPatch
    {
        static void Postfix(ref Vector3 __result)
        {
            Cell cell = World.inst.GetCellDataClamped(__result);
            if(cell != null)
            {
                CellMark mark = ElevationManager.GetCellMark(cell);
                if (mark != null)
                    __result.y = mark.Elevation;
            }
        }
    }
}
