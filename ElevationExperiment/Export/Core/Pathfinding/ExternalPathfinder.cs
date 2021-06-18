using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CHusse.Pathfinding;

namespace Elevation
{
    public class ExternalPathfinder : ElevationPathfinder
    {
        public static int width {get; private set; }
        public static int height {get; private set; }

        public static SpatialAStar<Node, Cell> aStar;

        public static bool allowingDiagonals = false;

        public override void Init(int width, int height)
        {
            ExternalPathfinder.width = width;
            ExternalPathfinder.height = height;

            Node[,] grid = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    grid[x, z] = new Node(World.inst.GetCellDataClamped(x, z));

            aStar = new SpatialAStar<Node, Cell>(grid);
        }

        public override void Path(Vector3 startPos, bool upperGridStart, Vector3 endPos, bool upperGridEnd, ref List<Vector3> path, Pathfinder.blocksPathTest blocksPath, Pathfinder.blocksPathTest pull, Pathfinder.applyExtraCost extraCost, int team, bool doDiagonal, bool doTrimming, bool allowIntergridTravel)
        {
            allowingDiagonals = doDiagonal;
            //try
            //{

            //    // Reformat end position to be within same region as start cell
            Cell startCell = World.inst.GetCellDataClamped(startPos);
            Cell endCell = World.inst.GetCellDataClamped(endPos);
            if (
                //!WorldRegions.Reachable(startCell, endCell) || 
                blocksPath(endCell, team))
            {
                endCell = World.inst.FindMatchingSurroundingCell(World.inst.GetCellDataClamped(endPos), false, 2, c =>
                {
                    return !(blocksPath(c, team)
                    //&& WorldRegions.Reachable(startCell, c)
                    );
                });
            }


            //    Func<Vector3, int, IEnumerable<Vector3>> expander = (position, lv) =>
            //    {
            //        CellMeta meta = Grid.Cells.Get((int)position.x, (int)position.z);
            //        if (meta == null)
            //            return new List<Vector3>();
            //        CellMeta[] neighbors = doDiagonal ? meta.neighborsPlusFast : meta.neighborsFast;
            //        List<Vector3> result = new List<Vector3>();
            //        for (int i = 0; i < neighbors.Length; i++)
            //            if (Math.Abs(neighbors[i].elevationTier - meta.elevationTier) <= PrebakedPathfinder.ElevationClimbThreshold && !blocksPath(neighbors[i].cell, team))
            //                result.Add(neighbors[i].Center);
            //        return result;
            //    };

            //    var queryable = HeuristicSearch.AStar(startPos, endPos, expander);
            //    path = queryable.ToList();

            //}catch(Exception ex)
            //{
            //    DebugExt.HandleException(ex);
            //}

            LinkedList<Node> pathNodes = aStar.Search(new Node(startCell), new Node(endCell), startCell);

            if(path.Count > 0)
                path.Clear();
            foreach (Node node in pathNodes)
                path.Add(node.cell.Center);

            //DebugExt.dLog($"path {startPos}{(upperGridStart ? "u" : "l")} to {endPos}{(upperGridEnd ? "u" : "l")} size {path.Count}");
        }

        public class Node : IPathNode<Cell>
        {
            public int X => cell.x;
            public int Y => cell.z;

            public Cell cell;

            public Node(Cell cell)
            {
                this.cell = cell;
            }

            public bool IsWalkable(Cell other)
            {
                CellMeta meta = Grid.Cells.Get(cell);

                if (meta)
                {
                    CellMeta otherMeta = Grid.Cells.Get(other);
                    if (otherMeta && Math.Abs(meta.elevationTier - otherMeta.elevationTier) <= 1)
                        return true;
                }

                if (!allowingDiagonals && Pathing.GetDiagonal(other, cell, out Diagonal diagonal))
                    return false;

                if (Pathing.BlockedCompletely(cell))
                    return false;

                return false;
            }

            public Cell GetContext()
            {
                return cell;
            }

        }
    }
}
