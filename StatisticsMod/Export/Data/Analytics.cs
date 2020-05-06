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


        public static int GetProductionPowerForResourceOnLandmass(FreeResourceType type, int landmassIdx)
        {
            int production = 0;
            ArrayExt<Building> buildings = Player.inst.GetBuildingListForLandMass(landmassIdx);

            foreach(Building building in buildings.data)
            {
                if (building != null)
                { 
                    if (building.Yield != null)
                    {
                        if (building.Yield.Get(type) > 0)
                        {
                            production += (int)(building.Yield.Get(type));
                        }
                    }
                }
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

                    production.amtByShip += _production.amtByShip;
                }
            }

            return production;
        }

    }
}
