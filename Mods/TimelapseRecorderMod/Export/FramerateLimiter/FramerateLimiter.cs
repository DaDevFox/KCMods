using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zat.Shared.ModMenu.Interactive;
using Zat.Shared.ModMenu.API;


namespace FramerateLimiterMod
{
    public class FramerateLimiter : MonoBehaviour
    {
        [Mod("Framerate Limiter", "1.0", "Agentfox")]
        public class Settings
        {
            public static Settings inst;
            public InteractiveConfiguration<Settings> Config { get; set; }

            [Slider(1f, 400f, 100f, "", true)]
            public InteractiveSliderSetting frameRate { get; private set; }
            public bool logging { get; set; }
        }

        public void SceneLoaded(KCModHelper helper)
        {
            var config = new InteractiveConfiguration<Settings>();
            Settings.inst = config.Settings;
            Settings.inst.Config = config;

            Settings.inst.frameRate.OnUpdatedRemotely.AddListener((setting) => UpdateFramerate(setting));

            ModSettingsBootstrapper.Register(config.ModConfig,
                (proxy, entries) =>
                {
                    if (Settings.inst.logging) helper.Log("Registered successfully");
                },
                (ex) => helper.Log(ex.ToString()));
        }


        private void UpdateFramerate(SettingsEntry setting)
        {
            Settings.inst.frameRate.Value = setting.slider.value;
            Application.targetFrameRate = (int)Settings.inst.frameRate.Value;
        }

    }
}
