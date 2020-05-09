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

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Mod.dLog(helper.modPath);

            if(Settings.debug)
                Application.logMessageReceived += onLogMessageReceived;
        }

        private void onLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                Mod.dLog("Unhandled Exception: " + condition + stackTrace);
        }

        public void SceneLoaded(KCModHelper helper)
        {
            try
            {
                Mod.helper = helper;
                Mod.dLog("test2");
                Settings.Setup();

                Broadcast.OnSaveEvent.Listen(OnSave);
                Broadcast.OnLoadedEvent.Listen(OnLoad);
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

        #region SaveLoad

        public static void OnSave(object sender, OnSaveEvent loadedEvent)
        {
            LoadSave.SaveDataGeneric("elevation", "cellMarks", ElevationManager.Save());
        }

        public static void OnLoad(object sender, OnLoadedEvent loadedEvent)
        {
           ElevationManager.ReadFromLoad(LoadSave.ReadDataGeneric("elevation", "cellMarks"));
        }

        #endregion

    }
}
