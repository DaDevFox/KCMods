﻿//#define ALPHA
//#define STABLE
//#define COMMON

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ReskinEngine.Engine
{
    public class Engine
    {
        public static event Action afterSceneLoaded;

        public static KCModHelper helper { get; set; }
        public static bool debug = true;


        public static string ReskinWorldLocation { get; } = "ReskinContainer";

        /// <summary>
        /// A dictionary of all the original instances of each type of skin
        /// </summary>
        public static Dictionary<string, SkinBinder> SkinLookup { get; } = new Dictionary<string, SkinBinder>();

        /// <summary>
        /// A dictionary of all registered mods, keyed by mod name. 
        /// </summary>
        internal static Dictionary<string, Mod> ModIndex { get; private set; } = new Dictionary<string, Mod>();

        public static List<string> ActiveMods { get; private set; }

        #region Initialization

        public void Preload(KCModHelper helper)
        {
            Engine.helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if (debug)
                Application.logMessageReceived += (condition, stack, type) =>
                {
                    if (type == LogType.Exception)
                        helper.Log($"ex:{condition} => {stack}");
                };

            helper.Log("Preload");
        }

        public void SceneLoaded(KCModHelper helper)
        {
            helper.Log("SceneLoaded");
        }

        #endregion

        #region Utilities

        public static void Log(object message) => helper.Log(message.ToString());

        public static void dLog(object message)
        {
            if (debug)
                helper.Log(message.ToString());
        }


        /// <summary>
        /// Gets the original SkinBinder for a specified type identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static SkinBinder GetOriginalBinder(string identifier)
        {
            if (SkinLookup.ContainsKey(identifier))
                helper.Log("found skin identifier " + identifier);

            return SkinLookup.ContainsKey(identifier) ? SkinLookup[identifier] : null;
        }

        public static Mod GetMod(string name)
        {
            return ModIndex.ContainsKey(name) ? ModIndex[name] : null;
        }

        /// <summary>
        /// Returns the mod with priority to bind the skin with the identifier skinIdentifier
        /// </summary>
        /// <param name="skinIdentifier"></param>
        /// <returns></returns>
        public static Mod GetPriority(string skinIdentifier)
        {
            if (Settings.priorityType == Settings.PriorityType.Absolute && ModIndex.Count > 0)
            {
                Mod result = null;
                foreach (Mod mod in ModIndex.Values)
                    if (mod.Binders.ContainsKey(skinIdentifier))
                        if (result == null || (result != null && mod.Priority > result.Priority))
                            result = mod;

                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a dictionary with skin id's as keys and skin binders as values
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<SkinBinder>> GetActivePool()
        {
            if (ModIndex.Keys.Count == 0)
                return new Dictionary<string, List<SkinBinder>>();

            return ModIndex[ModIndex.Keys.First()].Binders;
        }

        public static SkinBinder GetRandomBinderFromActive(string identifier)
        {
            if (GetPriority(identifier) == null || !GetPriority(identifier).Binders.ContainsKey(identifier))
                return null;

            List<SkinBinder> binders = GetPriority(identifier).Binders[identifier];

            return binders.Count > 0 ? binders[SRand.Range(0, binders.Count - 1)] : null;
        }

        public static SkinBinder GetBinderFromActive(string identifier)
        {
            if (!GetActivePool().ContainsKey(identifier))
                return null;

            List<SkinBinder> binders = GetActivePool()[identifier];

            return binders[0];
        }

        #endregion

        #region Setup

        static void AfterSceneLoaded()
        {
            InitLookup();
            ReadSkins();
            SetupMods();
            BindAll();
            BuildingSkinBinder.BindGameInternalPrefabs();

            afterSceneLoaded?.Invoke();

            helper.Log("AfterSceneLoaded complete");
        }

        private static void InitLookup()
        {
            SkinLookup.Clear();
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                if (type.GetCustomAttribute<UnregisteredAttribute>() != null)
                    continue;

                if (type.IsSubclassOf(typeof(SkinBinder)) && !type.IsAbstract)
                {
                    SkinBinder binder = Activator.CreateInstance(type) as SkinBinder;
                    SkinLookup.Add(binder.TypeIdentifier, binder);
                }

            }
        }

        private static void ReadSkins()
        {
            Transform target = World.inst.transform.Find(ReskinWorldLocation);

            if (!target)
                return;

            DeactivateWorldLocation();

            int childCount = target.childCount;
            for (int i = 0; i < childCount; i++)
            {
                SkinBinder binder = SkinBinder.Unpack(target.GetChild(i).gameObject);

                if (binder == null)
                    continue;

                if (!ModIndex.ContainsKey(binder.ModName))
                    ModIndex.Add(binder.ModName, Mod.Create(binder.ModName, binder.CompatabilityIdentifier));

                ModIndex[binder.ModName].Add(binder);
            }
        }

        private static void DeactivateWorldLocation()
        {
            World.inst.transform.Find(ReskinWorldLocation).gameObject.SetActive(false);
        }

        private static void SetupMods()
        {
            Settings.Setup();
        }


        private static void BindAll()
        {
            GetActivePool().Values.Do(binders => binders.Do((binder) => binder.Bind()));
        }

        #endregion

        [HarmonyPatch(typeof(KCModHelper.ModLoader), "SendSceneLoadSignal")]
        class AfterSceneLoadedPatch
        {
            static void Postfix()
            {
                helper.Log("After SceneLoaded");
                AfterSceneLoaded();
            }
        }


        public static void Exception(Exception ex)
        {
            helper.Log(ex.ToString());
        }

    }
}
