using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KCModUtils.World
{
    public class WorldDestruction
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
                else if (wreckBuildings)
                {
                    WreckColumn(cell);
                    WreckSubStructure(cell);
                }
            }


            if (cell.Type != ResourceType.Water)
            {

                if (cell.Type == ResourceType.WitchHut)
                    GameObject.Destroy(global::World.inst.GetWitchHutAt(cell));

                if (cell.Type == ResourceType.Stone || cell.Type == ResourceType.IronDeposit || cell.Type == ResourceType.UnusableStone)
                    global::World.inst.RemoveStone(cell);

                GameObject cave = global::World.inst.GetCaveAt(cell);
                if (cave)
                    GameObject.Destroy(cave);


                TreeSystem.inst.DeleteTreesAt(cell);
                cell.Type = ResourceType.None;
            }
        }

        /// <summary>
        /// Destroys a column of stackable buildings (usually walls) while leaving rubble
        /// </summary>
        /// <param name="cell">cell to target</param>
        public static void WreckColumn(Cell cell)
        {
            List<Rubble.BuildingState> states;
            states = cell.BottomStructure.GetComponent<Rubble>().GetBuildingStates();
            global::World.inst.WreckColumn(cell, states);
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
                    global::World.inst.VanishBuilding(structure);
                }
            }
            catch (Exception ex)
            {
                ModUtilsMain.Helper.Log(ex.Message + "\n" + ex.StackTrace);
            }

        }

        /// <summary>
        /// Destroys any sub-structure, which is underlying buildings, like moats, piers, or bridges, and leaves behind rubble. 
        /// </summary>
        /// <param name="cell"></param>
        public static void WreckSubStructure(Cell cell)
        {
            foreach (Building structure in cell.SubStructure)
            {
                global::World.inst.WreckBuilding(structure);
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
                global::World.inst.VanishBuilding(structure);
            }
        }


    }
}
