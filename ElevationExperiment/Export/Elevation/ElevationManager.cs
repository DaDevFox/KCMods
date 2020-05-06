using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace ElevationExperiment
{
    static class ElevationManager
    {
        private static Dictionary<string,CellMark> cellMarkLookup = new Dictionary<string, CellMark>();

        public static float elevationInterval = 0.5f;
        public static int maxElevation = 8;
        public static int minElevation = 0;

        

        private static float elevationPathfindingCost = 10f;

        public static List<CellMark> GetAll()
        {
            return cellMarkLookup.Values.ToList();
        }


        public static bool TryProcessElevationChange(Cell cell, int tierChange)
        {
            CellMark mark = GetCellMark(cell);
            bool valid = ValidElevation(mark.elevationTier + tierChange) && ValidTileForElevation(cell);
            
            if (valid)
            {
                mark.elevationTier += tierChange;
                RefreshTerrain();
            }

            return valid;
        }


        public static bool TrySetElevation(Cell cell, int elevationTier)
        {
            CellMark mark = GetCellMark(cell);
            bool valid = ValidElevation(elevationTier) && ValidTileForElevation(cell);

            if (valid)
                mark.elevationTier = elevationTier;
            

            return valid;
        }
        
        public static void Reset()
        {
            foreach (CellMark mark in cellMarkLookup.Values)
            {
                mark.OnDispose();
            }

            cellMarkLookup.Clear();
            Mod.dLog("Cell marking cleared");
        }

        public static void SetupCellMarks()
        {
            Reset();
            for(int i = 0; i < World.inst.NumLandMasses; i++)
            {
                foreach(Cell cell in World.inst.cellsToLandmass[i].data)
                {
                    if (cell != null)
                    {
                        CellMark mark = new CellMark(cell);
                        cellMarkLookup.Add(GetCellMarkID(cell),mark);
                    }
                }
            }
            Mod.dLog("Cell marking setup");
        }

        public static void RefreshTerrain()
        {
            try
            {
                foreach (CellMark mark in cellMarkLookup.Values)
                {
                    
                    Terraformer.SetCellHeight(mark.cell, mark.Elevation);
                    
                }

                UpdateMarks();
                UpdatePatches();

                TerrainGen.inst.FinalizeChanges();
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }

            DebugExt.dLog("terrain refreshed");
        }


        static void UpdatePatches()
        {
            Patches.TreeSystemPatch.UpdateTrees();
            Patches.RockPatch.UpdateStones();
            Patches.WitchHutPatch.UpdateWitchHuts();
            Patches.WoflDenPatch.UpdateWolfDens();
            Patches.EmptyCavePatch.UpdateEmptyCaves();
        }


        public static void UpdateMarks()
        {
            foreach(CellMark mark in cellMarkLookup.Values)
            {
                mark.UpdateMesh();
                mark.UpdatePathing();
            }
        }

        public static void PushMeshUpdate()
        {
            foreach (CellMark mark in cellMarkLookup.Values)
            {
                mark.UpdateMesh(true);
            }
        }



        public static int GetPathfindingCostForElevationTier(int tier)
        {
            return (int)((float)tier * elevationPathfindingCost);
        }



        public static bool ValidTileForElevation(Cell cell)
        {
            return cell.Type != ResourceType.Water;
        }

        public static CellMark GetCellMark(Cell cell)
        {
            return cellMarkLookup.ContainsKey(GetCellMarkID(cell)) ? cellMarkLookup[GetCellMarkID(cell)] : null;
        }

        private static bool ValidElevation(int elevationTier)
        {
            return elevationTier >= minElevation && elevationTier <= maxElevation;
        }


        public static string GetCellMarkID(Cell cell)
        {
            return cell.x.ToString() + "_" + cell.z.ToString();
        }





    }
}
