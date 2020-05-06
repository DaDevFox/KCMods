using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace TimelapseRecorderMod
{
    public class Mod
    {
        #region Mod Vars

        //Mod
        public static KCModHelper helper;
        public static string modID = "crimemod";

        //AssetBundle
        public static string assetBundleName = "crimemodassets";
        public static string assetBundlePath = "/assetbundle/";

        #endregion

        void Preload(KCModHelper helper)
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Mod.helper = helper;
        }


        void SceneLoaded()
        {
            GameObject obj = GameObject.Instantiate(new GameObject());
            obj.AddComponent<DebugUI>();
            Mod.helper.Log("Scene Loaded");
        }


        [HarmonyPatch(typeof(DebugUI),"Start")]
        static class TimelapseEnablePatch
        {
            static void Postfix(DebugUI __instance)
            {
                Timer gifTimer = typeof(DebugUI).GetFieldValue<Timer>("manualGifTimer");
                gifTimer.Enabled = true;
                __instance.gameObject.SetActive(true);
                Mod.helper.Log("test");
            }
        }

    }
}
