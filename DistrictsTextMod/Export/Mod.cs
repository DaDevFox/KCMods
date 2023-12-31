using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace DistrictsTextMod
{
    public class Mod : MonoBehaviour
    {
        public static KCModHelper Helper { get; private set; }

        private static Dictionary<string, List<Building>> _districts = new Dictionary<string, List<Building>>();
        private static Dictionary<string,ResourceText> _text = new Dictionary<string, ResourceText>();

        public static float TextY { get; } = 2f;

        private static float UpdateInterval { get; } = 3f;

        private float time = 0f;


        public void Preload(KCModHelper helper)
        {
            Helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += (condition, stackTrace, type) => 
            {
                if (type == LogType.Exception)
                    Mod.Log("Exception: " + condition + "\n" + stackTrace);
                if (type == LogType.Assert)
                    Mod.Log("Assertion: " + condition + "\n" + stackTrace);
                if (type == LogType.Error)
                    Mod.Log("Error: " + condition + "\n" + stackTrace);
                
            };
        }

        public void SceneLoaded()
        {
            Broadcast.BuildingAddRemove.ListenAny(OnBuildingAddRemove);
        }


        public static void Log(object message) => Helper.Log(message.ToString());

        public void OnBuildingAddRemove(object sender, OnBuildingAddRemove data) => UpdateDistricts();

        void Start()
        {
            //ResourceText myText = GameUI.inst.ResourceText.Create("Lots of textity text text");
        }



        void Update()
        {
            if (time > UpdateInterval)
            {
                UpdateDistricts();
                time = 0f;
            }
            UpdateText();
            time += Time.deltaTime;
        }

        void UpdateDistricts()
        {
            _districts.Clear();

            List<Building> buildings = new List<Building>();

            for (int i = 0; i < World.inst.NumLandMasses; i++)
                foreach (Building building in Player.inst.GetBuildingListForLandMass(i).data)
                    buildings.Add(building);

            

            foreach (Building b in buildings)
            {
                if (b != null)
                {
                    if (GameState.inst.GetPlaceableByUniqueName(b.UniqueName) != null)
                    {
                        if (GameState.inst.GetPlaceableByUniqueName(b.UniqueName).customName != b.  customName)
                        {
                            if (_districts.ContainsKey(b.customName))
                                _districts[b.customName].Add(b);
                            else
                                _districts.Add(b.customName, new List<Building>());
                        }
                    }
                }
            }
        }

        void UpdateText()
        {
            foreach (string district in _districts.Keys)
            {
                if (!_text.ContainsKey(district))
                    _text.Add(district, GameUI.inst.ResourceText.Create(district));

                ResourceText text = _text[district];

                text.SetText(district);
                
                float sumX = 0f;
                foreach (Building b in _districts[district])
                    sumX += b.transform.position.x;

                float sumZ = 0f;
                foreach (Building b in _districts[district])
                    sumZ += b.transform.position.z;

                text.transform.localScale *= GetSizeModifierForDistrict(district);   

                text.transform.position = new Vector3(sumX / _districts[district].Count, TextY, sumZ / _districts[district].Count);
            }
            DeleteOldText();
        }

        private void DeleteOldText()
        {
            foreach (string district in _text.Keys)
                if (!_districts.ContainsKey(district))
                {
                    _text[district].Release();
                    _text.Remove(district);
                }
        }


        private float GetSizeModifierForDistrict(string district)
        {
            int count = 0;
            foreach (string _district in _districts.Keys)
                count += _districts[_district].Count;


            return ((float)_districts[district].Count) / (float)count;
        }


    }
}
