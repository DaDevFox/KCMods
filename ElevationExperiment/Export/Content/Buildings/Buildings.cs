using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code;
using Harmony;
using UnityEngine;

namespace Elevation
{
    //[HarmonyPatch(typeof(BuildUI))]
    //[HarmonyPatch("Awake")]
    public class Buildings
    {
        //static void Postfix()
        //{
        //    Register();
        //}

        // TODO: Buildings
        public static void Register()
        {
            GameObject prefab = GameState.inst.GetPlaceableByUniqueName("destructioncrew").gameObject;
            GameObject.Destroy(prefab.GetComponent<DestructionCrew>());
            Dugout b = prefab.AddComponent<Dugout>();

            BuildingInfo info = new BuildingInfo(prefab.gameObject, "dugout")
            {
                tabCategory = "Castle",
                buildingCost = ResourceAmount.Make(FreeResourceType.Tree, 20),
                buildersRequiredOnLocation = true,
                building = prefab.GetComponent<Building>(),
                buildingPrefab = prefab,
                buildingSize = new Vector3(1f, 1f, 1f),
                buildAllowedWorkers = 10,
                placementSounds = new string[] { "castleplacement" },
                jobCategory = JobCategory.Undefined,
                ignoreRoadCoverageForPlacement = true,
                doBuildAnimation = true,
                allowOverAndUnderAqueducts = true,
                dragPlacementMode = Building.DragPlacementMode.Rectangle,
                uniqueName = "dugout",
                stackable = false,
                displayModel = null,
            };

            BuildingHelper.RegisterBuilding(info);
        }







    }
}
