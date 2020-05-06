using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

namespace ElevationExperiment.Patches
{
    class RoadVisualPatch
    {
        public enum RoadType
        {
            normal,
            stone
        }

        struct RoadData
        {
            public Direction[] connected;
            public Direction[] elevated;
            public RoadType type;
        }






        //static void GetDirectionBetweenCells(Cell from, Cell to)
        //{
        //    //Dictionary<Vector3, Direction> directions = new Dictionary<Vector3, Direction>()
        //    //{
        //    //    { new Vector3(1f, 0f, 0f), Direction.East },
        //    //    { Direction.South, new Vector3(0f, 0f, 1f) },
        //    //    { Direction.West, new Vector3(-1f, 0f, 0f) },
        //    //    { Direction.North, new Vector3(0f, 0f, -1f) },
        //    //};
        //}


        //public static void UpdateRoad(Road road)
        //{
        //    Cell cell = road.GetComponent<Building>().GetCell();
        //    CellMark mark = ElevationManager.GetCellMark(cell);

        //    if (cell == null || mark == null)
        //        return;

        //    Cell[] neighbors = World.inst.GetNeighborCells(cell);
        //    List<Direction> dirs = new List<Direction>();

        //    foreach(Cell neighborCell in neighbors)
        //    {
        //        CellMark neighborMark = ElevationManager.GetCellMark(neighborCell);
        //        if(neighborMark.elevationTier - mark.elevationTier == 1)
        //        {
        //            //dirs.Add()
        //        }

        //    }
        //}




    }


    //[HarmonyPatch(typeof(Road),"ShouldConnect")]
    public class RoadShouldConnectPatch
    {
        static void Postfix(Road __instance, ref bool __result, Cell c)
        {
            try
            {
                if (__instance != null)
                {
                    Mod.helper.Log("test");

                    Cell roadCell = World.inst.GetCellData(__instance.transform.position);
                    if (roadCell != null && c != null)
                    {
                        CellMark markFrom = ElevationManager.GetCellMark(roadCell);
                        CellMark markTo = ElevationManager.GetCellMark(c);
                        if (markFrom != null && markTo != null)
                        {
                            if (ElevationManager.ValidTileForElevation(roadCell) && ElevationManager.ValidTileForElevation(c))
                            {
                                if (!(markFrom.elevationTier - markTo.elevationTier == 1 || markFrom.elevationTier - markTo.elevationTier == 0))
                                {
                                    __result = false;
                                }
                            }
                        }
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
