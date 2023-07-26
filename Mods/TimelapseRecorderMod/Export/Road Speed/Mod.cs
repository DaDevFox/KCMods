using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Zat.Shared.ModMenu.Interactive;
using Zat.Shared.ModMenu.API;
using UnityEngine;

namespace Fox.RoadSpeedAdjuster
{
    public class Mod
    {
        public static KCModHelper helper { get; private set; }

        public static Settings settings { get; private set; }

        [Mod("Road Speed Adjuster", "1.0", "Fox")]
        public class Settings
        {
            public ModSettingsProxy proxy;
            public InteractiveConfiguration<Settings> config;

            [Setting("Road/Normal")]
            [Slider(0.1f, 50f, 5f, "5")]
            public InteractiveSliderSetting s_normalRoadSpeed { get; private set; }
            [Setting("Road/Stone")]
            [Slider(0.1f, 50f, 10f, "10")]
            public InteractiveSliderSetting s_stoneRoadSpeed { get; private set; }
            [Setting("Bridge/Normal")]
            [Slider(0.1f, 50f, 2.5f, "2.5")]
            public InteractiveSliderSetting s_normalBridgeSpeed { get; private set; }
            [Setting("Bridge/Stone")]
            [Slider(0.1f, 50f, 10f, "10")]
            public InteractiveSliderSetting s_stoneBridgeSpeed { get; private set; }
            [Setting("Bridge/Drawbridge")]
            [Slider(0.1f, 50f, 10f, "10")]
            public InteractiveSliderSetting s_drawbridgeSpeed { get; private set; }
        }

        public static float normalRoadSpeed { get => settings.s_normalRoadSpeed ?? 5f; }
        public static float stoneRoadSpeed { get => settings.s_stoneRoadSpeed ?? 10f; }
        public static float normalBridgeSpeed { get => settings.s_normalBridgeSpeed ?? 2.5f; }
        public static float stoneBridgeSpeed { get => settings.s_stoneBridgeSpeed ?? 10f; }
        public static float drawbridgeSpeed { get => settings.s_drawbridgeSpeed ?? 10f; }


        private void Preload(KCModHelper helper)
        {
            HarmonyInstance harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll();

            Mod.helper = helper;
        }

        private void SceneLoaded()
        {
            var config = new InteractiveConfiguration<Settings>();
            settings = config.Settings;
            settings.config = config;

            settings.s_normalRoadSpeed.OnUpdatedRemotely.AddListener(UpdateSlider);
            settings.s_stoneRoadSpeed.OnUpdatedRemotely.AddListener(UpdateSlider);
            settings.s_normalBridgeSpeed.OnUpdatedRemotely.AddListener(UpdateSlider);
            settings.s_stoneBridgeSpeed.OnUpdatedRemotely.AddListener(UpdateSlider);
            settings.s_drawbridgeSpeed.OnUpdatedRemotely.AddListener(UpdateSlider);

            ModSettingsBootstrapper.Register(config.ModConfig,
                (proxy, oldSettings) =>
                {
                    settings.proxy = proxy;
                    settings.config.Install(proxy, oldSettings);
                    Mod.helper.Log("Registered");
                },
                (ex) => {
                    Mod.helper.Log($"Failed to register:\n {ex.ToString()}");
                });
        }

        private void UpdateSlider(SettingsEntry slider)
        {
            slider.slider.label = Round(slider.slider.value, 0.1f).ToString();
            settings.proxy.UpdateSetting(slider, () => { }, (ex) => Mod.helper.Log(ex.ToString()));
        }

        private float Round(float val, float factor)
        {
            return Mathf.Round(val / factor) * factor;
        }

        [HarmonyPatch(typeof(Villager), "UpdateSpeed")]
        private class SpeedPatch
        {
            static void Prefix(Villager __instance)
            {

                if (__instance.cell != null)
                {
                    if (__instance.cell.OccupyingStructure.Count > 0)
                    {
                        if (__instance.cell.OccupyingStructure[0].uniqueNameHash == World.stoneRoadHash)
                            __instance.cell.speedModifier = stoneRoadSpeed;
                        if (__instance.cell.OccupyingStructure[0].uniqueNameHash == World.stoneBridgeHash)
                            __instance.cell.speedModifier = stoneBridgeSpeed;
                        if (__instance.cell.OccupyingStructure[0].uniqueNameHash == World.roadHash)
                            __instance.cell.speedModifier = normalRoadSpeed;
                        if (__instance.cell.OccupyingStructure[0].uniqueNameHash == World.bridgeName.GetHashCode())
                            __instance.cell.speedModifier = normalBridgeSpeed;
                        if (__instance.cell.OccupyingStructure[0].uniqueNameHash == World.drawBridgeHash)
                            __instance.cell.speedModifier = drawbridgeSpeed;
                    }
                }
            }
        }
    }
}
