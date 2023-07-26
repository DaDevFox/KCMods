using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace Elevation.Patches
{
    //[HarmonyPatch(typeof(TransportCart), "MoveUpdate")]
    //public class TransportCartPatch
    //{
    //    static void Postfix(TransportCart __instance, float timestep, Vector3 targetPos)
    //    {
    //        float yAdjust = 0f;
    //        Cell cell = World.inst.GetCellDataClamped(targetPos);
    //        if (cell.OccupyingStructure.Count > 0)
    //        {
    //            Building building = cell.OccupyingStructure[0];
    //            if (building.IsBuilt())
    //            {
    //                if (building.uniqueNameHash == World.dockHash)
    //                {
    //                    yAdjust = 0.06f;
    //                }else if (building.categoryHash == World.pathHash)
    //                {
    //                    yAdjust = 0.1f;
    //                }
    //            }
    //        }

    //        Vector3 displacement = __instance.transform.position;
    //        CellMeta meta = Grid.Cells.Get(targetPos);
    //        if (meta != null)
    //        {
    //             __instance.transform.position = new Vector3(displacement.x, yAdjust + meta.Elevation, displacement.z);
    //        }
    //    }

    //    static void Finalizer(Exception ex)
    //    {
    //        Mod.dLog(ex);
    //    }
    //}
}
