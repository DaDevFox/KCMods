using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
{
    static class Util
    {
        /// <summary>
        /// Completely destroys and rocks, buildings, or signs of life on the cell. 
        /// </summary>
        /// <param name="cell">The cell to destroy</param>
        /// <param name="vanishBuildings">Wether or not to destroy buidlings on the cell without leaving rubble</param>
        /// <param name="wreckBuildings">Wether or not to destroy buildings on the cell while leaving rubble</param>
        public static void AnnhiliateCell(Cell cell, bool vanishBuildings = true, bool wreckBuildings = false)
        {
            if (cell.OccupyingStructure.Count > 0)
            {
                if (vanishBuildings)
                {
                    VanishColumn(cell);
                    VanishSubStructure(cell);
                }
                else if(wreckBuildings)
                {
                    WreckColumn(cell);
                    WreckSubStructure(cell);
                }
            }

            
            if (cell.Type != ResourceType.Water) {
                
                if (cell.Type == ResourceType.WitchHut)
                    GameObject.Destroy(World.inst.GetWitchHutAt(cell));
                
                if (cell.Type == ResourceType.Stone || cell.Type == ResourceType.IronDeposit || cell.Type == ResourceType.UnusableStone)
                    World.inst.RemoveStone(cell);
                
                GameObject cave = World.inst.GetCaveAt(cell);
                if (cave)
                    GameObject.Destroy(cave);
                

                TreeSystem.inst.DeleteTreesAt(cell);
                cell.Type = ResourceType.None;
            }
        }

        /// <summary>
        /// Sets the cells type, while taking the neccesary measures to do this without breaking the game as much as possible, Note not all of this works, should be fixed in future. 
        /// </summary>
        /// <param name="cell">Cell to modify</param>
        /// <param name="type">then new type of cell</param>
        /// <param name="vanishBuildings">Wether or not to destroy buidlings on the cell without leaving rubble</param>
        /// <param name="wreckBuildings">Wether or not to destroy buildings on the cell while leaving rubble</param>
        public static void SetCellType(Cell cell, ResourceType type, bool vanishBuildings = true, bool wreckBuildings = true) 
        {
            switch (type)
            {
                case ResourceType.None:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    break;
                case ResourceType.Wood:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    TreeSystem.inst.GrowTree(cell);
                    break;
                case ResourceType.Stone:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.Stone);
                    break;
                case ResourceType.Water:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetWaterTile(cell);
                    break;
                case ResourceType.UnusableStone:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.UnusableStone);
                    break;
                case ResourceType.IronDeposit:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.IronDeposit);
                    break;
                case ResourceType.EmptyCave:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.AddEmptyCave((int)cell.Center.x, (int)cell.Center.z);
                    break;
                case ResourceType.WolfDen:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.AddWolfDen((int)cell.Center.x, (int)cell.Center.z);
                    break;
                case ResourceType.WitchHut:
                    Util.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    Util.SetLandTile(cell);
                    World.inst.AddWitchHut((int)cell.Center.x, (int)cell.Center.z);
                    break;
                default:
                    break;
            }
            cell.StorePostGenerationType();
        }

        /// <summary>
        /// Set's a cell to a land tile, while allowing height changes
        /// </summary>
        /// <param name="cell">cell to change</param>
        /// <param name="fertility">new fertility of the cell</param>
        /// <param name="height">new height of the cell</param>
        public static void SetLandTile(Cell cell, int fertility = 1, float height = 0f) 
        {
            TerrainGen.inst.SetLandTile((int)cell.Center.x,(int)cell.Center.z);
            cell.Type = ResourceType.None;
            TerrainGen.inst.SetFertileTile((int)cell.Center.x, (int)cell.Center.z,fertility);
            TerrainGen.inst.SetTileHeight(cell, height);
        }

        /// <summary>
        /// Sets a cell to a water tile, adjusting to deep or shallow water depending on the height. 
        /// </summary>
        /// <param name="cell">cell to change</param>
        /// <param name="height">new height of the water, should be below -0.25f. </param>
        public static void SetWaterTile(Cell cell, float height = -0.5f) 
        {
            bool invalid = height > TerrainGen.waterHeightTestThresold || cell.Type == ResourceType.Water;
            if (!invalid) {

                TerrainGen.inst.SetWaterTile((int)cell.Center.x, (int)cell.Center.z);
                TerrainGen.inst.SetTileHeight(cell, height);

                if (height < TerrainGen.waterHeightDeep) 
                {
                    cell.deepWater = true;
                }
                else
                {
                    cell.deepWater = false;
                }
            }
        }

        /// <summary>
        /// Safely changes the landmass of a cell
        /// </summary>
        /// <param name="cell">cell to change</param>
        /// <param name="landmassIdx">new landmass of the cell</param>
        public static void SetCellLandmass(Cell cell, int landmassIdx) 
        {
            if (cell.landMassIdx > 0 && cell.landMassIdx < World.inst.NumLandMasses)
            {
                try
                {
                    World.inst.cellsToLandmass[cell.landMassIdx].Remove(cell);
                }
                catch (Exception ex)
                {
                    Mod.helper.Log(ex.Message + "\n" + ex.StackTrace);
                }
            }

            cell.landMassIdx = landmassIdx;
            World.inst.cellsToLandmass[landmassIdx].Add(cell);
        }

        /// <summary>
        /// Destroys a column of stackable buildings (usually walls) while leaving rubble
        /// </summary>
        /// <param name="cell">cell to target</param>
        public static void WreckColumn(Cell cell)
        {
            List<Rubble.BuildingState> states;
            states = cell.BottomStructure.GetComponent<Rubble>().GetBuildingStates();
            World.inst.WreckColumn(cell, states);
        }

        /// <summary>
        /// Destroys a column of stackable buildings (usually walls) without leaving rubble
        /// </summary>
        /// <param name="cell"></param>
        public static void VanishColumn(Cell cell)
        {
            try
            {
                foreach (Building structure in cell.OccupyingStructure)
                {
                    World.inst.VanishBuilding(structure);
                }
            }
            catch (Exception ex)
            {
                Mod.helper.Log(ex.Message + "\n" + ex.StackTrace);
            }
            
        }

        /// <summary>
        /// Destroys any sub-structure, which is underlying buildings, like moats, piers, or bridges, and leaves behind rubble. 
        /// </summary>
        /// <param name="cell"></param>
        public static void WreckSubStructure(Cell cell)
        {
            foreach(Building structure in cell.SubStructure)
            {
                World.inst.WreckBuilding(structure);
            }
        }

        /// <summary>
        /// Destroys any sub-structure, which is underlying buildings, like moats, piers, or bridges, but doesn't leave behind rubble. 
        /// </summary>
        /// <param name="cell"></param>
        public static void VanishSubStructure(Cell cell)
        {
            foreach (Building structure in cell.SubStructure)
            {
                World.inst.VanishBuilding(structure);
            }
        }

        /// <summary>
        /// Gets the landmass on which the player started
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerStartLandmass()
        {
            foreach (int landmass in Player.inst.PlayerLandmassOwner.ownedLandMasses.data)
            {
                if (Player.inst.DoesAnyBuildingHaveUniqueNameOnLandMass("keep", landmass))
                {
                    return landmass;
                }
            }
            return 0;
        }

        /// <summary>
        /// Returns a random number between 0 and 1. (Obsolete, I thought this was impossible with SRand.Range(0f,1f) lol)
        /// </summary>
        /// <returns></returns>
        public static float Randi()
        {
            return SRand.Range(0f, 10f) / 10f;
        }

        /// <summary>
        /// A linear weighted random detremination starting at a min value and ending on a max value, with a given increment to define the step of the randomness, useful when defining chances of pickups or events. 
        /// </summary>
        /// <param name="min">The minimum value of the weighted random, must be less than the max</param>
        /// <param name="max">The maximum value of the weighted random, must be greater than the min</param>
        /// <param name="increment">The step of the weighted random</param>
        /// <returns></returns>
        public static float LinearWeightedRandom(float min, float max, float increment = 1f)
        {
            List<float> options = new List<float>();

            for (float i = max; i > min; i -= increment)
            {
                float num = max - i;

                for (float k = 0; k < num; k += increment)
                {

                    options.Add(i);
                }
            }

            return options[SRand.Range(0, options.Count - 1)];
        }

        /// <summary>
        /// Broken do not use
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="weight"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        public static float WeightedRandom(float min, float max, float weight = 0f, float increment = 1f)
        {
            List<float> options = new List<float>();

            bool valid = max > min;
            

            if (valid)
            {

                DebugExt.Log("1", true);

                float total = Util.RoundToFactor(max - min, increment);
                int _break = (int)total + 1;


                for (float i = min; i < max; i += increment)
                {


                    
                    float num = Util.RoundToFactor(-Math.Abs(weight - i) + total, increment);

                    DebugExt.Log("1-2: " + total, true);
                    DebugExt.Log("1-2: " + i, true);
                    DebugExt.Log("1-2: " + -Math.Abs(weight - i), true);

                    int counter = 0;

                    if (num != 0 && num != increment)
                    {
                        for (float k = 0; k < num; k += increment)
                        {
                            options.Add(Util.RoundToFactor(i, increment));
                            counter++;
                            if(counter > _break)
                            {
                                options.Add(Util.RoundToFactor(i, increment));
                                break;
                            }
                        }
                    }
                    else
                    {
                        options.Add(Util.RoundToFactor(i, increment));
                    }

                    DebugExt.Log("1-3: " + num, true);
                }

                DebugExt.Log(options.ToString(), true);

                return SRand.Range(0, options.Count);
            }
            else
            {
                DebugExt.Log("Invalid arguments: " + min.ToString() + ", " + max.ToString() + ", " + weight.ToString() + ", " + increment.ToString());
                return min;
            }
        }

        /// <summary>
        /// Possibly broken, hasn't been tested, don't consider reliable. Creates a linear weight pick emitted from a weight value, for example, a weight of 4 between 1 and 10 would mean you are most likely to get 4, and the chances lowering going in either direction of the number line. 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="weight">The weight emitter</param>
        /// <returns></returns>
        public static int WeightedRandom(int min, int max, int weight = 0)
        {
            List<int> options = new List<int>();

            bool valid = max > min;
            if (valid)
            {
                for (int i = min; i < max; i++)
                {

                    

                    for (int k = 0; k < Math.Abs(i - weight); k++)
                    {
                        options.Add(i);
                    }
                }

                DebugExt.Log(options.ToString(), true);

                return SRand.Range(0, options.Count);
            }
            else
            {
                return min;
            }

            
        }

        /// <summary>
        /// Rounds a number to a factor, I.E. Rounding 450.234324 to a facter of 0.1 would result in 450.2
        /// </summary>
        /// <param name="a"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static float RoundToFactor(float a, float factor)
        {
            return Mathf.Round(a/factor) * factor;
        }


    }
}
