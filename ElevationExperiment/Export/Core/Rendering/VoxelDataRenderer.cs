using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fox.Rendering.VoxelRendering;

namespace Elevation
{
    /// <summary>
    /// Rendering mode that utilizes minecraft style voxel rendering; makes better face culling and multiple tiles on one XZ grid space possible, also uses subdivided faces allowing diveting
    /// </summary>
    public class VoxelRenderingMode : RenderingMode
    {
        private VoxelDataRenderer voxels;
        private Mesh mesh;

        public override void Init()
        {
            voxels = new VoxelDataRenderer();
            voxels.textureSize = new Vector2(ElevationManager.maxElevation - ElevationManager.minElevation, 1);
        }

        public override void Setup()
        {
            if (voxels == null)
                return;

            Voxel[,,] data = new Voxel[World.inst.GridWidth, ElevationManager.maxElevation + 1, World.inst.GridHeight];

            for (int x = 0; x < World.inst.GridWidth; x++)
            {
                for(int z = 0; x < World.inst.GridHeight; z++)
                {
                    for(int y = 0; y < ElevationManager.maxElevation + 1; y++)
                    {
                        Cell cell = World.inst.GetCellData(x, z);
                        CellMeta meta = Grid.Cells.Get(x, z);
                        if (meta && y <= meta.elevationTier)
                        {
                            voxels.data[x, y, z].opacity = 1f;
                            voxels.data[x, y, z].uvOffset = new Vector2(y, 0f);
                        }
                        else
                            voxels.data[x, y, z].opacity = 0f;
                    }
                }
            }

            mesh = voxels.Rebuild(data);



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
                        voxels[x, i, z].opacity = i <= meta.elevationTier ? 1f : 0f;
        }

        public override void UpdateAll(bool forced = false)
        {
            Setup();
        }

        public override void Tick()
        {
            Graphics.DrawMesh(mesh, new Vector3(World.inst.GridWidth / 2f, 1f, World.inst.GridHeight / 2f), Quaternion.identity, ColorManager.terrainMat, 0);
        }
    }
}

namespace Fox.Rendering.VoxelRendering
{

    /// <summary>
    /// Builds a face-based mesh construction of a group of voxels
    /// </summary>
    public class VoxelDataRenderer
    {
        public static float buffer { get; } = 0.001f;

        #region Settings

        /// <summary>
        /// The number of elements in each dimension of this chunk
        /// </summary>
        public Vector3Int dimensions = new Vector3Int(10, 10, 10);
        /// <summary>
        /// The world space size 1 voxel index represents
        /// </summary>
        public float voxelSize = 1f;

        /// <summary>
        /// divets create minor variations in the center point of each quad face
        /// <para>Set to 0 for no divets (uses simple quad rendering rather than 9-point quads)</para>
        /// </summary>
        public float divetVariation = 0.2f;

        /// <summary>
        /// When set to true, mesh will use 32 bit integers rather than 16 bit; 
        /// <para>doubles memory footprint but allows up to 4 billion vertices versus the traditional 65535 vertex limit.  </para>
        /// </summary>
        public bool _32BitBuffer = true;

        public Vector2 textureSize;

        #endregion

        public Voxel[,,] data;

        public Mesh mesh;

        public Voxel this[int x, int y, int z]
        {
            private set
            {
                if (InDimensions(new Vector3Int(x, y, z)))
                    data[x, y, z] = value;
            }
            get
            {
                if (InDimensions(new Vector3Int(x, y, z)))
                    return data[x, y, z];
                return null;
            }
        }

        public void Reset()
        {
            data = new Voxel[dimensions.x, dimensions.y, dimensions.z];

            Loop((x, y, z) =>
            {
                data[x, y, z] = new Voxel();

                data[x, y, z].index = new Vector3Int(x, y, z);
                data[x, y, z].opacity = 1f;
            });
        }

        public Mesh Rebuild(Voxel[,,] data = null)
        {
            if (data != null)
            {
                this.data = data;
                dimensions = new Vector3Int(this.data.GetLength(0), this.data.GetLength(1), this.data.GetLength(2));
            }

            mesh = new Mesh();
            int master = 0;

            if (_32BitBuffer)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            Loop((voxel) =>
            {
                int x = voxel.index.x;
                int y = voxel.index.y;
                int z = voxel.index.z;

                Vector3[] faceNormals = GetFaces(voxel);

                foreach (Vector3 normal in faceNormals)
                {
                    Quad face = divetVariation > 0f ?
                            RendererUtil.NinePointQuad(
                            new Vector2(voxelSize, voxelSize),
                            new Vector3(x, y, z) * voxelSize,
                            master) :
                            RendererUtil.Quad(
                            new Vector2(voxelSize, voxelSize),
                            new Vector3(x, y, z) * voxelSize,
                            master);
                    face = face.SetRotation(
                            normal,
                            new Vector3(voxelSize / 2f, voxelSize / 2f, voxelSize / 2f) +
                            new Vector3(x, y, z) * voxelSize,
                            Vector3.up
                            );

                    if (divetVariation > 0f)
                        face.vertices[3] = face.vertices[3] + (normal * UnityEngine.Random.Range(-divetVariation, divetVariation));

                    for (int i = 0; i < face.vertices.Length; i++)
                        face.uvs[i] = (face.uvs[i] / textureSize) - new Vector2(buffer, buffer)
                        + (voxel.uvOffset / textureSize)
                        ;

                    mesh.Add(face);

                    master += face.vertices.Length;
                }
            });

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }


        public Vector3[] GetFaces(Voxel voxel)
        {
            if (voxel.opacity <= 0f)
                return new Vector3[0];

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
                Voxel neighbor = GetAt(new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z) + voxel.index);
                if (neighbor != null)
                    if (neighbor.opacity <= 0f)
                        found.Add(vector);
            }

            return found.ToArray();
        }

        /// <summary>
        /// Gets access to tile outside of this render data (for border tiles)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Voxel External(Vector3Int index)
        {
            return null;
        }

        public Voxel GetAt(int x, int y, int z) => GetAt(new Vector3Int(x, y, z));

        public Voxel GetAt(Vector3Int index)
        {
            if (InDimensions(index))
            {
                return data[index.x, index.y, index.z];
            }
            Voxel external = External(index);
            if (external != null && external.opacity <= 0f)
                return external;
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
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int y = 0; y < data.GetLength(1); y++)
                {
                    for (int z = 0; z < data.GetLength(2); z++)
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
                if (data[x, y, z] != null)
                    action(data[x, y, z]);
            });
        }



        #endregion

    }

    [Serializable]
    public class Voxel
    {
        /// <summary>
        /// Index of this voxel within its domain
        /// </summary>
        public Vector3Int index;
        /// <summary>
        /// TEMP: any opacity lower than 1f will mean the voxel will not be rendered
        /// </summary>
        public float opacity;

        /// <summary>
        /// UV coordinate of this voxel through its domain's texture
        /// </summary>
        public Vector2 uvOffset;

        public static implicit operator bool(Voxel obj) => obj != null;
    }

}