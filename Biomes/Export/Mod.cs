using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using Fox.Localization;

namespace Biomes
{
    public class Mod : MonoBehaviour
    {
        //TODO: Process Chain
        public static event Action Init;
        public static event Action Setup;
        public static event Action Tick;


        public static KCModHelper helper { get; private set; }

        private void Preload(KCModHelper helper)
        {
            Mod.helper = helper;

            var harmony = HarmonyInstance.Create("kc.fox.biomes");
            harmony.PatchAll();
        }

        private void SceneLoaded()
        {
            Init?.Invoke();
        }

        private void Update()
        {
            Tick?.Invoke();
        }

        [HarmonyPatch(typeof(World), "Generate")]
        static class PostGeneratePatch
        {
            static void Postfix()
            {
                Setup?.Invoke();
            }
        }
    }

    public static class Engine
    {
        private static Dictionary<int, CellType> _data = new Dictionary<int, CellType>();

        public static int Register(CellType type)
        {
            int id = GetFreeId();
            _data.Add(id, type);
            Localization.Append(type.localization);
            return id;
        }

        private static int GetFreeId()
        {
            int id = -1;
            int i = 0;
            while(id != -1)
            {
                if (!_data.ContainsKey(i))
                    id = i;
                i++;
            }
            return id;
        }

        public static CellType? Get(string name)
        {
            foreach(KeyValuePair<int, CellType> pair in _data)
            {
                if (pair.Value.name != name)
                    continue;
                return pair.Value;
            }
            return null;
        }

        public static CellType? Get(int id) => _data.ContainsKey(id) ? (CellType?)_data[id] : null;

        public static string GetDescription(int id)
        {
            CellType? found = Get(id);
            if(found != null)
            {
                Localization.Get($"{((CellType)found).name}_description");
            }
            return "Could not find cell type";
        }
    }

    public struct CellType
    {
        /// <summary>
        /// Name of this cell type
        /// </summary>
        public string name;
        /// <summary>
        /// Identifier of this cell type
        /// </summary>
        public int id;
        /// <summary>
        /// color of this cell type
        /// </summary>
        public Color color;
        /// <summary>
        /// Localization data (to be used by LocalizationManager) for this cell type's description; make term to have format <c>{CellType.name}_description</c>
        /// </summary>
        public string localization;
    }
}
