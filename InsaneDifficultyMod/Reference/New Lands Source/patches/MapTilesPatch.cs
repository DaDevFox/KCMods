using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace New_Lands.patches
{

   


    // MAP GENERATION PATCH
    // This patch creates the land tiles and manipulates
    // the tree generation.
    [HarmonyPatch(typeof(MapGenTest))]
    [HarmonyPatch("Init")]
    static class MapTilesPatch
    {
        

        

        static bool Prefix(MapGenTest __instance, ref int w, ref int h,
           ref int desiredTiles, ref int islandCount)
        {
            // Calculate the amount of tiles that should be placed onto 
            // the grid.
            Main.Generator.UpdateValues();
            desiredTiles = Main.Generator.EstimatedUseableTiles;
            islandCount = Main.Generator.IslandCounts;
            TreeGrowth.inst.MaxTreesOnMap = desiredTiles;

            MapGenTest MapGen = (MapGenTest)GameObject.FindObjectOfType(typeof(MapGenTest));
            MapGen.riverStepPunch = Main.Generator.RiverStep; // original was 2f
            MapGen.riverPerpJitter = Main.Generator.RiverJitter; // original is 1f 

            MapGen.minIslandArea = Main.Generator.minIslandArea; // original is 64f
            MapGen.minCircleArea = Main.Generator.minCircleArea; // original is 80f

            MapGen.edgeHoleStep = Main.Generator.EdgeHole; // int, original is 5
            MapGen.waterFreq = Main.Generator.WaterFreq;

           

            /*
            var NumIslands = typeof(MapGenTest).GetField("numIslands", BindingFlags.Instance | BindingFlags.NonPublic);
            NumIslands.SetValue(MapGenTest.this, Main.Grid.current);
            var WorldHeight = typeof(World).GetField("gridHeight", BindingFlags.Instance | BindingFlags.NonPublic);
            WorldHeight.SetValue(World.inst, Main.Grid.current);
            */


            //treeAimToSpawn = ScriptEntry.treeAim;
            
            return true;
        }

        static void PostFix()
        {

        

        }


    }



    // Shape patch
    // This patch creates the land tiles and manipulates
    // the tree generation.
    [HarmonyPatch(typeof(TerrainGen))]
    [HarmonyPatch("AddShape")]
    static class ShaperPatch
    {
        // Slider value for global height noise
        static void Postfix()
        {
            try
            {

                int NewX = Main.Grid.current;
                int NewY = Main.Grid.current;

                float PerlinGen = 0.3f; // 0f is original value

                int num = NewX * 2 + 1;
                int num2 = NewY * 2 + 1;
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        float vertY = TerrainGen.inst.GetVertY(i, j);
                        if (vertY > TerrainGen.waterHeightShallow)
                        {
                            TerrainGen.inst.SetVert(i, j, vertY + Mathf.PerlinNoise((float)i / (float)num * 10f, (float)j / (float)num2 * 10f) * PerlinGen);
                        }
                    }
                }
            }
            catch (ChunkException ex)
            {
                // This is going to cause a lot of trouble.
                Debug.LogError(ex.ToString()); // this.HandleException(ex);     
            }

        }
    }

    // Noise patch
    [HarmonyPatch(typeof(TerrainGen))]
    [HarmonyPatch("AddNoise")]
    [HarmonyPatch(new Type[] {  })]

    static class NoisePatch
    {
        // Slider value for local height noise
        static void Postfix()
        {
            try
            {
                int NewX = Main.Grid.current;
                int NewY = Main.Grid.current;
                float NoiseIntense = 0.03f;  //original 0.01f


                int num = NewX * 2 + 1;
                int num2 = NewY * 2 + 1;
                for (int i = 0; i < num; i++)
                {
                    for (int j = 0; j < num2; j++)
                    {
                        TerrainGen.inst.SetVert(i, j, TerrainGen.inst.GetVertY(i, j)
                            + SRand.Range(-NoiseIntense, NoiseIntense));
                    }
                }
            }
            catch (ChunkException ex)
            {
                // This is going to cause a lot of trouble.
                Debug.LogError(ex.ToString()); // this.HandleException(ex);     
            }


        }



    }
}



