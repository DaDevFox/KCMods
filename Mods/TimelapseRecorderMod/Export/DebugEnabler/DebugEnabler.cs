using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace KCModUtils.Debugging
{
    public class DebugEnabler
    {
        public static KCModHelper helper;

        void Preload(KCModHelper helper)
        {
            DebugEnabler.helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Application.logMessageReceived += onLogMessageReceived;

            DebugEnabler.helper.Log(Application.isEditor.ToString());
        }

        private void onLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                DebugEnabler.helper.Log("Unhandled Exception: " + condition  + "\n" + stackTrace);
            if(type == LogType.Assert)
                DebugEnabler.helper.Log("Assertion: " + condition + "\n" + stackTrace);
            if(type == LogType.Error)
                DebugEnabler.helper.Log("Error: " + condition + "\n" + stackTrace);
        }


        [HarmonyPatch(typeof(Application))]
        [HarmonyPatch("isEditor", MethodType.Getter)]
        class ApplicationIsEditorPatch
        {
            static void Postfix(ref bool __result)
            {
                __result = true;
                DebugEnabler.helper.Log(__result.ToString());
            }
        }
    }
}
