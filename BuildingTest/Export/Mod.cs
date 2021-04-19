using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Harmony;

namespace BuildingTest
{
    public class Mod : MonoBehaviour
    {
        #region ModInfo

        public static string ModIdentifier { get; } = "Scorpion Tower";
        public static string AssetBundleName { get; } = "scorpionmodassets";

        #endregion

        public static KCModHelper Helper { get; private set; }

        public static GameObject ScorpionPrefab;
        public static GameObject ProjectilePrefab;



        private void Preload(KCModHelper helper)
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Helper = helper;








            

        }

        public static void CreateAssets()
        {
            AssetBundleManager.UnpackAssetBundle();

            ScorpionPrefab = AssetBundleManager.GetAsset<GameObject>("ScorpionTower.prefab");
            ProjectilePrefab = ScorpionPrefab.transform.Find("Offset/base/rotateBase/arrow").gameObject;
            
        }

        public static BuildingInfo CreateBuildingInfo()
        {
            BuildingInfo info = new BuildingInfo(ScorpionPrefab, "scorpiontower");

            info.tabCategory = "Castle";

            info.preqBuilding = "chamberofwar";

            info.customName = "Scorpion Tower";
            info.descOverride = "Massive tower with long reload time and enormous damage";
            info.categoryName = "projectiletopper";

            info.workersForFullYield = 20;

            info.buildAllowedWorkers = 30;
            info.buildersRequiredOnLocation = true;
            info.buildingCost = ScorpionTower.BuildingCost;
            info.buildingSize = new Vector3(2f, 1f, 2f);

            info.displayModel = ScorpionPrefab;

            info.jobCategory = JobCategory.Ballista;
            info.skillUsed = "Ballisteer";

            return info;
        }

        public static Projectile CreateProjectile()
        {
            Projectile projectile = ProjectilePrefab.AddComponent<Projectile>();



            return projectile;
        }

        public static Building CreateBuilding()
        {
            return ScorpionPrefab.AddComponent<Building>();
        }

        public static ProjectileDefense CreateProjectileDefense()
        {
            ProjectileDefense defense = ScorpionPrefab.AddComponent<ProjectileDefense>();

            defense. = 



            return defense;
        }



    }
}
