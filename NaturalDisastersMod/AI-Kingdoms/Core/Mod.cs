using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace Fox.AlphaDisasters
{
    public class Mod : MonoBehaviour
    {
        public static bool debug = false;

        public static Mod mod { get; private set; }
        public static string modID { get; } = "naturaldisastersmod";
        public static bool inited { get; private set; } = false;

        public static KCModHelper helper { get; private set; } = null;

        void Preload(KCModHelper _helper)
        {
            helper = _helper;
            mod = this;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += LogReceived;

            AssetBundleManager.UnpackAssetBundle();

            Log("Preload");
        }

        private void LogReceived(string condition, string stackTrace, LogType type)
        {
            if(type == LogType.Exception)
                Log(condition + "\n" + stackTrace);
        }

        public static void Log(object message)
        {
            if(helper != null)
                helper.Log(message.ToString());
        }

        void SceneLoaded(KCModHelper _helper)
        {
            //Events.EventManager.Init();
            //Settings.Init();
        }


        void Update()
        {
            if (!inited)
                Init();

            if (Settings.debug)
            {
                if (Input.GetKeyDown(KeyCode.T))
                    Events.EventManager.TriggerEvent(typeof(Events.EarthquakeEvent));
                
                if (Input.GetKeyDown(KeyCode.E))
                    Events.EventManager.TriggerEvent(typeof(Events.DroughtEvent));
                    
                if (Input.GetKeyDown(KeyCode.R))
                    Events.EventManager.TriggerEvent(typeof(Events.TornadoEvent));
                
                if(Input.GetKeyDown(KeyCode.M))
                    Events.EventManager.TriggerEvent(typeof(Events.MeteorEvent));
            }
        }

        private void Init()
        {
            Events.EventManager.Init();
            Settings.Init();
            inited = true;
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
