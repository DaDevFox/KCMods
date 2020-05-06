using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace NaturalDisastersMod
{
    class Mod : MonoBehaviour
    {

        public static Mod mod;

        public static string modID = "naturaldisastersmod";

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
            Events.EventManager.Init();
        }


        void Update()
        {
            if (Settings.debug)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.EarthquakeEvent));
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.DroughtEvent));
                    
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Events.EventManager.TriggerEvent(typeof(Events.TornadoEvent));
                }
            }
        }


        [HarmonyPatch(typeof(Player), "OnNewYear")]
        public class YearPatch
        {

            static void Postfix()
            {
                Events.EventManager.OnYearEnd();
            }

        }


    }
}
