using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace Elevation.Patches
{
    [HarmonyPatch(typeof(World), "SetupStoneForCell")]
    class RockPatch
    {
        static void Postfix(Cell cell)
        {
            if (cell == null)
                return;

            CellMeta meta = Grid.Cells.Get(cell);

            if (meta == null || cell.Models == null)
                return;

            foreach (GameObject obj in cell.Models)
            {
                if(obj != null)
                    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + meta.Elevation, obj.transform.position.z);
            }
            World.inst.CombineStone();
           
        }


        struct TrackedRock
        {
            public Cell cell;
            public ResourceType type;
        }


        public static void UpdateCell(Cell cell, bool recombine = true)
        {
            ResourceType type = cell.Type;
            if (cell.Type == ResourceType.Stone || cell.Type == ResourceType.UnusableStone || cell.Type == ResourceType.IronDeposit)
            {
                World.inst.RemoveStone(cell, recombine);
                World.inst.PlaceStone(cell, type);
            }
        }

        public static void UpdateStones()
        {
            List<TrackedRock> tracked = new List<TrackedRock>();
            foreach (ArrayExt<Cell> landmass in World.inst.cellsToLandmass)
            {
                foreach (Cell _cell in landmass.data)
                {
                    if (_cell != null)
                    {
                        if(_cell.Type == ResourceType.Stone || _cell.Type == ResourceType.IronDeposit || _cell.Type == ResourceType.UnusableStone)
                        {
                            tracked.Add(new TrackedRock() {  cell = _cell, type = _cell.Type});
                        }
                    }
                }
            }

            foreach(TrackedRock rock in tracked)
            {
                World.inst.RemoveStone(rock.cell);
                World.inst.PlaceStone(rock.cell, rock.type);
            }
        }

        static void Finalizer(Exception __exception)
        {
            DebugExt.HandleException(__exception);
        }

    }

    [HarmonyPatch(typeof(World), "AddWitchHut")]
    class WitchHutPatch
    {
        static void Postfix(WitchHut __result)
        {
            Cell cell = World.inst.GetCellData(__result.transform.position);
            if (cell != null)
            {
                CellMeta meta = Grid.Cells.Get(cell);
                if (meta != null)
                {
                    __result.transform.position = new Vector3(__result.transform.position.x, meta.Elevation, __result.transform.position.z);
                }
            }
        }


        public static void UpdateCell(Cell cell)
        {
            WitchHut witch = World.inst.GetWitchHutAt(cell);
            if (witch)
            {
                GameObject.Destroy(witch);

                World.inst.AddWitchHut(cell.x, cell.z);
            }
        }

        public static void UpdateWitchHuts()
        {
            List<Cell> tracked = new List<Cell>();
            foreach (ArrayExt<Cell> landmass in World.inst.cellsToLandmass)
            {
                foreach (Cell cell in landmass.data)
                {
                    if (cell != null)
                    {
                        if (cell.Type == ResourceType.WitchHut)
                        {
                            tracked.Add(cell);
                        }
                    }
                }
            }

            foreach (Cell cell in tracked)
            {
                WitchHut witch = World.inst.GetWitchHutAt(cell);
                if (witch)
                {
                    GameObject.Destroy(witch);

                    World.inst.AddWitchHut(cell.x, cell.z);
                }
            }
        }
    }



    [HarmonyPatch(typeof(World), "AddWolfDen")]
    class WoflDenPatch
    {
        static void Postfix(WolfDen __result)
        {
            Cell cell = World.inst.GetCellData(__result.transform.position);
            if (cell != null)
            {
                CellMeta meta = Grid.Cells.Get(cell);
                if (meta != null)
                {
                    __result.transform.position = new Vector3(__result.transform.position.x, meta.Elevation, __result.transform.position.z);
                }
            }
        }

        public static void UpdateCell(Cell cell)
        {
            GameObject caveObj = World.inst.GetCaveAt(cell);
            if (!caveObj)
                return;

            WolfDen cave = caveObj.GetComponent<WolfDen>();
            if (cave)
            {
                GameObject.Destroy(caveObj);

                World.inst.AddWolfDen(cell.x, cell.z);
            }
        }

        public static void UpdateWolfDens()
        {
            List<Cell> tracked = new List<Cell>();
            foreach (ArrayExt<Cell> landmass in World.inst.cellsToLandmass)
            {
                foreach (Cell cell in landmass.data)
                {
                    if (cell != null)
                    {
                        if (cell.Type == ResourceType.WolfDen)
                        {
                            tracked.Add(cell);
                        }
                    }
                }
            }

            foreach (Cell cell in tracked)
            {
                GameObject cave = World.inst.GetCaveAt(cell);
                if (cave && cell.Type == ResourceType.WolfDen)
                {
                    GameObject.Destroy(cave);

                    World.inst.AddWolfDen(cell.x, cell.z);
                }
            }
        }
    }


    [HarmonyPatch(typeof(World), "AddEmptyCave")]
    class EmptyCavePatch
    {
        static void Postfix(EmptyCave __result)
        {
            Cell cell = World.inst.GetCellData(__result.transform.position);
            if (cell != null)
            {
                CellMeta meta = Grid.Cells.Get(cell);
                if (meta != null)
                {
                    __result.transform.position = new Vector3(__result.transform.position.x, meta.Elevation, __result.transform.position.z);
                }
            }
        }


        public static void UpdateCell(Cell cell)
        {
            GameObject caveObj = World.inst.GetCaveAt(cell);
            if (!caveObj)
                return;

            EmptyCave cave = caveObj.GetComponent<EmptyCave>();
            if (cave)
            {
                GameObject.Destroy(caveObj);

                World.inst.AddEmptyCave(cell.x, cell.z);
            }
        }

        public static void UpdateEmptyCaves()
        {
            List<Cell> tracked = new List<Cell>();
            foreach (ArrayExt<Cell> landmass in World.inst.cellsToLandmass)
            {
                foreach (Cell cell in landmass.data)
                {
                    if (cell != null)
                    {
                        if (cell.Type == ResourceType.EmptyCave)
                        {
                            tracked.Add(cell);
                        }
                    }
                }
            }

            foreach (Cell cell in tracked)
            {
                UpdateCell(cell);
            }
        }
    }
}
