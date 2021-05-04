using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elevation
{

    public class PrebakedPathfinder : ElevationPathfinder
    {
        private const string V = "_";

        //private static Dictionary<string, Node> _pathGrid;
        //private static Dictionary<string, Node> _upperGrid;
        private static ClusterGrid ClusterGrid;

        /// <summary>
        /// Multiplier of the additional cost applied for the difference between two elevated cells' tiers
        /// </summary>
        public static readonly float ElevationClimbCostMultiplier = 1.5f;
        /// <summary>
        /// Maximum difference between two elevated cells' tiers that can still be climbed
        /// </summary>
        public static readonly int ElevationClimbThreshold = 1;
        /// <summary>
        /// Base cost of pathfinding
        /// </summary>
        public static float BasePathfindingCost { get; } = 1f;
        /// <summary>
        /// G score appended per node
        /// </summary>
        public static float CostPerNode { get; } = 1f;

        /// <summary>
        /// Cost to travel between grids
        /// </summary>
        public static float IntergridTraversalCost { get; } = 0f;

        // the dimensions, height and width, of the cluster grid
        public static int ClusterGridClusterDimentions { get; } = 10;
        
        /// <summary>
        /// If this funciton returns true on a node, it can be used to travel from the path to the upper grid or vice versa
        /// </summary>
        public static Func<Node, bool> CheckIntergridTravel { get; } = (n) => n.cell.isStairs;

        public static Func<Cell, int, bool> BlocksPath { get; } = (cell, team) =>
        {
            return cell.Type == ResourceType.Water;
        };

        #region Initialization

        public override void Init(int width, int height)
        {
            Mod.dLog("Pathfinding Initializing");
            
            // Path grid is the base grid; terrain tiles, this will never change
            // Upper grid is a grid exclusively for structures units can travel on top of
            // such as castle blocks or gates, these y-coordinates will change dynamically

            Mod.dLog("Creating");
            
            // For incrementing the cluster grid.
            // clusterGridRow is the row on the grid of clusters currently being accessed
            int clusterGridRow = 0;

            // clusterGridColumn is the column on the grid of clusters currently being accessed
            int clusterGridColumn = 0;

            // used to keep track of where in the cluster we are.
            int currentClusterRow = 0;

            // used to keep track of where in the cluster we are.
            int currentClusterColumn = 0;
            
            // Init all grids
            ClusterGrid = new ClusterGrid(ClusterGridClusterDimentions, height, width);
                
            // For loop handling setting up the cluster grid using World.inst.GetCellData(i, j) to 
            // iterate through the world grid and clusterRow and clusterColumn to place them in the 
            // correct cluster on the grid.

            // i is the world grids width.
            for(int i = 0; i < width; i++){

                // j is the world grids height.
                for(int j = 0; j < height; j++){

                    Cell cell = World.inst.GetCellData(i, j);

                    if (cell == null)
                        continue;

                    Elevation.PrebakedPathfinder.Node pathGridNode = new Elevation.PrebakedPathfinder.Node()
                    {
                        cell = cell,
                        meta = Grid.Cells.Get(cell),
                        upperGrid = false
                    };

                    Elevation.PrebakedPathfinder.Node upperGridNode = new Elevation.PrebakedPathfinder.Node()
                    {
                        cell = cell,
                        meta = Grid.Cells.Get(cell),
                        upperGrid = true
                    };
                    
                    string id = clusterGridColumn + V + clusterGridRow + ":" + currentClusterColumn + V + currentClusterRow;

                    currentClusterRow++;

                    if (!ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersGrid.ContainsKey(id))
                        ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersGrid.Add(id, pathGridNode);
                    else
                        ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersGrid[id] = pathGridNode;
                       
                    if (!ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersUpperGrid.ContainsKey(id))
                        ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersUpperGrid.Add(id, upperGridNode);
                    else
                        ClusterGrid.Clusters[clusterGridColumn][clusterGridRow].ClustersUpperGrid[id] = upperGridNode;
                        
                    // if checking if it is time to change cluster grid column.
                    if((j + 1) % (height / ClusterGridClusterDimentions) == 0){

                        clusterGridRow++;

                        currentClusterRow = 0;
                    }
                }

                currentClusterColumn++;

                // if checking if it is time to change cluster grid rows.
                if((i + 1) % (width / ClusterGridClusterDimentions)  == 0){

                    clusterGridColumn++;

                }
                else if(i == width){

                    clusterGridColumn = 0;
                }

            }

            // clusterGridRow is the row on the grid of clusters currently being accessed
            clusterGridRow = 0;

            // clusterGridColumn is the column on the grid of clusters currently being accessed
            clusterGridColumn = 0;
            
                // checking ouside ring of clusters open paths to neighboring clusters
            foreach(List<Cluster> clusterColumn in ClusterGrid)
            {
            
                foreach(Cluster cluster in clusterColumn){ 
            
                    foreach (Node node in cluster.ClustersGrid.Values) {
                    
                        node.ClearConnections();
                    }
                    
                    foreach(Node node in cluster.ClustersUpperGrid.Values) {
                    
                        node.ClearConnections();
                    }
                    
                    for(int i = 0; i < width / ClusterGridClusterDimentions; i++){ 
                    
                        for(int j = 0; j < height / ClusterGridClusterDimentions; j++){
                        
                            if((i == 0 || i + 1 == width / ClusterGridClusterDimentions) || (j == 0 || j + 1 == width / ClusterGridClusterDimentions)){
                                
                                CellMeta mark = Grid.Cells.Get(cluster.ClustersGrid[clusterGridColumn + V + clusterGridRow + V + currentClusterColumn + V + currentClusterRow].cell);
                                
                                CellMeta[] neighbors = mark.neighborsPlusFast;
                                
                                Node current_path = cluster.ClustersGrid[clusterGridColumn + V + clusterGridRow + ":" + currentClusterColumn + V + currentClusterRow];
                                
                                Node current_upper = cluster.ClustersUpperGrid[clusterGridColumn + V + clusterGridRow + ":" + currentClusterColumn + V + currentClusterRow];
                                
                                if(j == 0){
                                
                                    MakeConnections(neighbors[7], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[0], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[1], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                }
                                
                                if(j + 1 == height / ClusterGridClusterDimentions){
                                    
                                    MakeConnections(neighbors[1], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[2], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[3], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                }
                                
                                if(i + 1 == width / ClusterGridClusterDimentions){
                                    
                                    MakeConnections(neighbors[3], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[4], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[5], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                }
                                
                                if(i == 0){
                                    
                                    MakeConnections(neighbors[5], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[6], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                    
                                    MakeConnections(neighbors[7], mark, cluster.ClustersGrid, current_path, cluster.ClustersUpperGrid, current_upper);
                                }
                            }
                        }
                    }
                    clusterGridRow++;
                }
                
                clusterGridColumn++;
                
                clusterGridRow = 0;
            }

            Mod.dLog("Connecting");

            Mod.Log("Pathfinding Initialized");
        }
        
        public static void MakeConnections(CellMeta neighbor, CellMeta mark, Dictionary<string, Node> _pathGrid, Node current_path, Dictionary<string, Node> _upperGrid, Node current_upper){
                                
            if (neighbor == null)
                return;

            string id = CellMetadata.GetPositionalID(neighbor.cell);

            string[] strings = id.Split(Convert.ToChar("_"));

            int neighborGridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

            int neighborGridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

            int currNeighborClusterColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

            int currNeighborClusterRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

            string currId = CellMetadata.GetPositionalID(neighbor.cell);

            string[] currStrings = id.Split(Convert.ToChar("_"));

            int currGridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

            int currGridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

            int currClusterColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

            int currClusterRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

            Dictionary<string, Node> neighbor_upperGrid = ClusterGrid.Clusters[neighborGridColumn][neighborGridRow].ClustersUpperGrid;
               
            Node neighborNode_upper = neighbor_upperGrid[neighborGridColumn + V + neighborGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow];
             
            current_upper.AddConnection(neighborNode_upper, BasePathfindingCost);

            neighborNode_upper.AddConnection(current_upper, BasePathfindingCost);

            int difference = Math.Abs(mark.elevationTier - neighbor.elevationTier);

            if (difference <= ElevationClimbThreshold){
               
                Dictionary<string, Node> neighbor_pathGrid = ClusterGrid.Clusters[neighborGridColumn][neighborGridRow].ClustersGrid;

                Node neighborNode_path = _pathGrid[neighborGridColumn + V + neighborGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow];

                current_path.AddConnection(neighborNode_path, BasePathfindingCost + (difference * ElevationClimbCostMultiplier));

                neighborNode_path.AddConnection(current_path, BasePathfindingCost + (difference * ElevationClimbCostMultiplier));

                ClusterGrid.Clusters[currGridColumn][currGridRow].Routes.Add(currGridColumn + V + currGridRow + ":" + neighborGridColumn + V + neighborGridRow, new GridRoute(currGridColumn, currGridRow, currGridColumn + V + currGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow, neighborGridColumn + V + neighborGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow));

                ClusterGrid.Clusters[neighborGridColumn][neighborGridRow].Routes.Add(neighborGridColumn + V + neighborGridRow + ":" + currGridColumn + V + currGridRow, new GridRoute(currGridColumn, currGridRow, neighborGridColumn + V + neighborGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow, currGridColumn + V + currGridRow + ":" + currNeighborClusterColumn + V + currNeighborClusterRow));
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Gets the node at the given cell coordinate in either the upper or lower grid depending on the param upperGrid
        /// <para>Time Complexity: O(1)</para>
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="upperGrid"></param>
        /// <returns></returns>
        public static Node GetAt(Cell cell, bool upperGrid = false)
        {

            if (cell == null)
                return null;
            
            string id = CellMetadata.GetPositionalID(cell);

            string[] strings = id.Split(Convert.ToChar("_"));

            int GridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

            int GridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

            int currGridColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

            int currGridRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

            return upperGrid ?
                (ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid.ContainsKey(GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow) ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : null) :
                (ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid.ContainsKey(GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow) ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : null);
        }

        /// <summary>
        /// Gets the node at the given world-bound coordinate in either the upper or lower grid depending on the param upperGrid
        /// <para>Time Complexity: O(1)</para>
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="upperGrid"></param>
        /// <returns></returns>
        public static Node GetAt(int x, int z, bool upperGrid = false)
        {

            if (new Vector3(x, 0f, z) != World.inst.ClampToWorld(new Vector3(x, 0f, z)))
                return null;

            string id = CellMetadata.GetPositionalID(x, z);

            string[] strings = id.Split(Convert.ToChar("_"));

            int GridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

            int GridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

            int currGridColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

            int currGridRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

            return upperGrid ?
                (ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid.ContainsKey(GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow) ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : null) :
                (ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid.ContainsKey(GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow) ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : null);

        }

        public static Node GetClosestUnblocked(Node node, int team, Pathfinder.blocksPathTest blocks, bool upperGrid)
        {
            Node found = null;

            if (blocks(node.cell, team) || node.connected.Count == 0)
            {

                Cell cell = World.inst.FindMatchingSurroundingCell(node.cell, false, 2, c =>
                {

                    Node n = GetAt(c);

                    return !(blocks(c, team) && n.connected.Count != 0 && n.upperGrid == upperGrid);
                });

                found = cell != null ? GetAt(cell, upperGrid) : null;
            }
            else
                found = node;
            return found;
        }

        private static Node GetNextCandidate(List<Node> openSet)
        {

            if (openSet.Count > 0)
            {

                float lowest = float.MaxValue;

                Node candidate = null;

                for (int i = 0; i < openSet.Count; i++)
                {

                    Node n = openSet[i];

                    if (n.f < lowest)
                    {

                        lowest = n.f;
                        candidate = n;
                    }
                }

                return candidate;
            }

            return null;
        }

        private static float Dist(Vector3 start, Vector3 end)
        {

            return (start - end).sqrMagnitude;
        }

        private static float Dist(Node start, Node end)
        {

            return Dist(start.cell.Center.xz(), end.cell.Center.xz());
        }

        public static bool Connected(Cell a, Cell b)
        {

            Node nA = GetAt(a);
            Node nB = GetAt(b);

            return nA.connected.ContainsKey(nB);
        }

        private static void Clear()
        {

            foreach (List<Cluster> clusters in ClusterGrid.Clusters)
            {

                foreach(Cluster cluster in clusters)
                {

                    cluster.ClustersGrid.Clear();

                    cluster.ClustersUpperGrid.Clear();
                }
            }
        }


        #endregion

        #region Pathing

        // Implementation of A* pathfinding algorithm; modified slightly for KC and Elevation
        public override void Path(
            Vector3 startPos,
            bool upperGridStart,
            Vector3 endPos,
            bool upperGridEnd,

            ref List<Vector3> path,

            Pathfinder.blocksPathTest blocksPath,
            Pathfinder.blocksPathTest pull,
            Pathfinder.applyExtraCost extraCost,

            int team,

            bool doDiagonal,
            bool doTrimming,
            bool allowIntergridTravel)
        {
            Clear();

            // Blocks water as well as other obstacles
            Pathfinder.blocksPathTest blocks = (c, teamId) =>
            {

                return blocksPath(c, teamId) || BlocksPath(c, teamId);
            };

            // Init vars
            List<Node> openSet = new List<Node>();

            List<Node> closedSet = new List<Node>();

            Cell startCell = World.inst.GetCellDataClamped(World.inst.ClampToWorld(startPos));

            Cell endCell = World.inst.GetCellDataClamped(World.inst.ClampToWorld(endPos));

            Node start = GetAt(startCell, upperGridStart);

            Node end = GetAt(endCell, upperGridEnd);

            // Check for blocked start and/or end cell
            start = GetClosestUnblocked(start, team, blocks, upperGridStart);

            end = GetClosestUnblocked(end, team, blocks, upperGridEnd);

            List<string> visited = new List<string>();

            if (start == null || end == null)
            {

                path.Add(startPos);

                path.Add(endPos);

                return;
            }

            // Begin searching by adding start to open set
            start.g = 0;

            start.Heuristic(end);

            start.parent = null;

            openSet.Add(start);

            Node current = null;

            while (openSet.Count > 0)
            {

                // Find element with lowest f cost
                current = GetNextCandidate(openSet);

                openSet.Remove(current);

                closedSet.Add(current);

                // Check if reached destination
                if (current == end)
                {

                    try
                    {

                        path = RetracePath(current);
                    }
                    catch
                    {

                        Mod.dLog($"Path retrace looped on path {startPos}{(upperGridStart? "u" : "l")} to {endPos}{(upperGridEnd ? "u" : "l")}");
                    }

                    return;
                }

                // Find nodes that are connected
                Dictionary<Node, float> connected = current.connected;

                string posId = CellMetadata.GetPositionalID(current.cell);

                string[] strings = posId.Split(Convert.ToChar("_"));

                int GridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

                int GridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

                int currGridColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

                int currGridRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

                // Check for intergrid travel
                if (CheckIntergridTravel(current) && allowIntergridTravel)
                    connected.Add(
                        (current.upperGrid ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow]),
                        IntergridTraversalCost);

                // Check for diagonal travel
                if (!doDiagonal)
                    connected = ExcludeDiagonals(current, connected);


                // Add all connected to openset to be later evaluated
                foreach(KeyValuePair<string, GridRoute> route in ClusterGrid.Clusters[GridColumn][GridRow].Routes)
                {

                    CheckRoute(currGridColumn, currGridRow, ClusterGrid.Clusters[GridColumn][GridRow], GridColumn + V + GridRow + ":", route, team, closedSet, openSet, visited, blocks, extraCost, current, end);
                }
            }

            if (doTrimming)
            {

                StringPull(start, end, team, pull, extraCost);
            }
        }

        private static void CheckRoute(int currGridColumn, int currGridRow, Cluster cluster, string clusterIdPrefix, KeyValuePair<string, GridRoute> route, int team, List<Node> closedSet, List<Node> openSet, List<string> visited, Pathfinder.blocksPathTest blocks, Pathfinder.applyExtraCost extraCost, Node current, Node end)
        {
            for (int i = currGridColumn; i != route.Value.X; i -= ((i - route.Value.X) / -(i - route.Value.X)))
            {

                for (int j = currGridRow; j != route.Value.Z; j -= ((j - route.Value.Z) / -(j - route.Value.Z)))
                {
                    if (visited.Contains(clusterIdPrefix + i + V + j))
                        continue;

                    if (route.Key.Contains(i + V + j))
                        continue;


                    if (blocks(cluster.ClustersGrid[clusterIdPrefix + i + V + j].cell, team) || closedSet.Contains(cluster.ClustersGrid[clusterIdPrefix + i + V + j]))
                    {

                        CheckRoute(currGridColumn, currGridRow, cluster, clusterIdPrefix, route, team, closedSet, openSet, visited, blocks, extraCost, current, end);
                        
                    }
                    
                    float connectionCost = current.g +
                        (CostPerNode + route.Value.Value + extraCost(cluster.ClustersGrid[clusterIdPrefix + i + V + j].cell, team));

                    if (!openSet.Contains(cluster.ClustersGrid[clusterIdPrefix + i + V + j]))
                        openSet.Add(cluster.ClustersGrid[clusterIdPrefix + i + V + j]);

                    if (connectionCost < cluster.ClustersGrid[clusterIdPrefix + i + V + j].g || !cluster.ClustersGrid[clusterIdPrefix + i + V + j].visited)
                    {

                        cluster.ClustersGrid[clusterIdPrefix + i + V + j].g = connectionCost;

                        cluster.ClustersGrid[clusterIdPrefix + i + V + j].Heuristic(end);

                        cluster.ClustersGrid[clusterIdPrefix + i + V + j].parent = current;

                        cluster.ClustersGrid[clusterIdPrefix + i + V + j].visited = true;
                    }

                    visited.Add(clusterIdPrefix + i + V + j);

                }


            }
        }

        /// <summary>
        /// Extracts which nodes are diagonal connections from the origin's connections and returns the new list of connections without diagonals
        /// </summary>
        /// <param name="origin">node used to detremine position and diagonals</param>
        /// <param name="connections">Dictionary of connections that will be checked for diagonals</param>
        /// <returns></returns>
        private static Dictionary<Node, float> ExcludeDiagonals(Node origin, Dictionary<Node, float> connections)
        {
            Dictionary<Node, float> _new = new Dictionary<Node, float>(connections);

            foreach (KeyValuePair<Node, float> connected in connections)
                if (Pathing.GetDiagonal(origin.cell, connected.Key.cell, out Diagonal diag))
                    _new.Remove(connected.Key);

            return _new;
        }

        private static List<Vector3> RetracePath(Node final)
        {
            List<Vector3> path = new List<Vector3>();
            //path.Add(final.cell.Center);

            int iterations = 0;
            int kill = World.inst.GridWidth * World.inst.GridHeight;

            Node current = final;
            while (current.parent != null)
            {
                if (path.Contains(current.cell.Center))
                    throw new Exception("path loop detected");

                path.Insert(0, current.cell.Center);
                current = current.parent;


                iterations++;
                if (iterations > kill)
                    break;
            }


            return path;
        }

        #region Source

        // Directly taken from source (with slight modifications) at Pathfinder.StringPull
        private static void StringPull(Node start, Node end, int teamId, Pathfinder.blocksPathTest blocksPath, Pathfinder.applyExtraCost extraCost)
        {
            Pathfinder.blocksPathTest blocks = (c, team) =>
            {
                return blocksPath(c, team) || BlocksPath(c, team);
            };

            Node node = end;
            if (node.parent != null)
            {
                Node parent = node.parent.parent;
                while (parent != null)
                {
                    bool flag = LineBlock(node, parent, extraCost, blocks, teamId);
                    if (node.cell.isUpperGridBlocked && flag)
                    {
                        node.parent = parent;
                        if (parent == start)
                        {
                            break;
                        }
                        parent = parent.parent;
                    }
                    else
                    {
                        node = node.parent;
                        parent = node.parent.parent;
                    }
                }
            }
        }

        // Directly taken from source (with slight modifications) at Pathfinder.rasterLine
        private static bool LineBlock(Node a, Node b, Pathfinder.applyExtraCost extraCost, Pathfinder.blocksPathTest blockFunc, int teamId)
        {
            bool result;
            if (a.upperGrid != b.upperGrid)
            {
                result = false;
            }
            else
            {
                int x1 = a.cell.x;
                int z1 = a.cell.z;
                int x2 = b.cell.x;
                int z2 = b.cell.z;
                float y = a.cell.Center.y;
                int startCost = extraCost(a.cell, teamId);
                int num4 = Mathf.Abs(x2 - x1);
                int num5 = (x1 >= x2) ? -1 : 1;
                int num6 = Mathf.Abs(z2 - z1);
                int num7 = (z1 >= z2) ? -1 : 1;
                int num8 = num4 - num6;
                float num9 = (num4 + num6 != 0) ? Mathf.Sqrt((float)num4 * (float)num4 + (float)num6 * (float)num6) : 1f;
                for (; ; )
                {
                    string posId = CellMetadata.GetPositionalID(a.cell);

                    string[] strings = posId.Split(Convert.ToChar("_"));

                    int GridColumn = Int32.Parse(strings[0]) / ClusterGridClusterDimentions;

                    int GridRow = Int32.Parse(strings[1]) / ClusterGridClusterDimentions;

                    int currGridColumn = Int32.Parse(strings[0]) % ClusterGridClusterDimentions;

                    int currGridRow = Int32.Parse(strings[1]) % ClusterGridClusterDimentions;

                    Node node = a.upperGrid ? ClusterGrid.Clusters[GridColumn][GridRow].ClustersUpperGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow] : ClusterGrid.Clusters[GridColumn][GridRow].ClustersGrid[GridColumn + V + GridRow + ":" + currGridColumn + V + currGridRow];
                    if (GetPathBlocked(node, blockFunc, teamId) || startCost != extraCost(node.cell, teamId) || Mathf.Abs(node.cell.Center.y - y) > 0.01f)
                    {
                        break;
                    }
                    int num10 = num8;
                    int num11 = x1;
                    if (2 * num10 >= -num4)
                    {
                        if (x1 == x2)
                        {
                            goto Block_8;
                        }
                        if ((float)(num10 + num6) < num9)
                        {

                            node = GetAt(x1, z1 + num7, a.upperGrid);
                            if (node == null || GetPathBlocked(node, blockFunc, teamId) || startCost != extraCost(node.cell, teamId) || Mathf.Abs(node.cell.Center.y - y) > 0.01f)
                            {
                                goto IL_1C3;
                            }
                        }
                        num8 -= num6;
                        x1 += num5;
                    }
                    if (2 * num10 <= num6)
                    {
                        if (z1 == z2)
                        {
                            goto Block_13;
                        }
                        if ((float)(num4 - num10) < num9)
                        {
                            node = GetAt(num11 + num5, z1, a.upperGrid);
                            if (node == null || GetPathBlocked(node, blockFunc, teamId) || startCost != extraCost(node.cell, teamId) || Mathf.Abs(node.cell.Center.y - y) > 0.01f)
                            {
                                goto IL_25C;
                            }
                        }
                        num8 += num4;
                        z1 += num7;
                    }
                }
                return false;
            Block_8:
                goto IL_277;
            IL_1C3:
                return false;
            Block_13:
                goto IL_277;
            IL_25C:
                return false;
            IL_277:
                result = true;
            }
            return result;
        }

        // Directly taken from source (with slight modifications) at Pathfinder.GetPathBlocked
        private static bool GetPathBlocked(Node n, Pathfinder.blocksPathTest bt, int teamId)
        {
            if (n.upperGrid != true)
            {
                if (bt(n.cell, teamId))
                {
                    return true;
                }
            }
            else if (n.cell.isUpperGridBlocked)
            {
                return true;
            }
            return false;
        }

        //public override void Path(Vector3 startPos, bool upperGridStart, Vector3 endPos, bool upperGridEnd, ref List<Vector3> path, Pathfinder.blocksPathTest blocksPath, Pathfinder.blocksPathTest pull, Pathfinder.applyExtraCost extraCost, int team, bool doDiagonal, bool doTrimming, bool allowIntergridTravel)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #endregion

        // TODO: Upper grid connection checks during find path

        public class Node : IComparable<Node>
        {
            /// <summary>
            /// Previous node in path
            /// </summary>
            public Node parent { get; set; } = null;
            /// <summary>
            /// Whether or not this node has been visited
            /// </summary>
            public bool visited = false;

            public float h;
            public float g;
            /// <summary>
            /// Cumulative cost factoring the heuristic and g cost
            /// </summary>
            public float f => h + g; 

            /// <summary>
            /// Dictionary of the connected nodes to this node and the travel costs of each connection
            /// </summary>
            public Dictionary<Node, float> connected = new Dictionary<Node, float>(9);
            public Cell cell;
            public CellMeta meta;

            public bool upperGrid;

            /// <summary>
            /// Positional id of this node's cell
            /// </summary>
            public string id => CellMetadata.GetPositionalID(cell);

            public bool Connected(Node node) => connected.ContainsKey(node);

            public void AddConnection(Node connection, float weight = 1f)
            {
                connected.Add(connection, weight);
            }

            public void ClearConnections()
            {
                connected.Clear();
            }

            public float Heuristic(Node other)
            {
                h = PrebakedPathfinder.Dist(cell.Center.xz(), other.cell.Center.xz());
                return h;
            }

            public override string ToString()
            {
                string text = $"Node: {(meta != null ? meta.id : "No Mark")}; Grid: {(upperGrid ? "Upper" : "Path")}";

                return text;
            }

            public int CompareTo(Node other)
            {
                if (other == null)
                    return 1;

                return this.f.CompareTo(other.f);
            }
        }


    }

}
