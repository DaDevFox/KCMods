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
        public static AssetDB assets { get; private set; }
        public static AssetDB legacyAssets { get; private set; }

        public static string modID = "fox_insanedifficulty";
        public static string legacyModID = "insanedifficultymod";

        public static KCModHelper helper;

        void Preload(KCModHelper _helper)
        {
            helper = _helper;
            mod = this;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            assets = AssetBundleManager.Unpack(helper.modPath + "/assetbundle/", modID);
            //legacyAssets = AssetBundleManager.Unpack(helper.modPath + "/legacy_assetbundle/", legacyModID);
            if (Settings.debug)
                Application.logMessageReceived += onLogMessageReceived;
        }

        private void onLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                Mod.dLog("Unhandled Exception: " + condition + "\n" + stackTrace);
        }


        public static void dLog(object message)
        {
            if(Settings.debug)
                Mod.Log(message);
        }

        public static void Log(object message)
        {
            Mod.helper.Log(message.ToString());
        }



        void SceneLoaded(KCModHelper _helper)
        {
            Setup();
        }

        void Setup() 
        {
            Settings.Setup();
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
                    Events.RiotSystem.Iterate();
                }else if (Input.GetKeyDown(KeyCode.V))
                {
                    Events.RiotSystem.EndAll();
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (GameUI.inst.personUI.villager != null)
                    {
                        Villager villager = GameUI.inst.personUI.villager;
                        float activeSpeed = (float)typeof(Villager).GetField("activeSpeed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameUI.inst.personUI.villager);
                        bool ignoreDeferred = (bool)typeof(Villager).GetField("ignoreDeferred", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameUI.inst.personUI.villager);
                        DebugExt.Log($"Villager: paralyzed={GameUI.inst.personUI.villager.paralyzed}\ntravelPath={GameUI.inst.personUI.villager.travelPath.Count}\nactiveSpeed={activeSpeed}\nignoreDeferred={ignoreDeferred}");
                        //GameUI.inst.personUI.villager.MoveToDeferred(World.inst.cellsToLandmass[GameUI.inst.personUI.villager.landMass].RandomElement().Center);
                    }
                }
                //if (Input.GetKeyDown(KeyCode.E))
                //{
                //    Events.EventManager.TriggerEvent(typeof(Events.DroughtEvent));
                //}
            }

            Events.RiotSystem.Update();

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

