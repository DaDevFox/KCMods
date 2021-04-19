using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace TotallyAccurateModEngine
{
    public class Engine : Singleton<Engine>
    {
        public Engine()
        {
            this.Init();
        }

        private void Init()
        {
            Debug.Log("TAME Engine Init");
            Application.logMessageReceived += OnLogMessageRecieved;

            LoadMods();
        }

        private void LoadMods()
        {
            Assembly main = Assembly.GetExecutingAssembly();
            string directory = Path.Combine(main.Location, "mods");

            string[] mods = Directory.GetFiles(directory, "*.dll");

            foreach(string modPath in mods)
            {
                if (!modPath.Contains("TABS.Mods"))
                    continue;

                Debug.Log("Loading mod at path " + modPath);

                var assembly = Assembly.LoadFile(modPath);

                if (assembly.GetType("TABS.Mods.Main") != null)
                    assembly.CreateInstance("TABS.Mods.Main");
            }
        }

        private void OnLogMessageRecieved(string condition, string stackTrace, LogType logType)
        {

        }

    }
}
