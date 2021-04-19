using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace Elevation.Patches
{
    [HarmonyPatch(typeof(World), "CanPlace")]
    public class PlacementValidationPatch
    {

        public static bool CurrentPlacementOnBlockedCell { get; private set; } = false;
        public static bool ItemOnTerrainInvalid { get; private set; } = false;


        static void Postfix(Building PendingObj, ref PlacementValidationResult __result)
        {
            Cell cell = World.inst.GetCellDataClamped(PendingObj.transform.position);
            if (cell == null)
                return;

            /// For [Experimental Elevation]: place building on different level depending on raycast target. 
            CellMeta meta = Grid.Cells.Get(cell);

            if (PendingObj.UnevenTerrain())
            {
                __result = PlacementValidationResult.MustBeOnFlatLand;
            }

            if (Pathing.BlockedCompletely(World.inst.GetCellData(PendingObj.transform.position)) && PendingObj.UniqueName != "outpost" && PendingObj.UniqueName != "keep")
            {
                __result = PlacementValidationResult.OutsideOfTerritory;
                CurrentPlacementOnBlockedCell = true;
            }
            else
            {
                CurrentPlacementOnBlockedCell = false;
            }

            if ((meta != null && meta.elevationTier > 0) &&
                (PendingObj.UniqueName == "aqueduct" || 
                PendingObj.UniqueName == "reservoir" || 
                PendingObj.UniqueName == "noria"))
            {
                __result = PlacementValidationResult.WellOnGround;
                ItemOnTerrainInvalid = true;
            }
            else
                ItemOnTerrainInvalid = false;
        }
    }

    [HarmonyPatch(typeof(BuildInfoUI), "Update")]
    class BuildInfoUIPatch
    {
        static void Postfix(BuildInfoUI __instance)
        {
            if (PlacementValidationPatch.ItemOnTerrainInvalid)
                __instance.ruleTextUI.text = "This building cannot be placed on elevation, sire. ";

            if (PlacementValidationPatch.CurrentPlacementOnBlockedCell)
                __instance.ruleTextUI.text = "Our builders cannot find a way to reach this tile, sire. ";

        }
    }

}
