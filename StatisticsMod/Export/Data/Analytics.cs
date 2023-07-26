using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code;
using Assets.Code.Interface;

namespace StatisticsMod.Data
{
    class Analytics
    {
        public static int GetPopulationForLandmass(int landmassIdx)
        {
            return World.inst.GetVillagersForLandMass(landmassIdx).Count;
        }

        public static int GetPlayerKingdomPopulation()
        {
            int num = 0;

            for(int i = 0; i < World.inst.NumLandMasses; i++)
            {
                if (Player.inst.LandMassIsAPlayerLandMass(i))
                {
                    num += GetPopulationForLandmass(i);
                }
            }

            return num;
        }

        public static float GetPlayerKingdomEatingPopulationTime()
        {
            float total = 0f;
            foreach (Villager villager in Villager.villagerPool.data)
            {
                if (villager != null)
                {
                    if (DataTracker.yearsOfVillagerCreation.ContainsKey(villager.guid) && villager.Residence != null)
                    {
                        total += UnityEngine.Mathf.Clamp((float)Player.inst.CurrYear - DataTracker.yearsOfVillagerCreation[villager.guid], 0f, 1f);
                        //DebugExt.Log($"{Player.inst.CurrYear}, {DataTracker.yearsOfVillagerCreation[villager.guid]}");
                    }
                }
            }
            return total;
        }

        public static int GetHousingForLandmass(int landmassIdx)
        {
            return Player.inst.TotalResidentialSlotsOnLandMass(landmassIdx);
        }

        public static int GetHousingForKingdom()
        {
            int num = 0;
            for(int i = 0; i < World.inst.NumLandMasses; i++)
            {
                if (Player.inst.LandMassIsAPlayerLandMass(i))
                {
                    num += GetHousingForLandmass(i);
                }
            }
            return num;
        }

        public static bool FoodProductionPollutedByFishingHuts()
        {
            return Player.inst.DoesAnyBuildingHaveUniqueNameOnPlayerLandMass("fishinghut", true);
        }

        public static int GetProductionPowerForResourceOnLandmass(FreeResourceType type, int landmassIdx)
        {
            int production = 0;
            ArrayExt<Building> buildings = Player.inst.GetBuildingListForLandMass(landmassIdx);
            try
            {
                foreach (Building building in buildings.data)
                {
                    if (building != null)
                    {
                        if (!building.Yield().IsEmpty())
                        {
                            if (building.Yield().Get(type) > 0)
                            {
                                int buildingValue = (int)(building.Yield().Get(type));

                                //DebugExt.Log($"yield building: {building.UniqueName}, yields {building.Yield().Get(type)} {type.ToString()}", true);
                                // Special bonuses for fields
                                if (building.GetComponent<Field>())
                                {
                                    int newValue = buildingValue;

                                    newValue += (int)((float)(typeof(Field).GetMethod("GetTotalBonus", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(building.GetComponent<Field>(), new object[0])));

                                    //DebugExt.Log("1", true);
                                    int windmillsNearby = (int)typeof(Field).GetField("windmillsNearby", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(building.GetComponent<Field>());
                                    int bonusPerWindmill = (int)typeof(Field).GetField("bonusPerWindmill", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(building.GetComponent<Field>());
                                    newValue += (windmillsNearby * bonusPerWindmill);

                                    if (Assets.StreamerEffects.Flag_BetterCropYield)
                                    {
                                        newValue = ((int)Math.Ceiling((double)newValue * 1.5));
                                    }

                                    production += newValue;
                                    //DebugExt.Log($"farm: {building.UniqueName}, ACTUALLY yields {newValue} {type.ToString()}", true);

                                }
                                // Special bonuses for orchards
                                else if (building.GetComponent<Orchard>())
                                {
                                    int newValue = buildingValue;

                                    newValue += (int)building.GetComponent<Orchard>().GetTotalBonus();

                                    int barrenCells = (int)typeof(Orchard).GetField("barrenCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(building.GetComponent<Orchard>());
                                    newValue = (int)UnityEngine.Mathf.Lerp(newValue, 0f, (float)barrenCells / 4f);


                                    if (Assets.StreamerEffects.Flag_BetterCropYield)
                                    {
                                        newValue = ((int)Math.Ceiling((double)newValue * 1.5));
                                    }

                                    production += newValue;

                                    //DebugExt.Log($"orchard: {building.UniqueName}, ACTUALLY yields {newValue} {type.ToString()}", true);
                                }
                                else
                                {
                                    production += buildingValue;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //DebugExt.Log(ex.ToString());
            }


            return production;
        }

        public static int GetProductionPowerForResourceInKingdom(FreeResourceType type)
        {
            int production = 0;
            for (int i = 0; i < World.inst.NumLandMasses; i++)
            {
                if (Player.inst.LandMassIsAPlayerLandMass(i))
                {
                    production += GetProductionPowerForResourceOnLandmass(type, i);
                }
            }
            return production;
        }

        public static Player.Production GetGameCalculatedProductionForLandmass(int landmassIdx)
        {
            return Player.inst.GetCurrProduction(landmassIdx);
        }

        public static Player.Production GetGameCalculatedProductionForKingdom()
        {
            Player.Production production = new Player.Production()
            {
                foodFarm = 0,
                foodBakery = 0,
                foodOrchard = 0,
                foodFishing = 0,
                foodPigs = 0,

                ironMine = 0,
                stoneQuarry = 0,

                toolsBlacksmith = 0,
                armamentBlacksmith = 0,

                woodClearCutting = 0,
                woodForester = 0,
                charcoalCharcoalMaker = 0,

                amtByShip = new ResourceAmount()

            };
            
            for(int i = 0; i < World.inst.NumLandMasses; i++)
            {
                if (Player.inst.LandMassIsAPlayerLandMass(i))
                {
                    Player.Production _production = GetGameCalculatedProductionForLandmass(i);

                    production.foodFarm += _production.foodFarm;
                    production.foodBakery += _production.foodBakery;
                    production.foodOrchard += _production.foodOrchard;
                    production.foodFishing += _production.foodFishing;
                    production.foodPigs += _production.foodPigs;

                    production.ironMine += _production.ironMine;
                    production.stoneQuarry += _production.stoneQuarry;

                    production.toolsBlacksmith += _production.toolsBlacksmith;
                    production.armamentBlacksmith += _production.armamentBlacksmith;

                    production.woodClearCutting += _production.woodClearCutting;
                    production.woodForester += _production.woodForester;
                    production.charcoalCharcoalMaker += _production.charcoalCharcoalMaker;

                    production.amtByShip.Add(_production.amtByShip);
                }
            }

            return production;
        }

    }
}
