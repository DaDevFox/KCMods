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
    class DebugEnabler
    {
        void Preload()
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Application))]
        [HarmonyPatch("isEditor", MethodType.Getter)]
        class ApplicationIsEditorPatch
        {
            static void Postfix(ref bool __result)
            {
                __result = true;
            }
        }

        void SceneLoaded()
        {
            DebugEx.DrawArrow(new Vector3(0, 0, 20), new Vector3(0, 0, 0), Color.red);
        }

    }
}
