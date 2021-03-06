﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevationExperiment.Patches;
using UnityEngine;

namespace ElevationExperiment
{
    public static class BlockedTilePruner
    {
        public static bool Pruned { get; private set; } = false;

        public static List<Cell> Unreachable { get; private set; } = new List<Cell>();

        private static Dictionary<string, CellData> cellsData = new Dictionary<string, CellData>();
        private static Dictionary<int, List<CellData>> regionData = new Dictionary<int, List<CellData>>();

        public static int GetTileRegion(Cell cell)
        {
            if (cellsData.ContainsKey(ElevationManager.GetCellMarkID(cell)))
            {
                return cellsData[ElevationManager.GetCellMarkID(cell)].region;
            }
            return -1;
        }


        public static void DoRegionSearch(List<CellMark> data)
        {
            Pruned = false;
            regionData.Clear();
            cellsData.Clear();

            List<CellMark> groundLevel = new List<CellMark>();
            for (int landmass = 0; landmass < World.inst.NumLandMasses; landmass++)
            {
                foreach(Cell cell in World.inst.cellsToLandmass[landmass].data)
                {
                    if (cell != null)
                    {
                        CellMark mark = ElevationManager.GetCellMark(cell);
                        if(mark != null)
                            if(mark.elevationTier == 0)
                                groundLevel.Add(mark);
                    }
                }
            }


            // Preperation
            foreach (CellMark node in data)
            {
                CellData nodeData = new CellData()
                {
                    cell = node.cell,
                    mark = node,
                    region = -1
                };

                cellsData.Add(ElevationManager.GetCellMarkID(node.cell), nodeData);
            }

            

            // First Pass: Assigning all ground-level tiles their own region
            regionData.Add(0, new List<CellData>());
            foreach(CellData node in cellsData.Values)
            {
                if(node.mark.elevationTier == 0)
                {
                    node.region = 0;
                }
            }

            // Second Pass: Iterate on all ground-level nodes. 
            foreach (CellData node in cellsData.Values)
            {
                IterateNode(node);
            }

            ReformatRegions();
            MarkPruned();
            Mod.Log("Blocked Regions Pruned");
        }

        private static void IterateNode(CellData node)
        {
            List<Direction> dirs = new List<Direction>() { Direction.West, Direction.North, Direction.East, Direction.South };
            foreach (Direction dir in dirs)
            {
                if (!node.GetCardinal(dir).empty)
                {
                    CellData other = node.GetCardinal(dir);
                    if (!CheckSameRegion(node, other))
                    {
                        if (!PathfindingBlockerCheckPatch.BlocksPathDirectional(node.cell, other.cell) && !PathingManager.BlocksForBuilding(other.cell))
                        {
                            TagSameRegion(node, other);
                            IterateNode(other);
                        }
                    }
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

        private static void MarkPruned()
        {
            Unreachable.Clear();
            foreach (CellData node in cellsData.Values)
            {
                if (node.region == -1)
                {
                    BlockedTilePruner.Unreachable.Add(node.cell);
                }
            }
            Mod.dLog(Unreachable.Count.ToString());
            BlockedTilePruner.Pruned = true;
        }

        class CellData
        {
            public Cell cell;
            public CellMark mark;
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
                        string id = ElevationManager.GetCellMarkID(cardinal);
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
    }
}
