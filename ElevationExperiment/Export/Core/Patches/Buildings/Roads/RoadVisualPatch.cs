using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

namespace Elevation.Patches
{
    [HarmonyPatch(typeof(Road), "UpdateRotationForRoad")]
    public class RoadVisualPatch
    {
        public static float StairCentralOffset { get; } = -0.5f;

        internal static Dictionary<Guid, RoadData> _data = new Dictionary<Guid,RoadData>();

        public enum RoadType
        {
            normal,
            stone
        }

        public struct RoadData
        {
            public bool valid;

            public Cell[] neighbors;
            public Cell cell;
            public Transform stairContainer;
            public Dictionary<string, Transform> stairs;
            public RoadType type;
        }


        static void Postfix(Road __instance)
        {
            //NOTE: Road Visuals disabled

            //Guid guid = __instance.GetComponent<Building>().guid;

            //if (!_data.ContainsKey(guid))
            //{
            //    RoadData rData = CreateRoadDataFor(__instance);
            //    if (rData.valid)
            //        _data.Add(guid, rData);
            //    else
            //        return;
            //}

            //RoadData data = _data[guid];

            //if (!data.valid)
            //    return;


            //List<Cell> elevated = GetElevatedNeighbors(data.cell);

            //// Add stairs 
            //foreach (Cell cell in elevated)
            //{
            //    if (cell == null)
            //        continue;

            //    DebugExt.dLog($"Connected: {cell.x}_{cell.z}");

            //    string id = CellMetadata.GetPositionalID(cell);

            //    if (data.stairs.ContainsKey(id))
            //        continue;
                    
            //    Vector3 dir = (data.cell.Center - cell.Center).xz();
                
            //    GameObject stair = CreateStairInstanceFor(data, dir, id);
            //    DebugExt.dLog($"[{dir}] stair instance at {data.cell.Center} : {stair.transform.position}", false, stair.transform.position);
                
            //}

            //// Remove redundancies
            //List<string> redundancies = new List<string>();

            //foreach (string id in data.stairs.Keys)
            //{
            //    if (elevated.Any((c) => c != null && CellMetadata.GetPositionalID(c) == id))
            //        continue;

            //    redundancies.Add(id);
            //}

            //foreach (string id in redundancies)
            //{
            //    GameObject.Destroy(data.stairs[id].gameObject);
            //    data.stairs.Remove(id);
            //}
        }

        private static List<Cell> GetElevatedNeighbors(Cell cell)
        {
            Cell[] cells = World.inst.GetNeighborCells(cell);
            List<Cell> found = new List<Cell>();

            if (!Grid.Cells.Get(cell))
                return found;

            found.AddRange(
                cells.Where(
                    (c) =>
                    {
                        if(c != null)
                            if(Road.ShouldConnect(c))
                                if (Grid.Cells.Get(c))
                                    if (Grid.Cells.Get(c).elevationTier - Grid.Cells.Get(cell).elevationTier == 1)
                                        return true;

                        return false;
                    }));

            return found;
        }



        private static GameObject CreateStairInstanceFor(RoadData data, Vector3 dir, string id)
        {
            GameObject obj = null;
            if (data.type == RoadType.normal)
                obj = GameObject.Instantiate(RoadAssets.stairs_normal);
            else if (data.type == RoadType.stone)
                obj = GameObject.Instantiate(RoadAssets.stairs_stone);
            else
                Mod.Log("error; trying to create stair for road type that doesn't support elevation");
            

            obj.transform.position = data.cell.Center.xz() + (dir * StairCentralOffset);

            //obj.transform.position = new Vector3(obj.transform.position.x, road.cell.Center.y, obj.transform.position.z);

            obj.transform.SetParent(data.stairContainer);
            obj.transform.LookAt(obj.transform.position + dir, Vector3.up);
            //obj.transform.rotation.SetLookRotation(dirVector, Vector3.up);

            data.stairs.Add(id, obj.transform);

            return obj;
        }

        private static void Update(RoadData data)
        {
            if (!data.valid)
                return;

            if (data.cell == null)
                return;


        }

        private static RoadData CreateRoadDataFor(Road road)
        {
            RoadData data = new RoadData()
            {
                valid = false,
                stairs = new Dictionary<string, Transform>(),
                type = road.GetComponent<Building>().UniqueName == "stoneroad" ? RoadType.stone : RoadType.normal,
            };

            if(road.isBridge)
                return data;

            Cell cell = World.inst.GetCellDataClamped(road.transform.position);
            if (cell == null)
                return data;

            data.cell = cell;

            CellMeta meta = Grid.Cells.Get(cell);
            if (meta == null)
                return data;

            data.valid = true;
            
            GameObject container = new GameObject("stairContainer");
            container.transform.SetParent(road.transform);

            data.stairContainer = container.transform;

            return data;
        }
    }


    [HarmonyPatch(typeof(Road), "GetAdjacencyInfo")]
    public class RoadConnectPatch
    {
        static bool Prefix(Road __instance, ref bool north, ref bool south, ref bool east, ref bool west, ref int count)
        {
            try
            {
                World.inst.ToGridCoord(__instance.transform.position, out int num, out int num2);
                north = Road.ShouldConnect(World.inst.GetCellDataClamped(num + 1, num2));
                south = Road.ShouldConnect(World.inst.GetCellDataClamped(num - 1, num2));
                east = Road.ShouldConnect(World.inst.GetCellDataClamped(num, num2 + 1));
                west = Road.ShouldConnect(World.inst.GetCellDataClamped(num, num2 - 1));

                CellMeta roadMeta = Grid.Cells.Get(World.inst.GetCellDataClamped(__instance.transform.position));

                CellMeta mNorth = Grid.Cells.Get(World.inst.GetCellDataClamped(num + 1, num2));
                CellMeta mSouth = Grid.Cells.Get(World.inst.GetCellDataClamped(num - 1, num2));
                CellMeta mEast = Grid.Cells.Get(World.inst.GetCellDataClamped(num , num2 + 1));
                CellMeta mWest = Grid.Cells.Get(World.inst.GetCellDataClamped(num, num2 - 1));

                if(roadMeta != null)
                {
                    if (mNorth != null)
                        north &= Math.Abs(roadMeta.elevationTier - mNorth.elevationTier) <= 1;
                    if (mSouth != null)
                        south &= Math.Abs(roadMeta.elevationTier - mSouth.elevationTier) <= 1;
                    if (mEast != null)
                        east &= Math.Abs(roadMeta.elevationTier - mEast.elevationTier) <= 1;
                    if (mWest != null)
                        west &= Math.Abs(roadMeta.elevationTier - mWest.elevationTier) <= 1;
                }

                if (north)
                {
                    count++;
                }
                if (south)
                {
                    count++;
                }
                if (east)
                {
                    count++;
                }
                if (west)
                {
                    count++;
                }
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Road), "BuildingAddRemove")]
    public class RoadDestroyPatch
    {
        static void Postfix(OnBuildingAddRemove obj)
        {
            if(!obj.added && obj.targetBuilding.GetComponent<Road>())
            {
                Road r = obj.targetBuilding.GetComponent<Road>();

                if (!r.isBridge)
                {
                    if (RoadVisualPatch._data.ContainsKey(obj.targetBuilding.guid))
                    {
                        RoadVisualPatch._data.Remove(obj.targetBuilding.guid);
                    }
                }
            }
        }
    }
    

}
