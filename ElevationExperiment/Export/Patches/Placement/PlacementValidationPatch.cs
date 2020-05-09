using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace ElevationExperiment.Patches
{
    [HarmonyPatch(typeof(World), "CanPlace")]
    class PlacementValidationPatch
    {

        public static bool CurrentPlacementOnBlockedCell { get; private set; } = false;


        static void Postfix(Building PendingObj, ref PlacementValidationResult __result)
        {
            if (__result == PlacementValidationResult.Valid)
            {
                if (BuildingPlacePatch.UnevenTerrain(PendingObj))
                {
                    __result = PlacementValidationResult.MustBeOnFlatLand;
                }
                if (PathingManager.BlockedCompletely(World.inst.GetCellData(PendingObj.transform.position)) && PendingObj.UniqueName != "outpost" && PendingObj.UniqueName != "keep")
                {
                    __result = PlacementValidationResult.OutsideOfTerritory;
                    CurrentPlacementOnBlockedCell = true;
                }
                else
                {
                    CurrentPlacementOnBlockedCell = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BuildInfoUI), "Update")]
    class BuildInfoUIPatch
    {
        static void Postfix(BuildInfoUI __instance)
        {
            if (PlacementValidationPatch.CurrentPlacementOnBlockedCell)
                __instance.ruleTextUI.text = "Our builders cannot find a way to reach this tile, sire. ";
        }
    }

}
