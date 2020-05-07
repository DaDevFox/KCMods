using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace ElevationExperiment.Patches
{
    [HarmonyPatch(typeof(WolfDen), "Tick")]
    class WolfPatch
    {
        static void Postfix(WolfDen __instance)
        {
            if (__instance.wolfData != null)
            {
                foreach (WolfDen.WolfData wolf in __instance.wolfData.data)
                {
                    //if (wolf.origPos != null)
                    //{
                    //    Cell cell = World.inst.GetCellDataClamped(wolf.origPos);
                    //    if (cell != null)
                    //    {
                    //        CellMark mark = ElevationManager.GetCellMark(cell);
                    //        if (mark != null)
                    //        {
                    //            //wolf.pos.y = mark.Elevation;
                    //        }
                    //    }
                    //}
                }
            }
        }


    }
}
