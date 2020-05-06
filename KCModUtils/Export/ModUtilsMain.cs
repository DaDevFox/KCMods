using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace KCModUtils
{
    public class ModUtilsMain : MonoBehaviour
    {
        /// <summary>
        /// The helper used to print output messages
        /// </summary>
        public static KCModHelper Helper { get; set; }
        /// <summary>
        /// The unique identifier of this mod
        /// </summary>
        public static string ModID { get; set; }
        /// <summary>
        /// The name of the assetBundle this mod uses
        /// </summary>
        public static string AssetBundleName { get; set; }
        /// <summary>
        /// The path of the AssetBundle relative to the root of the mod folder
        /// </summary>
        public static string AssetBundleRelativePath { get; set; }
        /// <summary>
        /// wheter or not this is a debug build; if not, it will print less output errors
        /// </summary>
        public static bool debug = true;

        

        static void Prelaod(KCModHelper helper)
        {
            ModUtilsMain.Helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}