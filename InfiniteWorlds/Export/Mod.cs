using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace Elevation.InfiniteWorlds
{
    public class Mod : MonoBehaviour
    {
        public static KCModHelper helper { get; private set; }

        public static void Log(object message) => helper.Log(message.ToString());

        private void Preload(KCModHelper helper)
        {
            Mod.helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += LogMessage;

            Init();
        }

        private void LogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
                Mod.helper.Log(condition + "\n" + stackTrace);
        }


        private void Init()
        {
            //TerrainGen.inst.mapGen.rasterBorder = 0;
        }
    }
}
