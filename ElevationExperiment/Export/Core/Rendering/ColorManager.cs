using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elevation
{
    public static class ColorManager
    {
        private static Dictionary<int, Color> tierColoring = new Dictionary<int, Color>()
        {
            //{1, new Color(0.486f, 0.552f, 0.298f) },
            //{2, new Color(0.709f, 0.729f, 0.380f) },
            //{3, new Color(0.447f, 0.329f, 0.156f) },
            //{4, new Color(0.501f, 0.501f, 0.501f) },
            //{5, new Color(0.360f, 0.360f, 0.360f) },
            //{6, new Color(0.250f, 0.250f, 0.250f) },
            //{7, new Color(0.509f, 0.509f, 0.509f) },
            //{8, new Color(0.803f, 0.796f, 0.796f) }
        };

        public static Texture2D elevationMap;
        public static Material terrainMat { get; set; }
        public static float tilingConstant;

        public static Color GetColor(int elevationTier)
        {
            return tierColoring.ContainsKey(elevationTier) ? tierColoring[elevationTier] : Color.black;
        }

        public static void SetColor(int elevationTier, Color color)
        {
            if (tierColoring.ContainsKey(elevationTier))
            {
                if (tierColoring[elevationTier] == color)
                    return;

                tierColoring[elevationTier] = color;
            }
            else
                tierColoring.Add(elevationTier, color);
        }

        public static void Update()
        {
            BakeElevationMap();
            SetTerrainMat();
            ElevationManager.UpdateCellMetas();
        }

        public static void Setup()
        {
            tierColoring = Settings.elevationColorPresets["Default"];
            BakeElevationMap();
            SetTerrainMat();   
        }

        public static void BakeElevationMap()
        {

            Texture2D tex = new Texture2D(1, ElevationManager.maxElevation - ElevationManager.minElevation, TextureFormat.ARGB32, false);

            for (int i = ElevationManager.maxElevation; i > ElevationManager.minElevation; i--)
            {
                tex.SetPixel(1, i - ElevationManager.maxElevation - 1, tierColoring[i]);
            }
            tex.Apply();

            Mod.dLog("Terrain Map Baked");

            elevationMap = tex;

            World.inst.SaveTexture(Mod.helper.modPath + "/terrainmap.png", elevationMap);
        }



        public static void SetTerrainMat()
        {
            tilingConstant = 1f / (ElevationManager.maxElevation - ElevationManager.minElevation);
            terrainMat =
                //World.inst.uniMaterial[0];
                new Material(Shader.Find("Standard"));
            terrainMat.enableInstancing = true;

            terrainMat.SetFloat("_Glossiness", 0f);
            terrainMat.SetFloat("_Metallic", 0f);

            elevationMap.filterMode = FilterMode.Point;

            terrainMat.mainTexture = elevationMap;

            terrainMat.color = Color.white;

            Mod.dLog("Terrain Material Setup");

            if (terrainMat == null)
                Mod.Log("could not find terrain material");
        }

        private static Texture2D GetWorldColorTexture()
        {
            Texture2D texture = new Texture2D(World.inst.GridWidth, World.inst.GridWidth);
            return texture;
        }

        private static Texture2D GetOverlayTexture()
        {
            return typeof(TerrainGen).GetField("overlayTexture", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(TerrainGen.inst) as Texture2D;
        }

        public static void Tick()
        {
            if (TerrainGen.inst.terrainChunks == null || TerrainGen.inst.terrainChunks.Count == 0)
                return;

            //TerrainChunk chunk = TerrainGen.inst.terrainChunks[0];
            //terrainMat = chunk.GetComponent<MeshRenderer>().material;
            //terrainMat.SetFloat("_TerritoryYCutoff", ElevationManager.maxElevation * ElevationManager.elevationInterval);
        }
    }

    //[HarmonyPatch(typeof(TerrainChunk), "SetTerritoryTextures")]
    //static class TerritoryTexturePatch
    //{
    //    static void Postfix(Texture2D oldTex, Texture newTex, float blendTime)
    //    {
    //        ColorManager.terrainMat?.SetTexture("_TerritoryTexOld", oldTex);
    //        ColorManager.terrainMat?.SetTexture("_TerritoryTexNew", newTex);
    //        ColorManager.terrainMat?.SetFloat("_TerritoryBlend", 1f);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "UpdateCursorPosition")]
    //static class CursorPosPatch
    //{
    //    static void Postfix(Vector4 packedCursorInfo)
    //    {
    //        ColorManager.terrainMat?.SetVector("_Cursor", packedCursorInfo);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "UpdateCursorColor")]
    //static class CursorColorPatch
    //{
    //    static void Postfix(Color color)
    //    {
    //        ColorManager.terrainMat?.SetColor("_CursorColor", color);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "UpdateDimensions")]
    //static class TerrainDimensionsPatch
    //{
    //    static void Postfix(Vector4 dimensions)
    //    {
    //        ColorManager.terrainMat?.SetVector("_TerrainDimensions", dimensions);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "UpdateHighlight")]
    //static class TerrainHighlightPatch
    //{
    //    static void Postfix(float intensity)
    //    {
    //        ColorManager.terrainMat?.SetFloat("_TerritoryPulse", intensity);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "UpdateFade")]
    //static class TerrainFadePatch
    //{
    //    static void Postfix(float fade)
    //    {
    //        ColorManager.terrainMat?.SetFloat("_TerritoryFade", fade);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "SetSnowFade")]
    //static class TerrainSnowFadePatch
    //{
    //    static void Postfix(float fade)
    //    {
    //        ColorManager.terrainMat?.SetFloat("_SnowAlpha", fade);
    //    }
    //}

    //[HarmonyPatch(typeof(TerrainChunk), "SetOverlayFade")]
    //static class TerrainOverlayFadePatch
    //{
    //    static void Postfix(float endFade)
    //    {
    //        ColorManager.terrainMat?.SetFloat("_OverlayAlpha", endFade);
    //    }
    //}


}
