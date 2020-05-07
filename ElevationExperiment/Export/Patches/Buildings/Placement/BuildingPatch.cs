using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace ElevationExperiment.Patches
{




    [HarmonyPatch(typeof(World), "PlaceInternal")]
    public class BuildingPlacePatch
    {
        static Dictionary<string, float> buildingRealStackHeights = new Dictionary<string, float>() 
        {
            {"castleblock", 0.5f },
            {"woodcastleblock",0.5f },
            {"gate",1.5f },
            {"stonegate",1.5f },
            {"castlestairs",0.5f }
        };

        static void Postfix(Building PendingObj)
        {
            try
            {
                if (UnevenTerrain(PendingObj))
                {
                    DebugExt.dLog("Building on uneven terrain");
                }
                else
                {
                    Vector3 pos = PendingObj.transform.localPosition;
                    Cell cell = PendingObj.GetCell();
                    CellMark mark = ElevationManager.GetCellMark(cell);
                    
                    float stackHeight = 0;
                    if (PendingObj.Stackable)
                    {
                         stackHeight = GetStackHeightOfBuildingAtIndex(cell,cell.OccupyingStructure.IndexOf(PendingObj));
                    }
                    if(PendingObj.CategoryName == "projectiletopper")
                    {
                        stackHeight = GetStackHeightTotal(cell);
                    }

                    if(mark != null)
                    {
                        PendingObj.transform.localPosition = new Vector3(pos.x, mark.Elevation + stackHeight, pos.z);
                        PendingObj.UpdateShaderHeight();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }

        public static bool UnevenTerrain(Building b)
        {
            Cell firstCell = b.GetCell();
            bool flag = false;
            CellMark firstMark = ElevationManager.GetCellMark(firstCell);
            if (firstCell != null && firstMark != null)
            {
                
                int elevationTier = firstMark.elevationTier;
                
                b.ForEachTileInBounds(delegate (int x, int y, Cell cell)
                {
                    CellMark mark = ElevationManager.GetCellMark(cell);
                    if (mark != null)
                    {
                        if (mark.elevationTier != elevationTier)
                        {
                            flag = true;
                        }
                    }
                });
            }
            
            return flag;
        }



        public static int GetStackPosOfBuildingAtIndex(Cell c, int idx)
        {
            int count = 0;
            int i = 0;
            while(count < c.OccupyingStructure.Count)
            {
                if(count == idx)
                {
                    return i;
                }

                i += c.OccupyingStructure[i].StackHeight;
                count++;
            }
            return -1;
        }

        public static float GetStackHeightOfBuildingAtIndex(Cell c, int idx)
        {
            int count = 0;
            float i = 0;
            while (count < c.OccupyingStructure.Count)
            {
                float stackRealHeight = 0f;

                
                if (buildingRealStackHeights.ContainsKey(c.OccupyingStructure[count].UniqueName))
                {
                    stackRealHeight = buildingRealStackHeights[c.OccupyingStructure[count].UniqueName];
                }
                

                if(count == idx)
                {
                    return i;
                }


                i += stackRealHeight;
                count++;
            }
            return 0;
        }

        public static float GetAbsoluteStackHeightOfBuildingAtIndex(Cell c, int idx)
        {
            CellMark mark = ElevationManager.GetCellMark(c);
            int count = 0;
            float i = 0;
            if (mark != null)
                i = mark.Elevation;
            
            while (count < c.OccupyingStructure.Count)
            {
                float stackRealHeight = 0f;


                if (buildingRealStackHeights.ContainsKey(c.OccupyingStructure[count].UniqueName))
                {
                    stackRealHeight = buildingRealStackHeights[c.OccupyingStructure[count].UniqueName];
                }


                if (count == idx)
                {
                    return i;
                }


                i += stackRealHeight;
                count++;
            }
            return 0;
        }

        public static float GetStackHeightTotal(Cell c)
        {
            int count = 0;
            float i = 0;
            while (count < c.OccupyingStructure.Count)
            {
                float stackRealHeight = 0f;

                if (c.OccupyingStructure[count].Stackable)
                {
                    if (buildingRealStackHeights.ContainsKey(c.OccupyingStructure[count].UniqueName))
                    {
                        stackRealHeight = buildingRealStackHeights[c.OccupyingStructure[count].UniqueName];
                    }
                }

                i += stackRealHeight;
                count++;
            }
            return i;
        }

        public static float GetAbsoluteStackHeightTotal(Cell c)
        {
            CellMark mark = ElevationManager.GetCellMark(c);
            
            float i = 0;
            if (mark != null)
                i = mark.Elevation;

            bool flag = false;

            for (int count = 0; count < c.OccupyingStructure.Count; count++)
            {
                float stackRealHeight = 0f;

                

                if (c.OccupyingStructure[count].Stackable)
                {
                    if (buildingRealStackHeights.ContainsKey(c.OccupyingStructure[count].UniqueName))
                    {
                        DebugExt.dLog("stackable " + buildingRealStackHeights[c.OccupyingStructure[count].UniqueName].ToString());
                        stackRealHeight = buildingRealStackHeights[c.OccupyingStructure[count].UniqueName];
                        flag = true;
                    }
                }

                i += stackRealHeight;
            }
            if(flag)
                return i;
            else
                return 0;
        }

    }
}