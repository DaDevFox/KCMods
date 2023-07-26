using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Elevation.Patches;
using UnityEngine;
using Harmony;
using Fox.Profiling;

namespace Elevation
{
    public static class WorldRegions
    {
        public static event Action onMarked;

        #region Async Settings

        public static bool async { get; set; } = true;
        public static bool secondsDistribution { get; set; } = false;

        public static float markingFPS = 15f;
        public static float timePerFrame { get; set; } = 0.0666667f;

        public static float minProcessingTime = 0.2f;
        public static float minMarkingTime = 0.2f;
        public static float minFinalizingTime = 0.2f;

        private static Stopwatch timer;

        private static int cache = 0;

        #endregion

        /// <summary>
        /// Returns whether the world has had its regions marked already
        /// </summary>
        public static bool Marked { get; private set; } = false;

        /// <summary>
        /// Returns whether the region categorization algorithm is currently busy with an operation
        /// </summary>
        public static bool Busy { get; private set; } = false;

        public static List<Cell> Dirty { get; private set; } = new List<Cell>();

        public static List<Cell> Unreachable { get; private set; } = new List<Cell>();
        public static List<Cell> BeginSearchPositions { get; private set; } = new List<Cell>();

        private static Dictionary<string, CellData> cellsData = new Dictionary<string, CellData>();
        private static Dictionary<int, List<CellData>> regionData = new Dictionary<int, List<CellData>>();

        private static List<CellData> openSet = new List<CellData>();

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
                    ArrayExt<Vector3> path = new ArrayExt<Vector3>(World.inst.GridHeight*World.inst.GridWidth);
                    World.inst.FindFootPath(startingPos.Center, c.Center, ref path);
                    if (path.Count <= 2)
                        Unreachable.Add(c);
                    Dirty.RemoveAt(i);
                }
            }
        }

        public static void Search()
        {
            if (async)
                GameObject.FindObjectOfType<Mod>().StartCoroutine(RegionSearchAsync());
            else
                RegionSearch();
        }

        #region Single Frame

        // TODO: Async region search; pause game at beginning, show loading dialog, and asynchronously execute the region search
        [Profile]
        public static void RegionSearch()
        {
            Marked = false;

            regionData.Clear();
            cellsData.Clear();

            List<CellData> remaining = new List<CellData>();

            // Preperation
            // Mark all cells that support elevation as nodes
            foreach (CellMeta meta in Grid.Cells)
            {
                CellData nodeData = new CellData()
                {
                    cell = meta.cell,
                    meta = meta,
                    region = -1
                };
                    
                cellsData.Add(CellMetadata.GetPositionalID(meta.cell), nodeData);
                remaining.Add(nodeData);
            }

            int region = 1;

            // Iterate on all nodes, each node will be given a region in which it resides. 
            // Any node can reach another node in the same region, but anywhere else is unreachable 
            while(remaining.Count > 0)
            {
                CellData node = remaining[0];

                node.region = region;
                IterateNode(node);
                region++;
            }

            ReformatRegions();

            Busy = false;
            MarkComplete();
            
            Mod.Log("Blocked Regions Pruned");
        }

        private static void IterateNode(CellData node)
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

                if (!Pathing.Connected(node.cell, other.cell) && !Pathing.BlocksForBuilding(other.cell))
                {
                    TagSameRegion(node, other);
                    IterateNode(other);
                }
            }
        }

        #endregion

        #region Async

        public static IEnumerator RegionSearchAsync()
        {
            UI.loadingDialog.Activate();
            UI.loadingDialog.title = "pruning_title";

            Marked = false;
            Busy = true;

            regionData.Clear();
            cellsData.Clear();

            openSet.Clear();

            float totalElapsed = 0f;

            float timeBreak = 0.1f;
            float elapsed = 0f;
            int count = 0;

            timer = new Stopwatch();

            UI.loadingDialog.description = "pruning_preprocessing";
            UI.loadingDialog.UpdateText();
            UI.loadingDialog.desiredProgress = 0.5f;

            // Preperation
            // Mark all cells that support elevation as nodes
            foreach (CellMeta meta in Grid.Cells)
            {
                timer.Restart();
                
                CellData nodeData = new CellData()
                {
                    cell = meta.cell,
                    meta = meta,
                    region = -1
                };

                cellsData.Add(CellMetadata.GetPositionalID(meta.cell), nodeData);
                openSet.Add(nodeData);

                timer.Stop();

                elapsed += ((float)timer.Elapsed.Milliseconds) / 1000f;
                totalElapsed += ((float)timer.Elapsed.Milliseconds) / 1000f;
                count++;

                if(elapsed > timeBreak)
                {
                    elapsed = 0;
                    //UI.loadingDialog.desiredProgress = ((float)count) / Grid.Cells.Count;
                    yield return new WaitForEndOfFrame();
                }
            }

            // wait for the min time to complete if not complete already 
            while (totalElapsed < minProcessingTime)
            {
                yield return new WaitForEndOfFrame();
                totalElapsed += Time.unscaledDeltaTime;
            }


            UI.loadingDialog.description = "pruning_floodfill";
            UI.loadingDialog.desiredProgress = 0f;
            UI.loadingDialog.UpdateText();
            totalElapsed = 0f;
            elapsed = 0f;
            yield return new WaitForEndOfFrame();



            int region = 1;
            Stopwatch outerLoopTimer = new Stopwatch();
            outerLoopTimer.Start();


            // Iterate on all nodes, each node will be given a region in which it resides. 
            // Any node can reach another node in the same region, but anywhere else is unreachable 
            while (openSet.Count > 0)
            {
                timer.Restart();

                CellData node = openSet[0];
                node.region = region;

                if (node.cell == null || !node.hasCardinals)
                {
                    openSet.Remove(node);
                    timer.Stop();

                    Mod.dLog("skip");
                    continue;
                }

                yield return IterateNodeAsync(node, 1);
                region++;

                timer.Stop();

                Mod.dLog($"tick: {cache}");

                elapsed += (float)timer.Elapsed.TotalSeconds;
                UI.loadingDialog.desiredProgress = 1f - ((float)openSet.Count) / ((float)cellsData.Count);

                //if (elapsed > timePerFrame)
                //{
                //    UI.loadingDialog.desiredProgress = ((float)openSet.Count) / ((float)cellsData.Count);
                //    Mod.dLog(openSet.Count);
                //    yield return new WaitForEndOfFrame();
                //    elapsed = 0f;
                //}
            }


            timer.Stop();

            outerLoopTimer.Stop();
            totalElapsed = ((float)outerLoopTimer.Elapsed.Milliseconds) / 1000f;

            // wait for the min time to complete if not complete already 
            while (totalElapsed < minMarkingTime)
            {
                yield return new WaitForEndOfFrame();
                totalElapsed += Time.unscaledDeltaTime;
            }

            UI.loadingDialog.description = "pruning_reformat";
            UI.loadingDialog.desiredProgress = 0f;
            UI.loadingDialog.UpdateText();
            totalElapsed = 0f;
            yield return new WaitForEndOfFrame();

            ReformatRegions();

            UI.loadingDialog.desiredProgress = 1f;

            // wait for the min time to complete if not complete already 
            while (totalElapsed < minMarkingTime)
            {
                yield return new WaitForEndOfFrame();
                totalElapsed += Time.unscaledDeltaTime;
            }

            UI.loadingDialog.Deactivate();
            Busy = false;
            MarkComplete();
            Mod.Log("Blocked Regions Pruned [async]");
        }


        private static IEnumerator IterateNodeAsync(CellData node, int stack = 0)
        {
            cache = stack;
            openSet.Remove(node);

            //Mod.dLog((float)timer.Elapsed.TotalSeconds);

            if ((float)timer.Elapsed.TotalSeconds > timePerFrame)
            {
                Mod.dLog("inner tick");
                UI.loadingDialog.desiredProgress = 1f - ((float)openSet.Count) / ((float)cellsData.Count);
                timer.Restart();

                yield return new WaitForEndOfFrame();
            }

            if (node.cell != null)
            {
                foreach (Direction dir in CellData.directions)
                {
                    CellData other = node.GetCardinal(dir);
                    if (other == null)
                        continue;

                    if (CheckSameRegion(node, other))
                        continue;


                    if (Pathing.Connected(node.cell, other.cell) && !Pathing.BlocksForBuilding(other.cell))
                    {
                        TagSameRegion(node, other);
                        IterateNodeAsync(other, stack + 1);
                    }
                }
            }
        }

        #endregion

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
            onMarked?.Invoke();
        }

        public class CellData
        {
            public static Direction[] directions = { Direction.East, Direction.North, Direction.West, Direction.South };

            public Cell cell { get; set; }
            public CellMeta meta;
            public int region;
            public bool empty;

            public bool hasCardinals
            {
                get
                {
                    foreach (Direction dir in directions)
                    {
                        Cell cardinal = Pathing.GetCardinal(cell, dir);
                        if (Pathing.Connected(cell, cardinal) && !CheckSameRegion(this, cellsData[CellMetadata.GetPositionalID(cardinal)]) && !Pathing.BlocksForBuilding(cardinal))
                            return true;
                    }
                    return false;
                }
            }

            public CellData[] GetCardinals()
            {
                List<CellData> cardinals = new List<CellData>();
                foreach (Direction dir in directions)
                {
                    CellData found =GetCardinal(dir);
                    if (found != null)
                        cardinals.Add(found);
                }
                return cardinals.ToArray();
            }

            public CellData GetCardinal(Direction direction)
            {
                if (cellsData == null)
                {
                    Mod.dLog("cellsData null");
                    return null;
                }
                if (cell == null)
                {
                    Mod.dLog("cell null");
                    return null;
                }

                Cell cardinal = Pathing.GetCardinal(cell, direction);

                if (cardinal != null)
                {
                    string id = CellMetadata.GetPositionalID(cardinal);
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (cellsData.ContainsKey(id))
                            return cellsData[id];
                    }
                }

                return null;
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
