using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;

namespace InsaneDifficultyMod
{
    public class Mod : MonoBehaviour
    {
        public static GameObject modSettings;
        private GameObject modSettingsPrefab;

        public static Mod mod;

        public static string modID = "insanedifficultymod";

        public static KCModHelper helper;

        void Preload(KCModHelper _helper)
        {
            helper = _helper;
            mod = this;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            AssetBundleManager.UnpackAssetBundle();
        }


        void SceneLoaded(KCModHelper _helper)
        {
            Setup();
        }

        void Setup() 
        {
            Settings.ApplyGameVars();
            UI.Setup();
            Events.EventManager.Init();
        }
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                ModSettingsUI._base.SetActive(!ModSettingsUI._base.activeSelf);
            }

            #region Debug

            if (Settings.debug)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.EarthquakeEvent));
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    KingdomLog.TryLog("eat" + SRand.Range(1f, 100f).ToString(), "eat rate " + Player.inst.SecondsPerEat.ToString(), KingdomLog.LogStatus.Important);
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.RiotEvent));
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.DroughtEvent));
                }
            }

            #endregion
        
        }


    }

    #region HarmonyPatches

    [HarmonyPatch(typeof(Player), "OnNewYear")]
    public class YearPatch
    {
        static void Postfix()
        {
            Events.EventManager.OnYearEnd();
        }

    }

    #endregion

}

