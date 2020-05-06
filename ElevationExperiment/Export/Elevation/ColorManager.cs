using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevationExperiment
{
    static class ColorManager
    {
        private static Dictionary<int, Color> tierColoring = new Dictionary<int, Color>()
        {
            { 1, new Color(0.662f, 0.854f, 0.564f) },
            { 2, new Color(0.709f, 0.807f, 0.533f) },
            { 3, new Color(0.803f, 0.764f, 0.596f) },
            { 4, new Color(0.819f, 0.811f, 0.780f) },
            { 5, new Color(0.647f, 0.639f, 0.611f) },
            { 6, new Color(0.549f, 0.549f, 0.549f) },
            { 7, new Color(0.690f, 0.690f, 0.690f) },
            { 8, new Color(0.866f, 0.886f, 0.854f) }
        };

        public static Texture2D elevationMap;
        public static Material terrainMat;
        public static float tilingConstant;

        public static Color GetColor(int elevationTier)
        {
            return tierColoring.ContainsKey(elevationTier) ? tierColoring[elevationTier] : Color.black;
        }

        public static void SetColor(int elevationTier, Color color)
        {
            if (tierColoring.ContainsKey(elevationTier))
            {
                tierColoring[elevationTier] = color;
                Mod.dLog("changed color to " + color.ToString());
            }
        }

        public static void Update()
        {
            BakeElevationMap();
            SetTerrainMat();
            ElevationManager.PushMeshUpdate();

        }

        

        public static void Setup()
        {
            try
            {
                tierColoring = Settings.elevationColorPresets["Default"];
                BakeElevationMap();
                SetTerrainMat();
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }

        public static void BakeElevationMap()
        {

            Texture2D tex = new Texture2D(1, ElevationManager.maxElevation - ElevationManager.minElevation, TextureFormat.ARGB32, false);

            for (int i = ElevationManager.maxElevation; i > ElevationManager.minElevation; i--)
            {
                tex.SetPixel(1, i - ElevationManager.maxElevation - 1, tierColoring[i]);
            }
            tex.Apply();

            Mod.Log("Terrain Map Baked");
            //Texture2D test = World.inst.CaptureCameraShot(1080, 600);
            //World.inst.SaveTexture(Mod.helper.modPath + "/tex.png", tex);

            elevationMap = tex;
        }



        public static void SetTerrainMat()
        {
            tilingConstant = 1f / (ElevationManager.maxElevation - ElevationManager.minElevation);
            terrainMat = new Material(Shader.Find("Custom/Snow2"))
            {
                color = Color.white
            };
            elevationMap.filterMode = FilterMode.Point;

            terrainMat.mainTexture = elevationMap;
            //terrainMat.mainTextureScale = new Vector2(1f,tilingConstant);


            Mod.Log("Terrain Material Setup"); 

            if (terrainMat == null)
                Mod.Log("could not find terrain material");
        }
    }
}
