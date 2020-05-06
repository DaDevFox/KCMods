using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace ElevationExperiment.Patches
{


    //Patch stack height to match elevation
    //[HarmonyPatch(typeof(Cell), "CurrentStackHeight")]
    public class StackHeightPatch
    {
        static void Postfix(Cell __instance, ref int __result)
        {
            if (__instance != null && __instance.TopStructure != null)
            {
                if (__instance.TopStructure.Stackable)
                {
                    CellMark mark = ElevationManager.GetCellMark(__instance);
                    if (mark != null)
                    {
                        __result += mark.elevationTier * __instance.TopStructure.StackHeight;
                    }
                }
            }
        }
    }

    //Patch method to find building at stack height
    //[HarmonyPatch(typeof(CastleBlock),"GetBuildingAtStackHeight")]
    public class CastleBlockRelativityPatch
    {
        static bool Prefix(Cell c, ref int stackHeight)
        {
            CellMark mark = ElevationManager.GetCellMark(c);
            if(mark.elevationTier > 0)
            {
                stackHeight -= mark.elevationTier * 2;
            }
            return true;
        }
    }







}
