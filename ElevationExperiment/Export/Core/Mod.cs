﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Harmony;
using UnityEngine;
using Fox.Profiling;
using Fox.Localization;
using Fox.Debugging;

namespace Elevation
{
    public class Mod : MonoBehaviour
    {
        public static KCModHelper helper;

        #region Mod Info

        public static string ModID { get; } = "elevationexperiment";
        public static string AssetBundlePath => helper.modPath + AssetBundleRelativePath;
            
        public static string AssetBundleRelativePath { get; } = "/Content/Assets/assetbundle/";

        public static string AssetBundleName { get; } = "fox_elevation";

        #endregion

        #region Localization

        public static string localizationData { get; } = "en, de, fr, es" +
            "generating_title,       Generating..." +
            "generating_description, This may take a while";

        #endregion

        /*  Method Nomenclature:
        *  --------------------
        *  Init:   Called once after game has started (Preload)
        *  Setup:  Called before world generation each time a new world generates
        *  Tick:   Called every frame (alias of Unity's Update, to avoid confusion with other methods named Update)
        */

        // TODO: Shift to Events
        // TODO: Work on call chain/initialization chain
        // TODO: Call Chain with attributes

        public static event Action Init;
        public static event Action Setup;
        public static event Action Tick;

        public void Preload(KCModHelper helper)
        {
            // Helper
            Mod.helper = helper;

            // Logging
            if (Settings.debug)
                Application.logMessageReceived += onLogMessageReceived;

            // Harmony
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            
            // Localization
            // DONE: Localization Data
            Localization.Init(localizationData);
            
            // Profiler
            if (Settings.debug)
                SelectiveProfiler.instance.Init();
            
            // Assets
            ModAssets.Init();

            // API
            API.API.Init();

            // Visuals
            Rendering.Init();

            // Other
            Init?.Invoke();

            // TODO: Road Stairs Models

        }


        public void SceneLoaded()
        {
            // Save/Load
            Broadcast.OnSaveEvent.Listen(OnSave);
            Broadcast.OnLoadedEvent.Listen(OnLoad);

            // Settings
            Settings.Init();
            
            //// Buildings
            //Buildings.Register();
        }

        [Profile]
        private void Update()
        {
            // Input
            InputManager.Tick();
            // Visuals
            Rendering.Tick();
            // Colors
            ColorManager.Tick();
            // Debug Lines
            DebugLines.Tick();
        }

        
        private void onLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
                Mod.Log("Unhandled Exception: " + condition + "\n" + stackTrace);
            if (type == LogType.Assert)
                Mod.dLog("Assertion: " + condition + "\n" + stackTrace);
            if (type == LogType.Error)
                Mod.Log("Error: " + condition + "\n" + stackTrace);
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

        #region Setup

        public static void SetupAll()
        {
            // Coloring
            ColorManager.Setup();

            // Grid
            Grid.Setup();

            // Create Visuals
            Rendering.Setup();

            Setup?.Invoke();
        }

        [HarmonyPatch(typeof(World), "GenLand")]
        public class GameStartPatch
        {
            static void Postfix()
            {
                Mod.Log("--- Preperation ---");
                Mod.SetupAll();
                Mod.Log("--- Preperation Complete ---");
            }
        }

        [HarmonyPatch(typeof(World), "Generate")]
        public class PostGeneratePatch
        {
            static void Postfix()
            {
                Mod.Log("--- Generating ---");

                // Map Generation
                MapGenerator.Generate();

                // Pathfinding
                ElevationPathfinder.current.Init(World.inst.GridWidth, World.inst.GridHeight);

                // Update Visuals
                ElevationManager.RefreshTerrain();

                Mod.Log("--- Generation Complete ---");

            }
        }

        #endregion

        #region SaveLoad

        public static void OnSave(object sender, OnSaveEvent loadedEvent)
        {
            LoadSave.SaveDataGeneric("elevation", "grid", Grid.Save());
        }

        public static void OnLoad(object sender, OnLoadedEvent loadedEvent)
        {
           Grid.Load(LoadSave.ReadDataGeneric("elevation", "grid"));
        }

        #endregion

    }
}