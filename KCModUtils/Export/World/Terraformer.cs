using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCModUtils.World
{
    public class Terraformer
    {
        

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
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    break;
                case ResourceType.Wood:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    TreeSystem.inst.GrowTree(cell);
                    break;
                case ResourceType.Stone:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.Stone);
                    break;
                case ResourceType.Water:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetWaterTile(cell);
                    break;
                case ResourceType.UnusableStone:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.UnusableStone);
                    break;
                case ResourceType.IronDeposit:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.PlaceStone((int)cell.Center.x, (int)cell.Center.z, ResourceType.IronDeposit);
                    break;
                case ResourceType.EmptyCave:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.AddEmptyCave((int)cell.Center.x, (int)cell.Center.z);
                    break;
                case ResourceType.WolfDen:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.AddWolfDen((int)cell.Center.x, (int)cell.Center.z);
                    break;
                case ResourceType.WitchHut:
                    WorldDestruction.AnnhiliateCell(cell, vanishBuildings, wreckBuildings);
                    SetLandTile(cell);
                    global::World.inst.AddWitchHut((int)cell.Center.x, (int)cell.Center.z);
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
            TerrainGen.inst.SetLandTile((int)cell.Center.x, (int)cell.Center.z);
            cell.Type = ResourceType.None;
            TerrainGen.inst.SetFertileTile((int)cell.Center.x, (int)cell.Center.z, fertility);
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
            if (!invalid)
            {

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
            if (cell.landMassIdx > 0 && cell.landMassIdx < global::World.inst.NumLandMasses)
            {
                try
                {
                    global::World.inst.cellsToLandmass[cell.landMassIdx].Remove(cell);
                }
                catch (Exception ex)
                {
                    ModUtilsMain.Helper.Log(ex.Message + "\n" + ex.StackTrace);
                }
            }

            cell.landMassIdx = landmassIdx;
            global::World.inst.cellsToLandmass[landmassIdx].Add(cell);
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


    }
}
