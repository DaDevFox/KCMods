using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

namespace ElevationExperiment
{
    public class CellMark
    {
        public Cell cell;
        public Vector3 Center 
        {
            get
            {
                return new Vector3((float)cell.x + 0.5f, Elevation, (float)cell.z + 0.5f);
            }
        }
        
        public float Elevation 
        {
            get
            {
                return elevationTier == 0 ? 0f : (float)elevationTier * ElevationManager.elevationInterval;
            }
        }
        public int elevationTier = 0;

        public Color color;

        public TerrainChunk chunk;

        private CellElevationMesh mesh;

        public List<Direction> blockers = new List<Direction>();

        public CellMark(Cell cell)
        {
            this.cell = cell;
        }

        public void OnDispose()
        {
            if(mesh)
                mesh.OnDispose();
            this.elevationTier = 0;
            this.blockers.Clear();
            this.chunk = null;
            this.cell = null;
        }




        public void UpdateMesh(bool forced = false)
        {
            if (!ElevationManager.ValidTileForElevation(cell))
                return;

            if (mesh != null)
            {
                mesh.UpdateMesh(forced);
            }
            else
            {
                mesh = CellElevationMesh.Make(cell).GetComponent<CellElevationMesh>();
                mesh.UpdateMesh(forced);
            }
        }

        
        public void UpdatePathing()
        {
            if (!ElevationManager.ValidTileForElevation(cell))
                return;
            
            UpdatePathfinderCost();
            UpdateBlockers();
        }

        private void UpdatePathfinderCost()
        {
            for(int i = 0; i < cell.villagerFootPathCost.Length;i++)
            {
                cell.villagerFootPathCost[i] = PathingManager.tierPathingMin + (elevationTier * PathingManager.tierPathingCost);
            }
        }



        private void UpdateBlockers()
        {
            Cell[] cells = World.inst.GetNeighborCells(cell);
            Direction[] dirs = new Direction[4]
            {
                Direction.East,
                Direction.South,
                Direction.West,
                Direction.North
            };

            blockers.Clear();

            for (int i = 0; i < 4; i++)
            {
                Cell cell = cells[i];
                Direction dir = dirs[i];
                if (cell != null)
                {
                    if (!isPathable(cell))
                    {
                        blockers.Add(dir);
                    }
                }
            }
        }


        private bool isPathable(Cell cell)
        {
            CellMark mark = ElevationManager.GetCellMark(cell);
            if (mark != null)
            {
                if (Math.Abs(mark.elevationTier - elevationTier) <= 1)
                {
                    return true;
                }
            }
            return false;

        }
        

        

        public TerrainChunk GetTerrainChunk()
        {
            return chunk;
        }


        public int GetPathfindingCost()
        {
            return 0;
        }    


    }
}
