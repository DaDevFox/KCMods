using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;
using Zat.Shared.Rendering;
using Elevation.Utils;
using Priority_Queue;

namespace Elevation.Patches
{
    public class PathfindingVisualIndicator
    {

        //[HarmonyPatch(typeof(PlacementMode), "Update")]
        class PlacementModeUpdatePatch
        {
            private static LineRenderer renderer;

            public static Vector3 offset { get; } = new Vector3(0f,0.1f,0f);

            public static Dictionary<Direction, Vector3> DirectionAnchorPoints { get; } = new Dictionary<Direction, Vector3>() 
            {
                { Direction.East, new Vector3(-0.5f, 0f, 0f) },
                { Direction.West, new Vector3(0.5f, 0f, 0f) },

                { Direction.South, new Vector3(0f, 0f, -0.5f) } ,
                { Direction.North, new Vector3(0f, 0f, 0.5f) } 
            };


            static void Postfix(PlacementMode __instance)
            {
                //if (renderer == null)
                //{
                //    GameObject GO = GameObject.Instantiate(new GameObject(), World.inst.transform);
                //    renderer = GO.AddComponent<LineRenderer>();
                //    renderer.startColor = Color.blue;
                //    renderer.endColor = Color.blue;
                //    renderer.material = new Material(Shader.Find("Standard"));
                //    renderer.material.color = Color.blue;
                //    renderer.startWidth = 0.1f;
                //    renderer.endWidth = 0.1f;
                //    renderer.alignment = LineAlignment.View;
                //    renderer.loop = false;
                //    renderer.numCapVertices = 1;
                //    renderer.enabled = false;
                //}

                
                //if (__instance.IsPlacing() && Settings.inst.c_Visual.s_VisualPathfindingIndicatorEnabled.Value)
                //{
                //    bool onElevation = BuildingPlacementPatch.GetLevellingForBuilding(__instance.GetHoverBuilding()) >= 0.5f;
                //    bool dragging = (bool)typeof(PlacementMode)
                //        .GetField("attemptedDrag", BindingFlags.NonPublic | BindingFlags.Instance)
                //        .GetValue(__instance);

                //    if (!onElevation)
                //        return;

                //    if (dragging)
                //        return;

                //    renderer.enabled = true;

                //    Building b = __instance.GetHoverBuilding();
                //    Cell cell = World.inst.GetCellData(b.transform.position);

                //    if (cell != null)
                //    {
                        
                //        List<Vector3> positions = new List<Vector3>();
                //        List<Cell> cells = new List<Cell>();

                //        FindBestPath(cell, ref positions);

                //        foreach (Vector3 pos in positions)
                //        {
                //            Cell c = World.inst.GetCellDataClamped(pos);
                //            if (c != null)
                //                cells.Add(c);
                //        }

                //        positions.Clear();

                //        Cell lastCell = null;
                //        foreach (Cell c in cells)
                //        {
                //            if (lastCell != null)
                //            {
                //                if (PathingManager.GetCardinal(lastCell, c, out Direction dir1))
                //                {
                //                    Vector3 anchorPoint = DirectionAnchorPoints[dir1];

                //                    positions.Add(lastCell.Center + anchorPoint + offset);
                //                }

                //                if (PathingManager.GetCardinal(c, lastCell, out Direction dir2))
                //                {
                //                    Vector3 anchorPoint = DirectionAnchorPoints[dir2];

                //                    positions.Add(c.Center + anchorPoint + offset);
                //                }
                //            }
                //            positions.Add(c.Center + offset);
                //            lastCell = c;
                //        }

                //        renderer.positionCount = positions.Count;
                //        renderer.SetPositions(positions.ToArray());
                        
                //    }
                //    else
                //        renderer.enabled = false;
                //}
                //else
                //    renderer.enabled = false;
            }

            

            static void FindBestPath(Cell c, ref ArrayExt<Vector3> positions, int maxRadius = 15, int teamID = 0)
            {
                SimplePriorityQueue<Cell> queue = new SimplePriorityQueue<Cell>();

                World.inst.ForEachTileInRadius(c.x,c.z,maxRadius, 
                    (x,z,cell) =>
                    {
                        CellMeta meta = Grid.Cells.Get(cell);
                        if (meta != null) {

                            float priority = maxRadius;

                            priority *= 1f/World.inst.LineTestAccumFootpathCost(c, cell, teamID);

                            priority *= ElevationManager.maxElevation - meta.elevationTier;

                            queue.Enqueue(cell, priority);
                        }
                    });

                Cell chosen = queue.Dequeue();

                World.inst.FindFootPath(c.Center.xz(), chosen.Center.xz(), ref positions);
            }

        }

    }
}
