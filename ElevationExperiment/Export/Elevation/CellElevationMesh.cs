using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevationExperiment
{
    class CellElevationMesh : MonoBehaviour
    {
        private Material mat;
        private MeshRenderer renderer;

        public CellMark cellMark;
        public Cell cell;

        int lastTier = 0;

        #region Mesh Info

        /// <summary>
        /// A little bit of extra height for the meshes
        /// on the bottom half of the cube to hide any
        /// misalignment due to the diveting of cells in 
        /// KC
        /// </summary>
        private float lowerBuffer = 0.02f;

        /// <summary>
        /// A very small amount of buffer between elevation tier coloring so that the colors aren't always clashing with eachother. 
        /// </summary>
        private float tierColorBuffer = 0.000001f;

        #endregion

        public void Init()
        {
            #region Initialization

            renderer = gameObject.GetComponent<MeshRenderer>();

            mat = renderer.material;

            #endregion
        }


        public static GameObject Make(Cell cell)
        {
            CellMark mark = ElevationManager.GetCellMark(cell);

            if(mark != null)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                CellElevationMesh script = obj.AddComponent<CellElevationMesh>();
                script.cell = cell;
                script.cellMark = mark;

                obj.transform.SetPositionAndRotation(new Vector3(cell.Center.x, 0f, cell.Center.z), Quaternion.identity);
                obj.transform.SetParent(World.inst.caveContainer.transform,true);

                script.Init();

                return obj;
            }
            return null;
        }

        public void OnDispose()
        {
            this.GetComponent<MeshFilter>().mesh = null;
            this.mat = null;
            GameObject.Destroy(gameObject);
        }

        public void UpdateMesh(bool force = false)
        {
            if (cellMark.elevationTier == 0)
                gameObject.SetActive(false);

            if (cellMark.elevationTier == lastTier && !force)
                return;

            if (cellMark.elevationTier != 0)
            {
                gameObject.SetActive(true);
                transform.position = new Vector3(cell.Center.x, ((ElevationManager.elevationInterval/2) * cellMark.elevationTier) - (lowerBuffer/2), cell.Center.z);
                transform.localScale = new Vector3(1f, (cellMark.elevationTier * ElevationManager.elevationInterval) + lowerBuffer, 1f);
            }
            
            lastTier = cellMark.elevationTier;

            ApplyTexture();
            AddNoise(GetComponent<MeshFilter>().mesh);
        }

        
        private void AddNoise(Mesh m)
        {
            float range = 1f;

            for(int i = 0; i < m.vertices.Length; i++)
            {
                m.vertices[i].y += SRand.Range(-range, range);
            }
        }

        private void ApplyTexture()
        {
            MeshFilter m = gameObject.GetComponent<MeshFilter>();
            m.mesh.uv = GetCorrectedElevationUVFromMesh(m.mesh.vertices);

            if (mat != null)
            {
                //mat.color = ColorManager.GetColor(cellMark.elevationTier);
                mat.mainTexture = ColorManager.elevationMap;
                mat.mainTextureScale = new Vector2(1f, ColorManager.tilingConstant * cellMark.elevationTier - tierColorBuffer);
            }
            else
            {
                mat = new Material(ColorManager.terrainMat)
                {
                    mainTexture = ColorManager.elevationMap,
                    mainTextureScale = new Vector2(1f, ColorManager.tilingConstant * cellMark.elevationTier - tierColorBuffer)
                };
            }
            if(renderer == null)
            {
                Mod.dLog("no renderer found");
            }

            renderer.material = mat;
        }

        private void UpdateSnow()
        {
            mat.SetFloat("_Snow", TerrainGen.inst.GetSnowFade());
            mat.SetVector("_SnowColor", TerrainGen.inst.snowColor);
        }


        private Vector2[] GetCorrectedElevationUVFromMesh(Vector3[] points)
        {
            Vector2[] uv = new Vector2[points.Length];
            for(int i = 0; i < points.Length; i++)
            {
                uv[i] = new Vector2(1f, points[i].y > -0.5f ? 1f : 0f);
            }
            return uv;
        }

        

        void Update()
        {
            UpdateSnow();
        }


    }
}
