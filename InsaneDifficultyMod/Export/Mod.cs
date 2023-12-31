using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;
using System.Collections;

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
                }
                else if (Input.GetKeyDown(KeyCode.V))
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
                        //GameUI.instance.personUI.villager.MoveToDeferred(World.instance.cellsToLandmass[GameUI.instance.personUI.villager.landMass].RandomElement().Center);
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


        #region Coroutines

        public static void StartDroughtFadeCoroutine(Weather.WeatherType type)
        {
            mod.StartCoroutine("DroughtFadeCoroutine", type);
        }

        private IEnumerator DroughtFadeCoroutine(Weather.WeatherType type)
        {
            float time = Weather.inst.TransitionTime;
            float elapsed = 0f;

            Color originalLightColor = (Color)typeof(global::Weather).GetField("originalLightColor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::Weather.inst);
            float originalLightIntensity = (float)typeof(global::Weather).GetField("originalLightIntensity", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::Weather.inst);
            float originalLightShadowStrength = (float)typeof(global::Weather).GetField("originalLightShadowStrength", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::Weather.inst);

            float calculatedRainEmission = (float)typeof(global::Weather).GetField("calculatedRainEmission", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::Weather.inst);

            Timer lightningTimer = (Timer)typeof(global::Weather).GetField("lightningTimer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(global::Weather.inst);

            global::Weather.inst.Rain.Stop();
            global::Weather.inst.Snow.Stop();
            Color endValue = originalLightColor;
            float endValue2 = originalLightIntensity;
            float endValue3 = originalLightShadowStrength;
            float endValue4 = 0f;
            if (type != global::Weather.WeatherType.Snow)
            {
                if (type != global::Weather.WeatherType.NormalRain)
                {
                    if (type == global::Weather.WeatherType.HeavyRain)
                    {
                        global::Weather.inst.Rain.emissionRate = calculatedRainEmission;
                        global::Weather.inst.Rain.startSize = 0.8f;
                        endValue4 = 1f;
                        global::Weather.inst.Rain.Play();
                        endValue = new Color(0.5f, 0.65f, 0.85f);
                        endValue2 = 0.75f;
                        endValue3 = 0.4f;
                    }
                }
                else
                {
                    global::Weather.inst.Rain.emissionRate = calculatedRainEmission / 3f;
                    global::Weather.inst.Rain.startSize = 0.5f;
                    endValue4 = 0.8f;
                    global::Weather.inst.Rain.Play();
                    endValue = new Color(0.8f, 0.8f, 1f);
                    endValue2 = 0.9f;
                    endValue3 = 0.45f;
                }
            }
            else
            {
                global::Weather.inst.Snow.Play();
            }

            global::Weather.inst.currentWeather = type;
            if (lightningTimer.Enabled && global::Weather.inst.currentWeather != global::Weather.WeatherType.HeavyRain)
            {
                global::Weather.inst.lightningMadeFire = false;
            }

            lightningTimer.Enabled = (global::Weather.inst.currentWeather == global::Weather.WeatherType.HeavyRain || global::Weather.inst.currentWeather == global::Weather.WeatherType.LightningStorm);
            global::Weather.inst.Invoke("DeferNotifyBuildingsWeatherChanged", 5f);

            Color baseLightColor = Color.white;
            float baseIntensity = -1f;
            float baseShadowStrength = -1f;
            float baseStormAlpha = -1f;

            while (elapsed < time)
            {
                if (baseLightColor == Color.white)
                    baseLightColor = Weather.inst.Light.color;
                Weather.inst.Light.color = Color.Lerp(baseLightColor, endValue, elapsed / time);

                if (baseIntensity == -1f)
                    baseIntensity = Weather.inst.Light.intensity;
                Weather.inst.Light.intensity = Mathf.Lerp(baseIntensity, endValue2, elapsed / time);
                if (baseShadowStrength == -1f)
                    baseShadowStrength = Weather.inst.Light.shadowStrength;
                Weather.inst.Light.shadowStrength = Mathf.Lerp(baseShadowStrength, endValue3, elapsed / time);
                if (baseStormAlpha == -1f)
                    baseStormAlpha = CloudSystem.inst.stormAlpha;
                CloudSystem.inst.stormAlpha = Mathf.Lerp(baseStormAlpha, endValue4, elapsed / time);

                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion
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

