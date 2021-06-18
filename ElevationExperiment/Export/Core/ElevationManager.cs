using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;
using Harmony;
using System.Collections;

namespace Elevation
{
    

    /// <summary>
    /// Provides data about elevation and functions to manipulate elevation
    /// </summary>
    public static class ElevationManager
    {
        public static float elevationInterval { get; } = 0.5f;
        public static int maxElevation { get; } = 8;
        public static int minElevation { get; } = 0;

        public static float elevationPathfindingCost { get; } = 10f;

        /// <summary>
        /// Changes a tiles elevation by <c>tierChange</c>
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="tierChange"></param>
        /// <returns></returns>
        public static bool TryProcessElevationChange(Cell cell, int tierChange)
        {
            CellMeta meta = Grid.Cells.Get(cell);
            bool valid = ValidElevation(meta.elevationTier + tierChange) && ValidTileForElevation(cell);
            
            if (valid)
            {
                meta.elevationTier += tierChange;
                
                RefreshTile(cell, true);
                WorldRegions.MarkDirty(cell);

                Cell[] neighbors = World.inst.GetNeighborCells(cell);
                foreach (Cell neighbor in neighbors)
                {
                    RefreshTile(neighbor);
                    WorldRegions.MarkDirty(neighbor);
                }

                WorldRegions.UpdateDirty();
            }

            return valid;
        }

        /// <summary>
        /// Sets the elevation of a tile but doesn't notify other systems of the change.  <para>see also <seealso cref="UpdateElevation(Cell, int)"/> and <seealso cref="RefreshTile(Cell, bool)"/></para>
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="elevationTier"></param>
        /// <returns></returns>
        public static bool SetElevation(Cell cell, int elevationTier)
        {
            CellMeta meta = Grid.Cells.Get(cell);
            bool valid = ValidElevation(elevationTier) && ValidTileForElevation(cell);

            if (!meta)
                valid = false;

            if (valid)
                meta.elevationTier = elevationTier;

            return valid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="elevationTier"></param>
        /// <returns></returns>
        public static bool UpdateElevation(Cell cell, int elevationTier)
        {
            bool valid = SetElevation(cell, elevationTier);

            if(valid)
                RefreshTile(cell);

            return valid;
        }

        /// <summary>
        /// Updates a single CellMeta and the objects on that cell that will be affected by the elevation change
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="forced"></param>
        public static void RefreshTile(Cell cell, bool forced = false)
        {
            UpdateCellMetaForTile(cell, forced);
            UpdatePatchesForTile(cell);
        }

        /// <summary>
        /// Updates all CellMetas and objects affected by any elevation change on the map
        /// </summary>
        /// <param name="forced"></param>
        public static void RefreshTerrain(bool forced = false)
        {

            UpdateCellMetas(forced);
            UpdatePatches();

            DebugExt.dLog("terrain refreshed");
        }

        /// <summary>
        /// Updates any objects on a cell that must be modified to accomodate for elevation. 
        /// </summary>
        /// <param name="cell"></param>
        public static void UpdatePatchesForTile(Cell cell)
        {
            Patches.TreeSystemPatch.UpdateCell(cell);
            Patches.RockPatch.UpdateCell(cell);
            Patches.WitchHutPatch.UpdateCell(cell);
            Patches.WoflDenPatch.UpdateCell(cell);
            Patches.EmptyCavePatch.UpdateCell(cell);
        }

        /// <summary>
        /// Updates any objects on all cells that must be modified to accomodate for elevation. 
        /// </summary>
        public static void UpdatePatches()
        {
            Patches.TreeSystemPatch.UpdateTrees();
            Patches.RockPatch.UpdateStones();
            Patches.WitchHutPatch.UpdateWitchHuts();
            Patches.WoflDenPatch.UpdateWolfDens();
            Patches.EmptyCavePatch.UpdateEmptyCaves();
        }

        /// <summary>
        /// Updates only the visuals of an elevation block on a tile
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="forced">wether or not to update, even when the elevation of the tile has not changed since the last update</param>
        public static void UpdateCellMetaForTile(Cell cell, bool forced = false)
        {
            CellMeta meta = Grid.Cells.Get(cell);
            if(meta)
            {
                meta.UpdateVisuals(forced);
                meta.UpdatePathing();
            }
        }

        /// <summary>
        /// Updates only the visuals of all elevation blocks in the world
        /// </summary>
        /// <param name="forced"></param>
        public static void UpdateCellMetas(bool forced = false)
        {
            foreach(CellMeta meta in Grid.Cells)
            {
                meta.UpdateVisuals(forced);
                meta.UpdatePathing();
            }
        }

        /// <summary>
        /// Updates the positioning of all buildings in the world
        /// </summary>
        public static void UpdateBuildings()
        {
            foreach(BuildingMeta meta in Grid.Buildings)
                BuildingFormatter.UpdateBuilding(meta.building);
        }

        /// <summary>
        /// Returns wether the given tile can support elevation
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static bool ValidTileForElevation(Cell cell) => cell.Type != ResourceType.Water || cell.BottomStructureIs(World.moatHash);

        /// <summary>
        /// Returns wether the given elevation is valid
        /// </summary>
        /// <param name="elevationTier"></param>
        /// <returns></returns>
        private static bool ValidElevation(int elevationTier) => elevationTier >= minElevation && elevationTier <= maxElevation;

        /// <summary>
        /// Clamps the Vector3's y coordinate to an elevation tier
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector3 ClampPosToTier(Vector3 pos) => new Vector3(pos.x, ClampPosToTier(pos.y), pos.z);

        /// <summary>
        /// Clamps the y coordinate to an elevation tier
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int ClampPosToTier(float y) => Mathf.CeilToInt(y / ElevationManager.elevationInterval);
    }
}
