using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elevation.Patches;
using UnityEngine;
using Harmony;
using Fox.Profiling;

namespace Elevation
{
    public static class WorldRegions
    {
        public static bool Marked { get; private set; } = false;

        public static List<Cell> Dirty { get; private set; } = new List<Cell>();

        public static List<Cell> Unreachable { get; private set; } = new List<Cell>();
        public static List<Cell> BeginSearchPositions { get; private set; } = new List<Cell>();

        private static Dictionary<string, CellData> cellsData = new Dictionary<string, CellData>();
        private static Dictionary<int, List<CellData>> regionData = new Dictionary<int, List<CellData>>();

        public static int GetTileRegion(Cell cell)
        {
            if(cell != null)
                if (cellsData.ContainsKey(CellMetadata.GetPositionalID(cell)))
                    return cellsData[CellMetadata.GetPositionalID(cell)].region;
            return -1;
        }


        public static void MarkDirty(Cell cell)
        {
            Dirty.Add(cell);
        }

        public static void UpdateDirty()
        {
            for(int i = 0; i < Dirty.Count; i++) 
            {
                Cell c = Dirty[i];

                foreach(Cell startingPos in BeginSearchPositions)
                {
                    List<Vector3> path = new List<Vector3>();
                    World.inst.FindFootPath(startingPos.Center, c.Center, ref path);
                    if (path.Count <= 2)
                        Unreachable.Add(c);
                    Dirty.RemoveAt(i);
                }
            }
        }



        [Profile]
        public static void DoRegionSearch()
        {
            Marked = false;
            regionData.Clear();
            cellsData.Clear();

            List<CellData> remaining = new List<CellData>();

            // Preperation
            // Mark all cells that support elevation as nodes
            foreach (CellMeta node in Grid.Cells)
            {
                CellData nodeData = new CellData()
                {
                    cell = node.cell,
                    meta = node,
                    region = -1
                };
                    
                cellsData.Add(CellMetadata.GetPositionalID(node.cell), nodeData);
                remaining.Add(nodeData);
            }

            int region = 1;

            // Iterate on all nodes, each node will be given a region in which it resides. 
            // Any node can reach another node in the same region, but anywhere else is unreachable 
            while(remaining.Count > 0)
            {
                CellData node = remaining[0];

                node.region = region;
                IterateNode(node, ref remaining);
                region++;
            }

            ReformatRegions();
            MarkComplete();
            Mod.Log("Blocked Regions Pruned");
        }

        private static void IterateNode(CellData node, ref List<CellData> openSet)
        {
            openSet.Remove(node);

            List<Direction> dirs = new List<Direction>() { Direction.East, Direction.North, Direction.West, Direction.South };
            foreach (Direction dir in dirs)
            {
                
                CellData other = node.GetCardinal(dir);
                if (openSet.Contains(node))
                    continue;

                if (CheckSameRegion(node, other))
                    continue;

                if (!PrebakedPathfinder.Connected(node.cell, other.cell) && !Pathing.BlocksForBuilding(other.cell))
                {
                    TagSameRegion(node, other);
                    IterateNode(other, ref openSet);
                }
            }
        }

        private static bool CheckSameRegion(CellData a, CellData b)
        {
            return a.region == b.region;
        }

        private static void TagSameRegion(CellData a, CellData b)
        {
            if (a.region != -1)
            {
                b.region = a.region;
            }
            else
            {
                a.region = regionData.Keys.Count;
                b.region = a.region;
                regionData.Add(a.region, new List<CellData>() { a, b });
            }
            
        }

        private static void ReformatRegions()
        {
            regionData.Clear();
            foreach(CellData node in cellsData.Values)
            {
                if (regionData.ContainsKey(node.region) && node.region != -1)
                {
                    regionData[node.region].Add(node);
                }
                else
                {
                    if(node.region != -1)
                        regionData.Add(node.region, new List<CellData>() { node });
                }
            }
        }

        private static void MarkComplete()
        {
            WorldRegions.Marked = true;
        }

        public class CellData
        {
            public Cell cell;
            public CellMeta meta;
            public int region;
            public bool empty;

            public CellData GetCardinal(Direction direction)
            {
                Dictionary<Direction, Vector3> dirs = new Dictionary<Direction, Vector3>()
                {
                    { Direction.East,  new Vector3(1f, 0f, 0f) },
                    { Direction.South, new Vector3(0f, 0f, 1f) },
                    { Direction.West, new Vector3(-1f, 0f, 0f)},
                    { Direction.North, new Vector3(0f, 0f, -1f)},
                };

                if (dirs.ContainsKey(direction))
                {
                    Cell cardinal = World.inst.GetCellData(cell.Center + dirs[direction]);
                    if (cardinal != null)
                    {
                        string id = CellMetadata.GetPositionalID(cardinal);
                        if (!string.IsNullOrEmpty(id))
                        {
                            if (cellsData.ContainsKey(id))
                                return cellsData[id];
                        }
                    }
                }
                return new CellData()
                {
                    empty = true
                };
            }
        }

        /// <summary>
        /// Returns wehther one tile is in the same world region as another and therefore whether a path will be able to be found from one to the other
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool Reachable(Cell from, Cell to)
        {
            return GetTileRegion(from) == GetTileRegion(to);
        }





    }
}
