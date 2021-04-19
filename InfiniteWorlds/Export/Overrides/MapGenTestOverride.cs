using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace Elevation.InfiniteWorlds
{
    /*  Generation Override
     *  Setup - no modification neccessary
     *  GenLand - possible modifications
     *  World.Generate should generate World Map
     *  Need seperate method to switch region and generate that region
     *  
     *  
     *  
     * World Generators:
     *  
     *  Regions have some data (Metadata?) assigned by main generator
     *  Regions have interpreter assigned
     *  Interpreters interpret data and generate that region only when player moves to that region
     * 
     * 
     * 
     * 
     * 
     * 
     */




    // IDEA: Async loading for smooth transitions
    // IDEA: Transport units; caravan for land, barge for water; transports peasants and resources, but controlled like a unit
    // IDEA: Seamless trasition from region to main map view, by zooming camera, with no borders, but selection highlights to indicate regions. 
    // IDEA: Map colors for players, ai, vikings, dragons, [merchant empires?] show on map
    // IDEA: Interregional Object shows on map with color, icon, see Zat Minimap for example. 

    public class GenerationOverride
    {
        public static float waterLevel = 0.5f;
    }

    [HarmonyPatch(typeof(TerrainGen), "GenerateWater")]
    public class GenerateWaterPatch
    {
        private static int desiredIslands;
        private static int desiredTiles;
        private static float expandChance = 0.1f;

        private static int tilesUsed = 0;

        private static Dictionary<int, List<Cell>> islands;


        private static void CalcSettings(World.MapSize size)
        {
            World.MapBias generatedMapsBias = World.inst.generatedMapsBias;

            desiredIslands = SRand.Range(1, 2);
            if (generatedMapsBias == World.MapBias.Island)
            {
                if (size == World.MapSize.Small)
                {
                    desiredIslands = SRand.Range(2, 4);
                }
                if (size == World.MapSize.Medium)
                {
                    desiredIslands = SRand.Range(2, 4);
                }
                if (size == World.MapSize.Large)
                {
                    desiredIslands = SRand.Range(3, 6);
                }
            }
            if (RivalKingdomSettingsUI.inst != null)
            {
                desiredIslands = Mathf.Max(desiredIslands, RivalKingdomSettingsUI.inst.AICount() + 1);
            }

            desiredTiles = World.inst.mapSizeDefs[(int)size].DesiredTiles;
        }

        static bool Prefix(World.MapSize size)
        {
            try
            {

                CalcSettings(size);

                Mod.helper.Log("overriding terrain gen");

                for (int x = 0; x < World.inst.GridWidth; x++)
                {
                    for (int z = 0; z < World.inst.GridHeight; z++)
                    {
                        if (Mathf.PerlinNoise(x, z) <= GenerationOverride.waterLevel)
                            TerrainGen.inst.SetWaterTile(x, z);
                        else
                            TerrainGen.inst.SetLandTile(x, z);
                    }
                }

                islands.Add(-1, World.inst.GetCellsData().ToList());

                Mod.Log("1");

                for (int island = 0; island < desiredIslands; island++)
                {
                    Cell random = FindIslandStart(island);

                    Expand(random, island);
                }

                Mod.Log("2");

                while (tilesUsed < desiredTiles)
                {
                    int island = SRand.Range(0, desiredIslands - 1);

                    Expand(islands[island][SRand.Range(0, islands[island].Count)], island);
                }

                Mod.Log("3");

            }
            catch(Exception ex)
            {
                Mod.Log(ex);
            }


            return false;
        }


        private static void Expand(Cell cell, int island)
        {
            cell.landMassIdx = island;
            tilesUsed++;

            Cell[] neighbors = new Cell[8];
            World.inst.GetNeighborCellsExtended(cell, ref neighbors);

            foreach(Cell neighbor in neighbors)
                if(neighbor.landMassIdx != island)
                    if (!NeighborsIsland(neighbor, island))
                        if (SRand.Range(0f, 1f) <= expandChance)
                            Expand(neighbor, island);
        }

        private static Cell FindIslandStart(int ignoreIsland)
        {
            Cell found = islands[-1][UnityEngine.Random.Range(0, islands[-1].Count - 1)];
            if (!NeighborsIsland(found, ignoreIsland))
                return found;
            else
                return FindIslandStart(ignoreIsland);
        }


        private static bool NeighborsIsland(Cell cell, int ignore)
        {
            Cell[] neighbors = new Cell[8];
            World.inst.GetNeighborCellsExtended(cell, ref neighbors);
            foreach (Cell neighbor in neighbors)
                if (neighbor.landMassIdx != -1 && neighbor.landMassIdx != ignore)
                    return false;
            return true;
        }


    }

    //public class MapGenTestOverride
    //{
    //    internal static int islandCount;
    //    internal static int estimatedUsableTiles;

    //    public static float waterLevel = 1f;
    //}

    //[HarmonyPatch(typeof(MapGenTest), "Init")]
    //internal class InitPatch
    //{
    //    static bool Prefix(int estimatedUsableTiles, int islandCount)
    //    {
    //        MapGenTestOverride.estimatedUsableTiles = estimatedUsableTiles;
    //        MapGenTestOverride.islandCount = islandCount;

    //        return false;
    //    }
    //}

    //[HarmonyPatch(typeof(MapGenTest), "WriteMap")]
    //internal class WriteMapPatch
    //{
    //    private static int width;
    //    private static int height;

    //    static bool Prefix(MapGenTest __instance, ref bool[] waterTiles, ref int[] landmassIdx)
    //    {
    //        width = (int)typeof(MapGenTest).GetField("gridWidth", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
    //        height = (int)typeof(MapGenTest).GetField("gridHeight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                Set(ref waterTiles, x, y, Mathf.PerlinNoise(x, y) <= MapGenTestOverride.waterLevel);
    //            }
    //        }

    //        return false;
    //    }

    //    private static void Set(ref bool[] array, int x, int y, bool value) => array[y * width + x] = value;
    //}
}
