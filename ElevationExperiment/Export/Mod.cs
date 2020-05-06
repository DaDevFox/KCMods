using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ElevationExperiment
{
    public class Mod : MonoBehaviour
    {
        public static KCModHelper helper;
        public static string modID = "elevationexperiment";


        public void Preload(KCModHelper helper)
        {
            Mod.helper = helper;

            try
            {

                var harmony = HarmonyInstance.Create("harmony");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                //Mod.Log("test");
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }

        public void SceneLoaded()
        {
            try
            {
                Settings.Setup();
                
            }
            catch (Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }


        void Update()
        {
            InputManager.Update();
        }

        public static void dLog(object msg)
        {
            if (Settings.debug)
                Mod.helper.Log(msg.ToString());
        }


        public static void Log(object msg)
        {
            Mod.helper.Log(msg.ToString());
        }


        [HarmonyPatch(typeof(World), "GenLand")]
        public class GameStartPatch
        {
            static void Postfix()
            {
                try
                {
                    Mod.Log("--- Preperation ---");
                    ColorManager.Setup();
                    ElevationManager.SetupCellMarks();
                    Mod.Log("--- Preperation Complete ---");
                }
                catch(Exception ex)
                {
                    DebugExt.HandleException(ex);
                }
            }
        }


    }
}
