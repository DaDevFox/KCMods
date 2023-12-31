using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Assets.Code;
using I2.Loc;
using UnityEngine.UI;
using UnityEngine.Events;

/*
 * Special thanks to the Siege Cauldron mod where some of this code originated
 * 
 * New Houses Mod Main
 * By Rakceyen
 * https://rakceyen.wixsite.com/3dportfolio
 */

namespace mods.NewHouses
{
    public class ModMain : MonoBehaviour
    {
        public static KCModHelper helper;
        public static AssetBundle assetBundle;

        public static GameObject ApartmentPrefab;
        public static GameObject TallHovelPrefab;
        public static GameObject RowHousePrefab;
        public static GameObject CommonsHousePrefab;
        public static GameObject VillaPrefab;
        public static GameObject SlumPrefab;
        public static GameObject FancyHousePrefab;
        public static GameObject DummyHousePrefab;
        
        public static Material Blank;
        public static Material VeryHigh;
        public static Material High;
        public static Material MediumHigh;
        public static Material Medium;
        public static Material MediumLow;
        public static Material Low;
        
        public static ModMain inst;




        public void Preload(KCModHelper _helper)
        {
            inst = this;

            helper = _helper;
            String modpath = helper.modPath;

            Application.logMessageReceived += HandleLogMessage;

           
            try
            {
                assetBundle = KCModHelper.LoadAssetBundle(modpath + "/AssetBundle", "newhouses_bundle");
            }

            catch (Exception e)
            {
                helper.Log(modpath.ToString());

            }


            if (assetBundle != null)
            {

                Blank = assetBundle.LoadAsset("assets/workspace/Blank.mat") as Material;
                VeryHigh = assetBundle.LoadAsset("assets/workspace/VeryHighHappiness.mat") as Material;
                High = assetBundle.LoadAsset("assets/workspace/HighHappiness.mat") as Material;
                MediumHigh = assetBundle.LoadAsset("assets/workspace/MediumHighHappiness.mat") as Material;
                Medium = assetBundle.LoadAsset("assets/workspace/MediumHappiness.mat") as Material;
                MediumLow = assetBundle.LoadAsset("assets/workspace/MediumLowHappiness.mat") as Material;
                Low = assetBundle.LoadAsset("assets/workspace/LowHappiness.mat") as Material;

                //Apartment
                ApartmentPrefab = assetBundle.LoadAsset("assets/workspace/Apartment.prefab") as GameObject;

                if (ApartmentPrefab == null)
                {
                    helper.Log("Apartment prefab could not be loaded.");
                    return;

                }

                //=========================================================================//

                //attaches the Building script to the prefab
                ApartmentPrefab.AddComponent<Building>();
                Building bApartment = ApartmentPrefab.GetComponent<Building>();


                //attaches the Home script to the prefab
                ApartmentPrefab.AddComponent<Home>();
                Home hApartment = ApartmentPrefab.GetComponent<Home>();


                

                BuildingCollider apartmentCOL = bApartment.transform.Find("Offset").Find("Apartment").gameObject.AddComponent<BuildingCollider>();

                bApartment.UniqueName = "apartment";
                bApartment.customName = "Apartment";
                bApartment.uniqueNameHash = "apartment".GetHashCode();

                bApartment.CategoryName = "house";



                bApartment.JobCategory = JobCategory.Homemakers;
                //bApartment.placementSounds = new string[] { "castleplacement" };//replace with building sound
                bApartment.SelectionSounds = new string[] { "BuildingSelectManor_V2" };//replace with manor door sound
                //bApartment.skillUsed = "Homemakers";
                bApartment.size = new Vector3(4f, 1f, 2f);

                //cost
                SetField<Building>("Cost", bFlags, bApartment, BuildCost(300, 120, 100, 20, 0));


                bApartment.Stackable = false;

                //place people
                bApartment.personPositions = PeoplePositions(ApartmentPrefab, 12);
                
                bApartment.SubStructure = false;

                bApartment.WorkersForFullYield = 12;//number of heads of household
                bApartment.BuildAllowedWorkers = 15;

                bApartment.BuildShaderMinYAdjustment = -1f;
                bApartment.BuildShaderMaxYAdjustment = 1.65f;


                bApartment.Stackable = false;
                bApartment.troopsCanWalkOn = false;
                bApartment.transportCartsCanTravelOn = false;
                bApartment.allowOverAndUnderAqueducts = false;
                bApartment.allowCastleBlocksOnTop = 0;//0 = none, 1 = wood, 2 = stone, 3 = any
                bApartment.dragPlacementMode = 0;
                bApartment.ignoreRoadCoverageForPlacement = false;
                bApartment.doBuildAnimation = true;
                bApartment.MaxLife = 5f;//40hp

                
                hApartment.veryHighHappiness = VeryHigh;
                hApartment.highHappiness = High;
                hApartment.mediumHappiness = Medium;
                hApartment.mediumLowHappiness = MediumLow;
                hApartment.lowHappiness = Low;
                hApartment.noResidents = Blank;

                hApartment.veryHighHealth = VeryHigh;
                hApartment.highHealth = High;
                hApartment.mediumHealth = Medium;
                hApartment.mediumHighHealth = MediumHigh;
                hApartment.mediumLowHealth = MediumLow;
                hApartment.lowHealth = Low;
                
                

                //sets residents

                hApartment.MaxResidents = 60;
                //sets the tax rate. 1 = 1.25 per year, 2 = 2.5, 3 = 3.75, 4 = 5, 5 = 6.25 (I think this is how the math is working out)

                hApartment.taxYield = 12f;

                //set chimney smoke
                hApartment.ChimneySmoke = Smoke(ApartmentPrefab);






                //Tall Hovel 
                /*----------------------------------------------------------------------------------------*/
                TallHovelPrefab = assetBundle.LoadAsset("assets/workspace/TallHovel.prefab") as GameObject;

                if (TallHovelPrefab == null)
                {
                    helper.Log("Tall Hovel could not be loaded");
                    return;
                }

                TallHovelPrefab.AddComponent<Building>();
                Building bTallHovel = TallHovelPrefab.GetComponent<Building>();
                TallHovelPrefab.AddComponent<Home>();
                Home hTallHovel = TallHovelPrefab.GetComponent<Home>();



                BuildingCollider TallHovelCOL = bTallHovel.transform.Find("Offset").Find("Tall Hovel").gameObject.AddComponent<BuildingCollider>();
                TallHovelCOL.Building = bTallHovel;

                bTallHovel.UniqueName = "tallhovel";
                bTallHovel.customName = "Tall Hovel";
                bTallHovel.uniqueNameHash = "tallhovel".GetHashCode();
                bTallHovel.CategoryName = "house";


                bTallHovel.JobCategory = JobCategory.Homemakers;
                //bTallHovel.skillUsed = "Homemakers";
                bTallHovel.SubStructure = false;

                //placement sound
                //selection sound
                bTallHovel.SelectionSounds = new string[] { "BuildingSelectHovel" };

                bTallHovel.size = new Vector3(1f, 1f, 1f);

                //cost
                SetField<Building>("Cost", bFlags, bTallHovel, BuildCost(15, 10, 10, 0, 0));


                bTallHovel.personPositions = PeoplePositions(TallHovelPrefab, 2);
                bTallHovel.BuildShaderMinYAdjustment = -1f;
                bTallHovel.BuildShaderMaxYAdjustment = .35f;
                bTallHovel.WorkersForFullYield = 1;
                bTallHovel.BuildAllowedWorkers = 3;
                bTallHovel.troopsCanWalkOn = false;
                bTallHovel.transportCartsCanTravelOn = false;
                bTallHovel.allowOverAndUnderAqueducts = false;
                bTallHovel.allowCastleBlocksOnTop = 0;
                bTallHovel.ignoreRoadCoverageForPlacement = false;
                bTallHovel.doBuildAnimation = true;
                bTallHovel.MaxLife = 1.5f;//15hp


                //set residents
                hTallHovel.MaxResidents = 6;
                //set tax rev
                hTallHovel.taxYield = 2f;

                //set chimney smoke. Pass Smoke the prefab of the building.
                hTallHovel.ChimneySmoke = Smoke(TallHovelPrefab);
                //Row house

                hTallHovel.veryHighHappiness = VeryHigh;
                hTallHovel.highHappiness = High;
                hTallHovel.mediumHappiness = Medium;
                hTallHovel.mediumLowHappiness = MediumLow;
                hTallHovel.lowHappiness = Low;
                hTallHovel.noResidents = Blank;

                hTallHovel.veryHighHealth = VeryHigh;
                hTallHovel.highHealth = High;
                hTallHovel.mediumHealth = Medium;
                hTallHovel.mediumHighHealth = MediumHigh;
                hTallHovel.mediumLowHealth = MediumLow;
                hTallHovel.lowHealth = Low;


                RowHousePrefab = assetBundle.LoadAsset("assets/workspace/RowHouse.prefab") as GameObject;

                if (RowHousePrefab == null)
                {
                    helper.Log("Row house prefab not loaded");
                    return;
                }

                RowHousePrefab.AddComponent<Building>();
                Building bRowHouse = RowHousePrefab.GetComponent<Building>();
                RowHousePrefab.AddComponent<Home>();
                Home hRowHouse = RowHousePrefab.GetComponent<Home>();

                BuildingCollider RowHouseCOL = bRowHouse.transform.Find("Offset").Find("Row House").gameObject.AddComponent<BuildingCollider>();
                RowHouseCOL.Building = bRowHouse;

                bRowHouse.UniqueName = "rowhouse";
                bRowHouse.uniqueNameHash = "rowhouse".GetHashCode();
                bRowHouse.customName = "Row House";

                bRowHouse.CategoryName = "house";

                bRowHouse.JobCategory = JobCategory.Homemakers;
                //bRowHouse.skillUsed = "Homemakers";
                bRowHouse.SubStructure = false;
                //placement sound
                bRowHouse.SelectionSounds = new string[] { "BuildingSelectCottage" };
                bRowHouse.size = new Vector3(3f, 1f, 1f);

                SetField<Building>("Cost", bFlags, bRowHouse, BuildCost(30, 15, 0, 0, 0));

                bRowHouse.BuildShaderMinYAdjustment = -.5f;
                bRowHouse.BuildShaderMaxYAdjustment = 1.1f;

                bRowHouse.personPositions = PeoplePositions(RowHousePrefab, 4);
                bRowHouse.WorkersForFullYield = 4;
                bRowHouse.BuildAllowedWorkers = 5;
                bRowHouse.troopsCanWalkOn = false;
                bRowHouse.transportCartsCanTravelOn = false;
                bRowHouse.allowOverAndUnderAqueducts = false;
                bRowHouse.allowCastleBlocksOnTop = 0;
                bRowHouse.ignoreRoadCoverageForPlacement = false;
                bRowHouse.doBuildAnimation = true;
                bRowHouse.MaxLife = 2f;//20hp


                hRowHouse.MaxResidents = 20;
                hRowHouse.taxYield = 3;
                hRowHouse.ChimneySmoke = Smoke(RowHousePrefab);

                hRowHouse.veryHighHappiness = VeryHigh;
                hRowHouse.highHappiness = High;
                hRowHouse.mediumHappiness = Medium;
                hRowHouse.mediumLowHappiness = MediumLow;
                hRowHouse.lowHappiness = Low;
                hRowHouse.noResidents = Blank;

                hRowHouse.veryHighHealth = VeryHigh;
                hRowHouse.highHealth = High;
                hRowHouse.mediumHealth = Medium;
                hRowHouse.mediumHighHealth = MediumHigh;
                hRowHouse.mediumLowHealth = MediumLow;
                hRowHouse.lowHealth = Low;


                //Commons house

                CommonsHousePrefab = assetBundle.LoadAsset("assets/workspace/CommonsHouse.prefab") as GameObject;

                if (CommonsHousePrefab == null)
                {
                    helper.Log("Commons house prefab could not be loaded.");
                    return;
                }

                CommonsHousePrefab.AddComponent<Building>();
                Building bCommonsHouse = CommonsHousePrefab.GetComponent<Building>();
                CommonsHousePrefab.AddComponent<Home>();
                Home hCommonsHouse = CommonsHousePrefab.GetComponent<Home>();

                BuildingCollider CommonsHouseCOL = bCommonsHouse.transform.Find("Offset").Find("Commons House").gameObject.AddComponent<BuildingCollider>();
                CommonsHouseCOL.Building = bCommonsHouse;

                bCommonsHouse.UniqueName = "commonshouse";
                bCommonsHouse.uniqueNameHash = "commonshouse".GetHashCode();
                bCommonsHouse.customName = "Commons House";

                bCommonsHouse.CategoryName = "house";

                bCommonsHouse.JobCategory = JobCategory.Homemakers;
                //bCommonsHouse.skillUsed = "Homemakers";
                bCommonsHouse.SubStructure = false;
                //placement sound
                bCommonsHouse.SelectionSounds = new string[] { "BuildingSelectManor_V2" };
                bCommonsHouse.size = new Vector3(3f, 1f, 2f);

                SetField<Building>("Cost", bFlags, bCommonsHouse, BuildCost(75, 45, 35, 0, 0));

                bCommonsHouse.BuildShaderMinYAdjustment = -.1f;
                bCommonsHouse.BuildShaderMaxYAdjustment = .95f;

                bCommonsHouse.personPositions = PeoplePositions(CommonsHousePrefab, 6);
                bCommonsHouse.WorkersForFullYield = 6;
                bCommonsHouse.BuildAllowedWorkers = 8;
                bCommonsHouse.troopsCanWalkOn = false;
                bCommonsHouse.transportCartsCanTravelOn = false;
                bCommonsHouse.allowOverAndUnderAqueducts = false;
                bCommonsHouse.allowCastleBlocksOnTop = 0;
                bCommonsHouse.ignoreRoadCoverageForPlacement = false;
                bCommonsHouse.doBuildAnimation = true;
                bCommonsHouse.MaxLife = 3f;


                hCommonsHouse.MaxResidents = 40;
                hCommonsHouse.taxYield = 7f;
                hCommonsHouse.ChimneySmoke = Smoke(CommonsHousePrefab);

                hCommonsHouse.veryHighHappiness = VeryHigh;
                hCommonsHouse.highHappiness = High;
                hCommonsHouse.mediumHappiness = Medium;
                hCommonsHouse.mediumLowHappiness = MediumLow;
                hCommonsHouse.lowHappiness = Low;
                hCommonsHouse.noResidents = Blank;

                hCommonsHouse.veryHighHealth = VeryHigh;
                hCommonsHouse.highHealth = High;
                hCommonsHouse.mediumHealth = Medium;
                hCommonsHouse.mediumHighHealth = MediumHigh;
                hCommonsHouse.mediumLowHealth = MediumLow;
                hCommonsHouse.lowHealth = Low;



                //Villa
                VillaPrefab = assetBundle.LoadAsset("assets/workspace/Villa.prefab") as GameObject;

                if (VillaPrefab == null)
                {
                    helper.Log("Villa prefab could not be loaded");
                    return;
                }

                VillaPrefab.AddComponent<Building>();
                Building bVilla = VillaPrefab.GetComponent<Building>();
                VillaPrefab.AddComponent<Home>();
                Home hVilla = VillaPrefab.GetComponent<Home>();

                BuildingCollider VillaCOL = bVilla.transform.Find("Offset").Find("Villa").gameObject.AddComponent<BuildingCollider>();
                VillaCOL.Building = bVilla;

                bVilla.UniqueName = "villa";
                bVilla.uniqueNameHash = "villa".GetHashCode();
                bVilla.customName = "Villa";

                bVilla.CategoryName = "house";

                bVilla.JobCategory = JobCategory.Homemakers;
                //bVilla.skillUsed = "Homemakers";
                bVilla.SubStructure = false;
                //placement sound
                bVilla.SelectionSounds = new string[] { "BuildingSelectManor_V2" };
                bVilla.size = new Vector3(3f, 1f, 3f);

                SetField<Building>("Cost", bFlags, bVilla, BuildCost(200, 150, 350, 0, 0));

                bVilla.BuildShaderMinYAdjustment = -.5f;
                bVilla.BuildShaderMaxYAdjustment = .7f;

                bVilla.personPositions = PeoplePositions(VillaPrefab, 8);
                bVilla.WorkersForFullYield = 8;
                bVilla.BuildAllowedWorkers = 12;
                bVilla.troopsCanWalkOn = true;
                bVilla.transportCartsCanTravelOn = false;
                bVilla.allowOverAndUnderAqueducts = false;
                bVilla.allowCastleBlocksOnTop = 0;
                bVilla.ignoreRoadCoverageForPlacement = false;
                bVilla.doBuildAnimation = true;
                bVilla.MaxLife = 6f;


                hVilla.MaxResidents = 30;
                hVilla.taxYield = 18;
                hVilla.ChimneySmoke = Smoke(VillaPrefab);

                hVilla.veryHighHappiness = VeryHigh;
                hVilla.highHappiness = High;
                hVilla.mediumHappiness = Medium;
                hVilla.mediumLowHappiness = MediumLow;
                hVilla.lowHappiness = Low;
                hVilla.noResidents = Blank;

                hVilla.veryHighHealth = VeryHigh;
                hVilla.highHealth = High;
                hVilla.mediumHealth = Medium;
                hVilla.mediumHighHealth = MediumHigh;
                hVilla.mediumLowHealth = MediumLow;
                hVilla.lowHealth = Low;



                //slum
                SlumPrefab = assetBundle.LoadAsset("assets/workspace/Slum.prefab") as GameObject;

                if (SlumPrefab == null)
                {
                    helper.Log("Slum prefab could not be loaded");
                    return;
                }

                SlumPrefab.AddComponent<Building>();
                Building bSlum = SlumPrefab.GetComponent<Building>();
                SlumPrefab.AddComponent<Home>();
                Home hSlum = SlumPrefab.GetComponent<Home>();

                BuildingCollider SlumCOL = bSlum.transform.Find("Offset").Find("Slum").gameObject.AddComponent<BuildingCollider>();
                SlumCOL.Building = bSlum;

                bSlum.UniqueName = "slum";
                bSlum.uniqueNameHash = "slum".GetHashCode();
                bSlum.customName = "Slum";

                bSlum.CategoryName = "house";

                bSlum.JobCategory = JobCategory.Homemakers;
                //bSlum.skillUsed = "Homemakers";
                bSlum.SubStructure = false;
                //placement sound
                bSlum.SelectionSounds = new string[] { "BuildingSelectManor_V2" };
                bSlum.size = new Vector3(3f, 1f, 3f);

                SetField<Building>("Cost", bFlags, bSlum, BuildCost(60, 0, 0, 0, 0));

                bSlum.BuildShaderMinYAdjustment = -.5f;
                bSlum.BuildShaderMaxYAdjustment = 1.35f;

                bSlum.personPositions = PeoplePositions(SlumPrefab, 10);
                bSlum.WorkersForFullYield = 10;
                bSlum.BuildAllowedWorkers = 5;
                bSlum.troopsCanWalkOn = true;
                bSlum.transportCartsCanTravelOn = false;
                bSlum.allowCastleBlocksOnTop = 0;
                bSlum.ignoreRoadCoverageForPlacement = false;
                bSlum.doBuildAnimation = true;
                bSlum.MaxLife = 3f;
                bSlum.decayRate = 0.5f;


                hSlum.MaxResidents = 55;
                hSlum.taxYield = 1f;
                hSlum.ChimneySmoke = Smoke(SlumPrefab);

                hSlum.veryHighHappiness = VeryHigh;
                hSlum.highHappiness = High;
                hSlum.mediumHappiness = Medium;
                hSlum.mediumLowHappiness = MediumLow;
                hSlum.lowHappiness = Low;
                hSlum.noResidents = Blank;

                hSlum.veryHighHealth = VeryHigh;
                hSlum.highHealth = High;
                hSlum.mediumHealth = Medium;
                hSlum.mediumHighHealth = MediumHigh;
                hSlum.mediumLowHealth = MediumLow;
                hSlum.lowHealth = Low;



                FancyHousePrefab = assetBundle.LoadAsset("assets/workspace/FancyHouse.prefab") as GameObject;

                if (FancyHousePrefab == null)
                {
                    helper.Log("Fancy House prefab could not be loaded");
                    return;
                }

                FancyHousePrefab.AddComponent<Building>();
                Building bFancyHouse = FancyHousePrefab.GetComponent<Building>();
                FancyHousePrefab.AddComponent<Home>();
                Home hFancyHouse = FancyHousePrefab.GetComponent<Home>();

                BuildingCollider FancyHouseCOL = bFancyHouse.transform.Find("Offset").Find("Fancy House").gameObject.AddComponent<BuildingCollider>();
                FancyHouseCOL.Building = bFancyHouse;

                bFancyHouse.UniqueName = "fancyhouse";
                bFancyHouse.uniqueNameHash = "fancyhouse".GetHashCode();
                bFancyHouse.customName = "Fancy House";

                bFancyHouse.CategoryName = "house";

                bFancyHouse.JobCategory = JobCategory.Homemakers;
                //bFancyHouse.skillUsed = "Homemakers";
                bFancyHouse.SubStructure = false;
                //placement sound
                bFancyHouse.SelectionSounds = new string[] { "BuildingSelectCottage" };
                bFancyHouse.size = new Vector3(2f, 1f, 1f);

                SetField<Building>("Cost", bFlags, bFancyHouse, BuildCost(45, 15, 25, 0, 0));

                bFancyHouse.BuildShaderMinYAdjustment = -.5f;
                bFancyHouse.BuildShaderMaxYAdjustment = .7f;

                bFancyHouse.personPositions = PeoplePositions(FancyHousePrefab, 2);
                bFancyHouse.WorkersForFullYield = 2;
                bFancyHouse.BuildAllowedWorkers = 4;
                bFancyHouse.troopsCanWalkOn = false;
                bFancyHouse.transportCartsCanTravelOn = false;
                bFancyHouse.allowCastleBlocksOnTop = 0;
                bFancyHouse.ignoreRoadCoverageForPlacement = false;
                bFancyHouse.doBuildAnimation = true;
                bFancyHouse.MaxLife = 2f;


                hFancyHouse.MaxResidents = 8;
                hFancyHouse.taxYield = 5f;
                hFancyHouse.ChimneySmoke = Smoke(FancyHousePrefab);

                hFancyHouse.veryHighHappiness = VeryHigh;
                hFancyHouse.highHappiness = High;
                hFancyHouse.mediumHappiness = Medium;
                hFancyHouse.mediumLowHappiness = MediumLow;
                hFancyHouse.lowHappiness = Low;
                hFancyHouse.noResidents = Blank;

                hFancyHouse.veryHighHealth = VeryHigh;
                hFancyHouse.highHealth = High;
                hFancyHouse.mediumHealth = Medium;
                hFancyHouse.mediumHighHealth = MediumHigh;
                hFancyHouse.mediumLowHealth = MediumLow;
                hFancyHouse.lowHealth = Low;

                


                DummyHousePrefab = assetBundle.LoadAsset("assets/workspace/DummyHouse.prefab") as GameObject;
                if (DummyHousePrefab == null)
                {
                    helper.Log("Dummy house prefab could not be loaded");
                    return;
                }
                DummyHousePrefab.AddComponent<Building>();
                Building bDummyHouse = DummyHousePrefab.GetComponent<Building>();
                DummyHousePrefab.AddComponent<Home>();
                Home hDummyHouse = DummyHousePrefab.GetComponent<Home>();
                bDummyHouse.UniqueName = "newhouses";
                bDummyHouse.uniqueNameHash = "newhouses".GetHashCode();
                bDummyHouse.customName = "New Houses";
                bDummyHouse.CategoryName = "house";
                BuildingCollider DummyHouseCOL = bDummyHouse.transform.Find("Offset").Find("Dummy House").gameObject.AddComponent<BuildingCollider>();
                DummyHouseCOL.Building = bDummyHouse;

            }
            else
            {
                helper.Log("Bundle not loaded");
            }

            //initializing harmony patches
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());


        }

        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
                helper.Log($"{condition}\n\t{stackTrace}");
        }

        //places people at empty game objects in or near the building
        //be sure to name the positions p(1), p(2), p(3), etc
        //place the last position in the center of the building to prevent visible people stacking
        //place the first position in the center or in the doorway. The first point acts as the spot to bring materials
        public Transform[] PeoplePositions(GameObject prefab, int positions)
        {
            Transform offset = prefab.transform.Find("Offset");
            Transform[] peoplePositions = new Transform[positions];
            Transform peep;

            for (int i = 1; i < positions + 1; i++)
            {
                peep = offset.Find(("p (" + i.ToString() + ")"));
                peoplePositions[i - 1] = peep;
            }
            

            return peoplePositions;
        }

        //places the chimney smoke object onto the chimney. Make sure to have the particle system parented to the Offset object
        public ParticleSystem Smoke(GameObject prefab)
        {
            ParticleSystem loc = prefab.transform.Find("Offset").Find("ChimneySmoke").GetComponent<ParticleSystem>();
            return loc;
        }

        //sets the resource cost to the building. 
        public ResourceAmount BuildCost(int tree, int stone, int gold, int iron, int tools)
        {
            ResourceAmount cost = new ResourceAmount();
            cost.Set(FreeResourceType.Tree, tree);
            cost.Set(FreeResourceType.Stone, stone);
            cost.Set(FreeResourceType.Gold, gold);
            cost.Set(FreeResourceType.IronOre, iron);
            cost.Set(FreeResourceType.Tools, tools);

            return cost;
        }

        //sets how much stuff this building can hold
        public ResourceAmount BuildStorage(int wheat, int charcoal, int fish, int apples, int pork)
        {
            ResourceAmount storage = new ResourceAmount();

            storage.Set(FreeResourceType.Wheat, wheat);
            storage.Set(FreeResourceType.Charcoal, charcoal);
            storage.Set(FreeResourceType.Fish, fish);
            storage.Set(FreeResourceType.Apples, apples);
            storage.Set(FreeResourceType.Pork, pork);

            return storage;
        }



        BindingFlags bFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        public bool SetField<T>(string variable, BindingFlags flags, T source, object value)
        {
            FieldInfo fieldInfo = typeof(T).GetField(variable, flags);

            if (fieldInfo == null)
                return false;
            fieldInfo.SetValue(source, value);
            return true;
        }

        /*
         * [HarmonyPatch (typeof (Class))]
         * [HarmonyPatch ("Method")
         * public static class NamePatch
         */

        //inst prefabs
        [HarmonyPatch(typeof(GameState))]
        [HarmonyPatch("Awake")]
        public static class InternalPrefabsPatch
        {
            static void Postfix(GameState __instance)
            {
                __instance.internalPrefabs.Add(TallHovelPrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(RowHousePrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(ApartmentPrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(CommonsHousePrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(VillaPrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(SlumPrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(FancyHousePrefab.GetComponent<Building>());
                __instance.internalPrefabs.Add(DummyHousePrefab.GetComponent<Building>());

            }
        }

        // Activates logic for submenu opening
        [HarmonyPatch(typeof(BuildingCostUpdater), "OnPointerDown")]
        static class SubMenuFunctionalPatch
        {
            
            static bool Prefix(BuildingCostUpdater __instance)
            {
                if (!__instance.Enabled)
                    return false;

                if (__instance.prefab.UniqueName == "newhouses")
                {
                    GameUI.inst.OnShowBuildTabClicked("Houses");
                    return false;
                }
                return true;
            }
        }

        // Visually adds the [...] to indicate a submenu
        [HarmonyPatch(typeof(BuildingCostUpdater), "IsSubMenu")]
        static class SubMenuVisualPatch
        {
            static void Postfix(BuildingCostUpdater __instance, ref bool __result)
            {
                __result |= __instance.prefab.UniqueName == "newhouses";
            }
        }

        //Add building to build UI
        //Current version will just put houses in the town tab, commented section is an attempt at a new menu
        [HarmonyPatch(typeof(BuildUI))]
        [HarmonyPatch("Start")]
        public static class BuildUIPatch
        {
            static void Postfix(BuildUI __instance, List<BuildTab> ___tabs)
            {
                //not working yet
                
                MethodInfo AddTab = typeof(BuildUI).GetMethod("AddTab", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo AddTabVR = typeof(BuildUI).GetMethod("AddTabVR", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo AddTabConsole = typeof(BuildUI).GetMethod("AddConsoleTab", BindingFlags.Instance | BindingFlags.NonPublic);
                BuildTab HousesTab;
                BuildTab HousesTabVR;
                BuildTabConsole HousesTabConsole;

                // If you want a direct build tab (like Castle, Town, Advanced Town, etc (not thoroughly tested):
                //GameObject buildMenuButtonExemplar = __instance.buildMenuButtons.transform.Find("Town").gameObject;
                //GameObject buildMenuButtonClone = GameObject.Instantiate(buildMenuButtonExemplar, __instance.buildMenuButtons.transform);
                //buildMenuButtonClone.transform.SetSiblingIndex(2); // controls location in menu
                //// can also change image here with .GetComponent<Image>().image = [your image];
                //buildMenuButtonClone.name = "Houses";
                //buildMenuButtonClone.GetComponent<Button>().onClick.AddListener(() => { BuildUI.inst.SetTabActive("Houses"); });

                HousesTab = (BuildTab) AddTab.Invoke(__instance, new object[] { "Houses" });
                HousesTab.isBackable = true;
                HousesTab.back.onClick.AddListener(delegate ()
                {
                    GameUI.inst.OnShowBuildTabClicked(__instance.TownTab.title);
                });

                HousesTabVR = (BuildTab)AddTabVR.Invoke(__instance, new object[] { "Houses" });
                HousesTabVR.isBackable = true;
                HousesTabVR.back.onClick.AddListener(delegate ()
                {
                    GameUI.inst.OnShowBuildTabClicked(__instance.TownTabVR.title);
                });

                HousesTabConsole = (BuildTabConsole)AddTabConsole.Invoke(__instance, new object[] { "Houses" });  
                HousesTabConsole.isBackable = true;
                HousesTabConsole.back.onClick.AddListener(delegate ()
                {
                    GameUI.inst.OnShowBuildTabClicked(__instance.TownTabConsole.title);
                });

                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole, "newhouses", World.keepName, new Vector3(.7f, .7f, .7f));

                BuildUI.inst.TownTab.ButtonPanel.Find("newhouses").SetSiblingIndex(6);
                BuildUI.inst.TownTab.ButtonPanel.Find("smallhouse").SetParent(HousesTab.ButtonPanel);
                BuildUI.inst.TownTab.ButtonPanel.Find("largehouse").SetParent(HousesTab.ButtonPanel);
                BuildUI.inst.TownTab.ButtonPanel.Find("manorhouse").SetParent(HousesTab.ButtonPanel);

                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "tallhovel", World.keepName, new Vector3(.7f, .7f, .7f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "slum", World.keepName, new Vector3(.5f, .5f, .5f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "fancyhouse", World.keepName, new Vector3(.5f, .5f, .5f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "rowhouse", World.keepName, new Vector3(.5f, .5f, .5f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "commonshouse", World.keepName, new Vector3(.4f, .4f, .4f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "apartment", World.keepName, new Vector3(.3f, .3f, .3f));
                __instance.AddBuilding(HousesTab, HousesTabVR, HousesTabConsole,
                    "villa", World.keepName, new Vector3(.3f, .3f, .3f));

                __instance.RefreshButtonLayout();
                HousesTab.Visible = false;
                HousesTabVR.Visible = false;
                HousesTabConsole.Visible = false;
                
                
                /**
                
                // Previous version:
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "tallhovel", World.keepName, new Vector3(.7f, .7f, .7f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "slum", World.keepName, new Vector3(.3f, .3f, .3f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "fancyhouse", World.keepName, new Vector3(.5f, .5f, .5f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "rowhouse", World.keepName, new Vector3(.5f, .5f, .5f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "commonshouse", World.keepName, new Vector3(.4f, .4f, .4f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "apartment", World.keepName, new Vector3(.3f, .3f, .3f));
                __instance.AddBuilding(__instance.TownTab, __instance.TownTabVR, __instance.TownTabConsole,
                    "villa", World.keepName, new Vector3(.3f, .3f, .3f));
                **/
            }
        }

        //Thought bubbles and descriptions
        [HarmonyPatch(typeof(LocalizationManager))]
        [HarmonyPatch("GetTranslation")]
        public static class LocalizationManagerPatch
        {
            static void Postfix(string Term, ref string __result)
            {
                if (Term == "Building apartment FriendlyName")
                    __result = "Apartment";
                else if (Term == "Building apartment Description")
                    __result = "A large yet packed home. Can hold 60 peasants. This house will have a high demand for charcoal and food.";
                else if (Term == "Building apartment ThoughtOnBuilt")
                    __result = "Happy to see new homes being built.";

                if (Term == "Building tallhovel FriendlyName")
                    __result = "Tall Hovel";
                else if (Term == "Building tallhovel Description")
                    __result = "A slightly taller house. Houses 6 peasants and pays taxes.";
                else if (Term == "Building tallhovel ThoughtOnBuilt")
                    __result = "I hope I get the upstairs room.";

                if (Term == "Building rowhouse FriendlyName")
                    __result = "Row House";
                else if (Term == "Building rowhouse Description")
                    __result = "A slightly longer house. Houses 20 peasants and has the same demands as a cottage.";
                else if (Term == "Building rowhouse ThoughtOnBuilt")
                    __result = "";

                if (Term == "Building commonshouse FriendlyName")
                    __result = "Commons House";
                else if (Term == "Building commonshouse Description")
                    __result = "A somewhat tight house. Houses 40 peasants.";
                else if (Term == "Building commonshouse ThoughtOnBuilt")
                    __result = "The homes keep getting bigger!";

                if (Term == "Building villa FriendlyName")
                    __result = "Villa";
                else if (Term == "Building villa Description")
                    __result = "A fancy house for the wealthiest of peasants. Houses 30. Generates extra tax revenue.";
                else if (Term == "Building villa ThoughtOnBuilt")
                    __result = "The new villa looks like a dream house";

                if (Term == "Building slum FriendlyName")
                    __result = "Slum";
                else if (Term == "Building slum Description")
                    __result = "A low quality collection of buildings. Cheap and quick to build, but not very space efficient. Houses 55 peasants and generates very little taxes.";
                else if (Term == "Building slum ThoughtOnBuilt")
                    __result = "";

                if (Term == "Building fancyhouse FriendlyName")
                    __result = "Fancy House";
                else if (Term == "Building fancyhouse Description")
                    __result = "A nicer cottage. Houses 8. Gives more tax yield than a normal cottage. Demands charcoal.";
                else if (Term == "Building fancyhouse ThoughtOnBuilt")
                    __result = "";

                if (Term == "Building newhouses FriendlyName")
                    __result = "New Houses";
                else if (Term == "Building newhouses Description")
                    __result = "Mod: New Houses by Rakceyen";
                else if (Term == "Building newhouses ThoughtOnBuilt")
                    __result = "";

            }
        }

        


        //Designates the house type (small, large, manor), sets the home storage, and sets the base happiness of each house
        [HarmonyPatch(typeof(Home))]
        [HarmonyPatch("OnInit")]
        public static class HouseTypePatch
        {
            static void Prefix(Home __instance)
            {
                Building b = __instance.GetComponent<Building>();
                ResourceAmount storage = new ResourceAmount();//Sets the max storage amount, place values below



                FieldInfo hType = typeof(Home).GetField("houseType", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo hStorage = typeof(Home).GetField("resourceStorageMax", BindingFlags.Instance | BindingFlags.Public);
                FieldInfo hHappiness = typeof(Home).GetField("baseHappiness", BindingFlags.Instance | BindingFlags.Public);

                if (b.UniqueName == "apartment")
                {
                    hType.SetValue(__instance, 2);
                    storage.Set(FreeResourceType.Wheat, 60);
                    storage.Set(FreeResourceType.Charcoal, 20);
                    storage.Set(FreeResourceType.Fish, 30);
                    storage.Set(FreeResourceType.Apples, 30);
                    storage.Set(FreeResourceType.Pork, 30);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 45);

                }

                if (b.UniqueName == "tallhovel")
                {
                    hType.SetValue(__instance, 1);
                    storage.Set(FreeResourceType.Wheat, 7);
                    storage.Set(FreeResourceType.Charcoal, 3);
                    storage.Set(FreeResourceType.Fish, 6);
                    storage.Set(FreeResourceType.Apples, 6);
                    storage.Set(FreeResourceType.Pork, 6);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 60);

                }

                if (b.UniqueName == "rowhouse")
                {
                    hType.SetValue(__instance, 1);
                    storage.Set(FreeResourceType.Wheat, 20);
                    storage.Set(FreeResourceType.Charcoal, 6);
                    storage.Set(FreeResourceType.Fish, 12);
                    storage.Set(FreeResourceType.Apples, 12);
                    storage.Set(FreeResourceType.Pork, 12);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 60);

                }

                if (b.UniqueName == "commonshouse")
                {
                    hType.SetValue(__instance, 2);
                    storage.Set(FreeResourceType.Wheat, 40);
                    storage.Set(FreeResourceType.Charcoal, 15);
                    storage.Set(FreeResourceType.Fish, 30);
                    storage.Set(FreeResourceType.Apples, 30);
                    storage.Set(FreeResourceType.Pork, 30);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 50);

                }

                if (b.UniqueName == "villa")
                {
                    hType.SetValue(__instance, 2);
                    storage.Set(FreeResourceType.Wheat, 40);
                    storage.Set(FreeResourceType.Charcoal, 20);
                    storage.Set(FreeResourceType.Fish, 30);
                    storage.Set(FreeResourceType.Apples, 30);
                    storage.Set(FreeResourceType.Pork, 30);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 60);

                }

                if (b.UniqueName == "slum")
                {
                    hType.SetValue(__instance, 0);
                    storage.Set(FreeResourceType.Wheat, 60);
                    storage.Set(FreeResourceType.Charcoal, 15);
                    storage.Set(FreeResourceType.Fish, 25);
                    storage.Set(FreeResourceType.Apples, 25);
                    storage.Set(FreeResourceType.Pork, 25);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 35);



                }

                if (b.UniqueName == "fancyhouse")
                {
                    hType.SetValue(__instance, 2);
                    storage.Set(FreeResourceType.Wheat, 15);
                    storage.Set(FreeResourceType.Charcoal, 6);
                    storage.Set(FreeResourceType.Fish, 12);
                    storage.Set(FreeResourceType.Apples, 12);
                    storage.Set(FreeResourceType.Pork, 12);
                    hStorage.SetValue(__instance, storage);
                    hHappiness.SetValue(__instance, 65);

                }
                /*
                if (b.UniqueName == "manorhouse" || b.UniqueName == "largehouse" || b.UniqueName == "smallhouse")
                {
                    helper.Log(b.UniqueName.ToString() + " placement sounds: ");
                    foreach(var entry in b.placementSounds)
                    {
                        helper.Log(entry.ToString());
                    }

                    helper.Log(b.UniqueName.ToString() + " selection sounds: ");
                    foreach (var entry2 in b.SelectionSounds)
                    {
                        helper.Log(entry2.ToString());
                    }
                    
                    
                    
                }
                */

            }
        }

        //Sets the "has neighbor" bonus in new houses
        [HarmonyPatch(typeof(Home))]
        [HarmonyPatch("RefreshBonuses")]

        public static class HasNeighborsPatch
        {
            static void Postfix(Home __instance)
            {
                Building house = __instance.GetComponent<Building>();
                FieldInfo hasNeighbors = typeof(Home).GetField("hasNeighbors", BindingFlags.Instance | BindingFlags.NonPublic);

                int a;
                int b;
                int c;
                int d;

                hasNeighbors.SetValue(__instance, false);

                house.GetBoundingFootprint(out a, out b, out c, out d);

                int e = Mathff.Clamp(a - 1, 0, World.inst.GridWidth - 1);
                int f = Mathff.Clamp(c + 1, 0, World.inst.GridWidth - 1);
                int g = Mathff.Clamp(b - 1, 0, World.inst.GridHeight - 1);
                int h = Mathff.Clamp(d + 1, 0, World.inst.GridHeight - 1);

                //if anything needs to be within 1 tile of the house
                for (int i = g; i <= h; i++)
                {
                    for (int j = e; j <= f; j++)
                    {
                        //town square, market, garden...
                    }
                    //water wells
                }


                e = Mathff.Clamp(a - 2, 0, World.inst.GridWidth - 1);
                f = Mathff.Clamp(c + 2, 0, World.inst.GridWidth - 1);
                int k = Mathff.Clamp(b - 2, 0, World.inst.GridHeight - 1);
                h = Mathff.Clamp(d + 2, 0, World.inst.GridHeight - 1);

                //if anything needs to be within 2 tiles of the house
                for (int l = k; l <= h; l++)
                {
                    for (int m = e; m <= f; m++)
                    {
                        Cell cellDataUnsafe3 = World.inst.GetCellDataUnsafe(m, l);
                        if (!(cellDataUnsafe3.BottomStructure == house) && cellDataUnsafe3.OccupyingStructure.Count > 0)
                        {
                            Building building3 = cellDataUnsafe3.OccupyingStructure[0];
                            if (building3.uniqueNameHash == "apartment".GetHashCode() ||
                                building3.uniqueNameHash == "rowhouse".GetHashCode() ||
                                building3.uniqueNameHash == "tallhovel".GetHashCode() ||
                                building3.uniqueNameHash == "commonshouse".GetHashCode() ||
                                building3.uniqueNameHash == "villa".GetHashCode() ||
                                building3.uniqueNameHash == "slum".GetHashCode() ||
                                building3.uniqueNameHash == "fancyhouse".GetHashCode() ||
                                building3.uniqueNameHash == "smallhouse".GetHashCode() ||
                                building3.uniqueNameHash == "largehouse".GetHashCode() ||
                                building3.uniqueNameHash == "manorhouse".GetHashCode())
                            {
                                hasNeighbors.SetValue(__instance, true);
                            }
                            //fishmonger, charcoal maker, blacksmith...
                        }
                    }
                }
            }
        }


        //allows for plagues in new houses on multiple islands
        [HarmonyPatch(typeof(Player))]
        [HarmonyPatch("PlagueStart")]

        public static class PlaguePatch
        {
            static void Prefix(Player __instance, ref int landMass)
            {
                helper.Log("Landmass chosen: " + landMass);
                //int lmass = 0;
                float m = 1f;
                Villager peasant = null;
                MethodInfo PlagueVirulence = typeof(Player).GetMethod("GetPlagueVirulence", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo MaxInfections = typeof(Player).GetMethod("GetMaxInfectionsAtStart", BindingFlags.Instance | BindingFlags.NonPublic);

                if (World.inst.GetVillagersForLandMass(landMass).Count > 1000f)
                {

                    int n1 = 0;
                    int n2 = Mathff.Max((int)((float)World.inst.GetVillagersForLandMass(landMass).Count / 1000f), 1);
                    for (int Li = 0; Li < n2; Li++)
                    {
                        ArrayExt<Building> newHouseListHovels = Player.inst.GetBuildingListForLandMass(landMass, "tallhovel".GetHashCode());
                        ArrayExt<Building> newHouseListSlums = Player.inst.GetBuildingListForLandMass(landMass, "slum".GetHashCode());
                        ArrayExt<Building> newHouseListRowHouse = Player.inst.GetBuildingListForLandMass(landMass, "rowhouse".GetHashCode());
                        ArrayExt<Building> newHouseListFancy = Player.inst.GetBuildingListForLandMass(landMass, "fancyhouse".GetHashCode());
                        ArrayExt<Building> newHouseListCommons = Player.inst.GetBuildingListForLandMass(landMass, "commonshouse".GetHashCode());
                        ArrayExt<Building> newHouseListApartment = Player.inst.GetBuildingListForLandMass(landMass, "apartment".GetHashCode());
                        ArrayExt<Building> newHouseListVilla = Player.inst.GetBuildingListForLandMass(landMass, "villa".GetHashCode());
                        

                        

                        int maxHouses = newHouseListHovels.Count + newHouseListSlums.Count + newHouseListRowHouse.Count +
                            newHouseListFancy.Count + newHouseListCommons.Count + newHouseListApartment.Count + newHouseListVilla.Count;
                        int rand = SRand.Range(0, maxHouses);

                        ArrayExt<Building> AllHouses = new ArrayExt<Building>(maxHouses);

                        AllHouses.AddRange(newHouseListHovels);
                        AllHouses.AddRange(newHouseListSlums);
                        AllHouses.AddRange(newHouseListRowHouse);
                        AllHouses.AddRange(newHouseListFancy);
                        AllHouses.AddRange(newHouseListCommons);
                        AllHouses.AddRange(newHouseListApartment);
                        AllHouses.AddRange(newHouseListVilla);

                        Building house = null;
                        if(rand <= (int)(maxHouses * 0.25f))//gives a 25% chance of infecting any new house
                        {
                            house = AllHouses.RandomElement();
                        }
                        else
                        {
                            house = null;
                        }
                        /*
                        if (rand >= 0 && rand < newHouseListHovels.Count + newHouseListSlums.Count)
                        {
                            if (rand >= newHouseListSlums.Count)
                            {
                                house = newHouseListSlums.RandomElement();
                            }
                            else
                            {
                                house = newHouseListHovels.RandomElement();
                            }
                        }
                        else if (rand >= newHouseListRowHouse.Count + newHouseListFancy.Count + newHouseListCommons.Count && rand < newHouseListHovels.Count + newHouseListSlums.Count)
                        {
                            if (rand >= newHouseListRowHouse.Count && rand < newHouseListCommons.Count + newHouseListFancy.Count)
                            {
                                house = newHouseListRowHouse.RandomElement();
                            }
                            else if (rand >= newHouseListCommons.Count && rand < newHouseListFancy.Count + newHouseListRowHouse.Count)
                            {
                                house = newHouseListCommons.RandomElement();
                            }
                            else
                            {
                                house = newHouseListFancy.RandomElement();
                            }

                        }
                        else if (rand >= newHouseListApartment.Count + newHouseListVilla.Count && rand < newHouseListRowHouse.Count + newHouseListFancy.Count + newHouseListCommons.Count)
                        {
                            if (rand >= newHouseListApartment.Count)
                            {
                                house = newHouseListApartment.RandomElement();
                            }
                            else
                            {
                                house = newHouseListVilla.RandomElement();
                            }
                        }
                        */
                        if (house == null)
                        {
                            helper.Log("No house chosen for Plague");
                            return;
                        }
                        int plagueV = (int)PlagueVirulence.Invoke(__instance, new object[] { landMass, m });
                        int maxinfected = (int)MaxInfections.Invoke(__instance, new object[] { landMass });

                        helper.Log("Plague location chosen: " + house.ToString());

                        int n4 = (int)house.transform.position.x;
                        int n5 = (int)house.transform.position.z;
                        int n6 = Mathff.Clamp(n4 - 1, 0, World.inst.GridWidth - 1);
                        int n7 = Mathff.Clamp(n5 - 1, 0, World.inst.GridHeight - 1);
                        int n8 = Mathff.Clamp(n4 + 1, 0, World.inst.GridWidth - 1);
                        int n9 = Mathff.Clamp(n5 + 1, 0, World.inst.GridWidth - 1);
                        for (int Lj = n7; Lj <= n9; Lj++)
                        {
                            for (int Lk = n6; Lk <= n8; Lk++)
                            {
                                ArrayExt<Villager> peasantsAt = World.inst.GetVillagersAt(Lk, Lj);
                                for (int Ll = 0; Ll < peasantsAt.Count; Ll++)
                                {
                                    n1++;
                                    peasantsAt.data[Ll].BecomeSick(plagueV);
                                    peasant = peasantsAt.data[Ll];
                                    if (n1 >= maxinfected)
                                    {
                                        goto IL_exit;
                                    }
                                    if (SRand.Range(0, 100) > 50)
                                    {
                                        break;
                                    }

                                }
                            }
                        }
                    IL_exit:;
                    }
                    if (n1 > 0)
                    {
                        SfxSystem.inst.PlayFromBankIfVisible("BuildingSelectHospital", peasant.Pos, null);
                        KingdomLog.TryLog("plague", string.Format(ScriptLocalization.LogPlayerPlagueTakenIll,
                            __instance.PeasentAffectCountString(n1, "yellow")), KingdomLog.LogStatus.Warning, 20f, peasant.Pos,
                            false, landMass);
                        __instance.AddHappinessMod("plagueonset", 50f, -15, ScriptLocalization.PlayerPlagueUnease, landMass, false,
                            int.MaxValue, int.MinValue);



                    }
                    return;
                }


            }
        }

        //Allows for reclaiming iron from demolished apartments
        [HarmonyPatch (typeof(World))]
        [HarmonyPatch ("ReclaimResources")]

        public static class ReclaimPatch
        {
            static void Postfix( ref Cell cell, ref ResourceAmount collected)
            {
                if ( collected.Get(FreeResourceType.IronOre)> 0)
                {
                    FreeResourceManager.inst.GetAutoStackFor(FreeResourceType.IronOre, collected.Get(FreeResourceType.IronOre)).transform.position = cell.Position + new Vector3(0.5f, 0f, 0f);
                }
            }
        }


        [HarmonyPatch(typeof(Intention_EnoughResidential), "Tick")]
        public static class AIPatch
        {
            static void Postfix(Intention_EnoughResidential __instance, float dt)
            {
                Dictionary<int, List<string>> houseTypeUniqueNamesLookup = new Dictionary<int, List<string>>()
                {
                    {0,  new List<string>() { "smallhouse", "slum" } },
                    {1, new List<string>() { "largehouse", "tallhovel", "rowhouse" } },
                    {2, new List<string>() { "manorhouse", "apartment", "commonshouse", "villa", "fancyhouse" } }
                };

                Dictionary<string, float> peoplePerHouse = new Dictionary<string, float>()
                {
                    { "smallhouse", 5f },
                    { "largehouse", 12f },
                    { "manorhouse", 25f },
                    { "slum", 55f },
                    { "tallhovel", 6f },
                    { "rowhouse", 20f },
                    { "apartment", 60f },
                    { "commonshouse", 40f},
                    { "villa", 30f },
                    { "fancyhouse", 8f }
                };


                int workers = Player.inst.TotalWorkersForLandMass(__instance.LandMass);
                int residentialSlots = Player.inst.TotalResidentialSlotsOnLandMass(__instance.LandMass);
                int num3 = World.inst.AvailableWorkersOnLandMass(__instance.LandMass, true);
                float scaledResidenceRequirement = (float)residentialSlots;
                if (__instance.Kingdom.skillLevel == AIKingdom.SkillLevel.Low)
                {
                    scaledResidenceRequirement = (float)(residentialSlots - 1);
                }
                else if (__instance.Kingdom.skillLevel == AIKingdom.SkillLevel.Medium)
                {
                    scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.9f, (float)(residentialSlots - 5));
                    if (workers >= 50)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.9f, (float)(residentialSlots - 10));
                    }
                    if (workers >= 150)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.9f, (float)(residentialSlots - 15));
                    }
                    if (workers >= 300)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.95f, (float)(residentialSlots - 15));
                    }
                }
                else
                {
                    scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.8f, (float)(residentialSlots - 5));
                    if (workers >= 50)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.8f, (float)(residentialSlots - 10));
                    }
                    if (workers >= 150)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.8f, (float)(residentialSlots - 20));
                    }
                    if (workers >= 300)
                    {
                        scaledResidenceRequirement = Mathff.Min((float)residentialSlots * 0.9f, (float)(residentialSlots - 25));
                    }
                }

                if (workers >= scaledResidenceRequirement && !__instance.completelyOutOfSpace)
                {
                    float deficit = workers - scaledResidenceRequirement;
                    int numHouses = 3;
                    int houseType = 0;
                    if (workers >= 50)
                    {
                        houseType++;
                    }
                    if (workers >= 150)
                    {
                        houseType++;
                    }

                    List<AIIntentionBase> subIntentions = (List<AIIntentionBase>)typeof(Intention_EnoughResidential).GetField("subIntentions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                    for(int i = 0; i < (subIntentions != null ? subIntentions.Count : 0); i++)
                    {
                        Intention_BuildBuilding intention_BuildBuilding = (Intention_BuildBuilding)subIntentions[i];
                        if (intention_BuildBuilding)
                        {
                            if (intention_BuildBuilding.failType == AIIntentionBase.FailType.LayoutBlockOutOfSpace)
                            {
                                string uniqueName = intention_BuildBuilding.pendingBuilding.UniqueName;
                                if (!(houseTypeUniqueNamesLookup[2].Contains(uniqueName)))
                                {
                                    if (!(houseTypeUniqueNamesLookup[1].Contains(uniqueName)))
                                    {
                                        if (houseTypeUniqueNamesLookup[0].Contains(uniqueName))
                                        {
                                            numHouses = 0;
                                            
                                            __instance.completelyOutOfSpace = false;
                                        }
                                    }
                                    else if (houseType > 0)
                                    {
                                        houseType = 0;
                                    }
                                }
                                else if (houseType > 1)
                                {
                                    houseType = 1;
                                }
                            }
                            else
                            {
                                if (intention_BuildBuilding.initSuccess)
                                {
                                    __instance.completelyOutOfSpace = false;
                                }
                                numHouses--;
                            }
                        }
                    }


                    while (deficit > 0f && numHouses > 0)
                    {
                        try
                        {
                            string house = "";
                            switch (houseType)
                            {
                                case 0:
                                    house = houseTypeUniqueNamesLookup[0][SRand.Range(0, houseTypeUniqueNamesLookup[0].Count)];
                                    deficit -= peoplePerHouse[house];
                                    break;
                                case 1:
                                    house = houseTypeUniqueNamesLookup[1][SRand.Range(0, houseTypeUniqueNamesLookup[1].Count)];
                                    deficit -= peoplePerHouse[house];
                                    break;
                                case 2:
                                    house = houseTypeUniqueNamesLookup[2][SRand.Range(0, houseTypeUniqueNamesLookup[2].Count)];
                                    deficit -= peoplePerHouse[house];
                                    break;
                            }

                            Intention_BuildBuilding newIntention = __instance.AddSubIntention<Intention_BuildBuilding>("build new house");
                            newIntention.PlaceNewBlockIfNeeded = (house == World.smallHouseName);
                            newIntention.Init(house, AIBuildingBlockType.Residential, __instance.Layout.KingdomStart, 100);

                            numHouses--;
                        }
                        catch(Exception ex)
                        {
                            helper.Log(ex.ToString());
                        }
                    }
                }
            }

            /*
            [HarmonyPatch (typeof(Road))]
            [HarmonyPatch ("ConnectsToRoad")]

            public static class RoadPatch
            {
                static bool Prefix(Building __instance)
                {

                        return __instance.categoryHash == "path".GetHashCode() || 
                            __instance.categoryHash == "gate".GetHashCode() || 
                            __instance.uniqueNameHash == "townsquare".GetHashCode() || 
                            __instance.categoryHash == "park".GetHashCode() || 
                            __instance.uniqueNameHash == "villa".GetHashCode() ||
                            __instance.uniqueNameHash == "slum".GetHashCode();


                }
            }
            */
        }
    }

}

