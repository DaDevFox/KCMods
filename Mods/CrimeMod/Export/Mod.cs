using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;
using CrimeMod.CriminalTypes;

namespace CrimeMod
{
    public class Mod : MonoBehaviour
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
            Mod.helper.Log("Scene Loaded");
        }

        void Update()
        {
            if (Settings.Debug)
            {
                if (Input.GetKeyDown(Settings.keycode_checkCrime))
                {
                    CrimeManager.CheckHomesForCrime();
                }
            }
        }

        [HarmonyPatch(typeof(Player),"OnNewYear")]
        static class OnYearEndPatch
        {
            static void Postfix()
            {
                CrimeManager.CheckHomesForCrime();
            }
        }


        public static void Log(string message)
        {
            if (Settings.Debug)
                Mod.helper.Log(message);
        }
    }
}
