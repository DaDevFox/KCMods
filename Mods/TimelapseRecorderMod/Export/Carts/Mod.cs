using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;
using Assets.Code;

namespace Elevation.Carts
{
    public class Mod
    {
        public static KCModHelper helper { get; private set; }
        public static string assetbundleName { get; } = "fox_carts";
        public static AssetDB assets { get; private set; }


        private void Preload(KCModHelper helper)
        {
            HarmonyInstance harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll();

            Mod.helper = helper;

            assets = AssetBundleManager.Unpack(helper.modPath + "/assets", assetbundleName);

            Application.logMessageReceived += LogMessage;
        }

        private void LogMessage(string condition, string stackTrace, LogType type)
        {
            if(type == LogType.Exception || type == LogType.Error)
                Log(condition + "\n" + stackTrace);
        }

        public static void Log(object message)
        {
            Mod.helper.Log(message.ToString());
        }

        [HarmonyPatch(typeof(Villager), "Awake")]
        private class VillagerCreatePatch
        {
            static void Postfix(Villager __instance)
            {
                __instance.gameObject.AddComponent<Cart>().villager = __instance;
            }

        }
    }

    /// <summary>
    /// Component that increases villager capacity and instantiates model when attached to a villager's gameobject
    /// </summary>
    public class Cart : MonoBehaviour
    {
        public static string cartPrefabName { get => "Cart"; }

        public static ResourceAmount carryCapacity
        {
            get
            {
                ResourceAmount capacity = new ResourceAmount();

                capacity.Set(FreeResourceType.Apples, 10);
                capacity.Set(FreeResourceType.Armament, 10);
                capacity.Set(FreeResourceType.Charcoal, 10);
                capacity.Set(FreeResourceType.DeadVillager, 2);
                capacity.Set(FreeResourceType.Fish, 10);
                capacity.Set(FreeResourceType.IronOre, 10);
                capacity.Set(FreeResourceType.Pork, 10);
                capacity.Set(FreeResourceType.Stone, 10);
                capacity.Set(FreeResourceType.Tools, 10);
                capacity.Set(FreeResourceType.Tree, 10);
                capacity.Set(FreeResourceType.Wheat, 10);

                return capacity;
            }
        }

        public static Vector3 shoulderMount { get => new Vector3(0.024f, 0.049f, 0.047f); }
        public static Vector3 bodyMount { get => new Vector3(0.146f, 0.076f, 0.0142f); }

        public Villager villager;

        private void Start()
        {
            typeof(Villager).GetField("_defaultCarryCapacity", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(villager, Cart.carryCapacity);

            //villager.ShoulderMount.transform.position = shoulderMount;
            //villager.DeadBodyCarryMount.transform.position = bodyMount;

            //GameObject prefab = Mod.assets.GetByName<GameObject>(cartPrefabName);

            //GameObject.Instantiate(prefab, villager.transform);
        }



        
    }
}
