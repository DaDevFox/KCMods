using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fox.Rendering.VoxelRendering;

namespace Elevation
{
    /// <summary>
    /// Rendering mode that utilizes minecraft style voxel rendering; makes better face culling and multiple tiles on one XZ grid space possible
    /// </summary>
    public class VoxelRenderingMode : RenderingMode
    {
        private VoxelDataRenderer voxels;

        public override void Init()
        {
            voxels = new GameObject("Elevation_VoxelRenderer").AddComponent<VoxelDataRenderer>();
        }

        public override void Setup()
        {
            if (!voxels)
                return;

            voxels.dimensions = new Vector3Int(World.inst.GridWidth, ElevationManager.maxElevation + 1, World.inst.GridHeight);
            voxels.ResetVoxels();

            voxels.Loop(
                (x, y, z) =>
                {
                    if (y == ElevationManager.maxElevation)
                        if(voxels.GetAt(x, y, z))
                            voxels[x, y, z].opacity = 0f;
                });


            Mod.dLog("Voxel Rendering Mode Initialized");
        }

        public override void Update(Cell cell, bool forced = false)
        {
            cell.CheckNull("cell");

            int x = cell.x;
            int z = cell.z;

            CellMeta meta = Grid.Cells.Get(cell);

            if (meta)
                for (int i = ElevationManager.minElevation; i < ElevationManager.maxElevation; i++)
                    if(voxels.GetAt(x, i, z))
                        voxels[x, i, z].opacity = i < meta.elevationTier ? 1f : 0f;
        }
    }
}

namespace Fox.Rendering.VoxelRendering
{

    public class VoxelDataRenderer : MonoBehaviour
    {
        /// <summary>
        /// The number of elements in each dimension of this chunk
        /// </summary>
        public Vector3Int dimensions = new Vector3Int(10, 10, 10);
        public float voxelSize = 1f;
        public Voxel[,,] chunk;

        private Mesh mesh;

        public Voxel this[int x, int y, int z]
        {
            private set
            {
                if (InDimensions(new Vector3Int(x, y, z)))
                    chunk[x, y, z] = value;
            }
            get
            {
                if(InDimensions(new Vector3Int(x, y, z)))
                    return chunk[x, y, z];
                return null;
            }
        }

        void Start()
        {
            mesh = new Mesh();
            
            if (!GetComponent<MeshCollider>())
                gameObject.AddComponent<MeshCollider>();
            if (!GetComponent<MeshFilter>())
                gameObject.AddComponent<MeshFilter>();


            ResetVoxels();
            Rebuild();
        }

        public void ResetVoxels()
        {
            chunk = new Voxel[dimensions.x, dimensions.y, dimensions.z];

            Loop((x, y, z) =>
            {
                chunk[x, y, z] = new Voxel();

                chunk[x, y, z].index = new Vector3Int(x, y, z);
                chunk[x, y, z].opacity = 1f;
            });
        }

        public void Rebuild()
        {
            mesh = new Mesh();
            int master = 0;

            Loop((voxel) =>
            {
                int x = voxel.index.x;
                int y = voxel.index.y;
                int z = voxel.index.z;

                List<Vector3> faceNormals = GetFaces(voxel);

                foreach (Vector3 normal in faceNormals)
                {
                    mesh.Add(
                        RendererUtil.Quad(
                            new Vector2(voxelSize, voxelSize),
                            new Vector3(x, y, z) * voxelSize,
                            master)
                        .SetRotation(
                            normal,
                            new Vector3(voxelSize / 2f, voxelSize / 2f, voxelSize / 2f) +
                            new Vector3(x, y, z),
                            Vector3.up
                            ));

                    master += 4;
                }
            });

            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;

        }


        public List<Vector3> GetFaces(Voxel voxel)
        {
            if (voxel.opacity < 1f)
                return new List<Vector3>();

            Vector3[] toCheck = new Vector3[]
            {
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),

            new Vector3(0f, 1f, 0f),
            new Vector3(0f, -1f, 0f),

            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, -1f),
            };

            List<Vector3> found = new List<Vector3>();


            foreach (Vector3 vector in toCheck)
            {
                if (GetAt(new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z) + voxel.index) != null)
                {
                    Voxel neighbor = GetAt(new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z) + voxel.index);
                    if (neighbor.opacity < 1f)
                        found.Add(vector);
                }
            }

            return found;
        }

        public Voxel GetAt(int x, int y, int z) => GetAt(new Vector3Int(x, y, z));

        public Voxel GetAt(Vector3Int index)
        {
            if (InDimensions(index))
            {
                return chunk[index.x, index.y, index.z];
            }
            return null;
        }

        private bool InDimensions(Vector3Int vector)
        {
            return (vector.x >= 0 && vector.x < dimensions.x)
                && (vector.y >= 0 && vector.y < dimensions.y)
                && (vector.z >= 0 && vector.z < dimensions.z);
        }



        #region Utils

        /// <summary>
        /// Loops through the chunk in the order x, y, z and calls a function (passing in the x, y, and z integer coordinates of that voxel)
        /// </summary>
        /// <param name="action"></param>
        public void Loop(Action<int, int, int> action)
        {
            for (int x = 0; x < chunk.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.GetLength(2); z++)
                    {
                        action(x, y, z);
                    }
                }
            }
        }

        /// <summary>
        /// Loops through the chunk in the order x, y, z and calls a function (passing in the voxel)
        /// </summary>
        /// <param name="action"></param>
        public void Loop(Action<Voxel> action)
        {
            Loop((x, y, z) =>
            {
                action(chunk[x, y, z]);
            });
        }



        #endregion

        
    }

    [Serializable]
    public class Voxel
    {
        public Vector3Int index;
        /// <summary>
        /// TEMP: any opacity lower than 1f will mean the voxel will not be rendered
        /// </summary>
        public float opacity;
        public bool collision => opacity == 1f;

        public static implicit operator bool(Voxel obj) => obj != null;
    }
}