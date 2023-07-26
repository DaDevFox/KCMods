using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Assets.Code;
using Zat.Shared.ModMenu.Interactive;
using Zat.Shared.ModMenu.API;
using UnityEngine;
using System.Reflection;

namespace InsaneDifficultyMod
{
    [Mod("Insane Difficulty Mod", "0.3.1 Beta", "Agentfox")]
    public class Settings
    {
        public static Settings inst { get; private set; }
        public InteractiveConfiguration<Settings> Config { get; private set; }
        public ModSettingsProxy Proxy { get; private set; }

        public static Dictionary<int, string> Modes { get; } = new Dictionary<int, string>()
        {
            { 0, "Easy" },
            { 1, "Difficult" },
            { 2, "Challenging" },
            { 3, "Expert" },
            { 4, "Insane" }
        };


        #region Interactive

        public static int Mode {
            get
            {
                if (Settings.inst != null)
                    return (int)Settings.inst?.s_Mode.Value;
                else
                    return 0;
            }
            set
            {
                if (Settings.inst != null)
                    Settings.inst.s_Mode.Value = value;
            }
        }

        [Setting("Difficulty Preset", "A preset of settings tailored to a specific difficulty")]
        [Slider(0f, 4f, 0f, "Easy", true)]
        public InteractiveSliderSetting s_Mode { get; private set; }

        public static bool debug = true;


        [Setting("Events/Random Events", "Whether or not random events will be triggered; if also using Natural Disasters, turn this off or else twice as many disasters will be spawned")]
        [Toggle(true, "")]
        public InteractiveToggleSetting s_RandomEvents { get; private set; }

        public static bool RandomEvents
        {
            get
            {
                if (Settings.inst != null)
                    return Settings.inst.s_RandomEvents.Value;
                else
                    return true;
            }
            set
            {
                if (Settings.inst != null)
                    Settings.inst.s_RandomEvents.Value = value;
            }
        }

        [Setting("Events/Earthquakes/Formation Chance", "The chance of an earthquake happening at the beggining of a year")]
        [Slider(0.05f, 0.95f, 0.15f, "0.15")]
        public InteractiveSliderSetting s_EarthquakeChance { get; private set; }

        public static float EarthquakeChance {
            get
            {
                if (Settings.inst != null)
                    return Settings.inst.s_EarthquakeChance.Value;
                else
                    return 0.15f;
            }
            set
            {
                if (Settings.inst != null)
                    Settings.inst.s_EarthquakeChance.Value = value;

            }
        }

        [Setting("Events/Earthquakes/Minimum Strength", "The min magnitude of an earthquake. ")]
        [Slider(1f, 10f, 1f, "1", true)]
        public InteractiveSliderSetting s_EarthquakeStrengthMin { get; private set; }

        [Setting("Events/Earthquakes/Maximum Strength", "The max magnitude of an earthquake. ")]
        [Slider(1f, 10f, 4f, "1", true)]
        public InteractiveSliderSetting s_EarthquakeStrengthMax { get; private set; }

        public static MinMax EarthquakeStrength {
            get
            {
                if (Settings.inst != null)
                    return new MinMax(Mathf.Clamp(Settings.inst.s_EarthquakeStrengthMin.Value, 0, Settings.inst.s_EarthquakeStrengthMax.Value - 1f), Settings.inst.s_EarthquakeStrengthMax.Value);
                else
                    return null;
            }
            set
            {
                if (Settings.inst != null)
                    SetMinMax(Settings.inst.s_EarthquakeStrengthMin, Settings.inst.s_EarthquakeStrengthMax, value);
            }
        }

        [Setting("Events/Earthquakes/Randomness", "The randomness of the earthquake line")]
        [Slider(0.05f, 0.95f, 0.8f, "0.8", false)]
        public InteractiveSliderSetting s_EarthquakeVariance { get; private set; }

        public static MinMax EarthquakeVariance
        {
            get
            {
                if (Settings.inst != null)
                    return new MinMax(0.5f - inst.s_EarthquakeVariance.Value / 2f, 0.5f + inst.s_EarthquakeVariance.Value / 2f);
                else
                    return new MinMax(0.1f, 0.9f);
            }
            set
            {
                if (Settings.inst != null)
                    inst.s_EarthquakeVariance.Value = value.Max - value.Min;
            }
        }

        public static MinMax EarthquakeLandElevation = new MinMax(0f, 0.3f);
        public static MinMax EarthquakeWaterElevation = new MinMax(-2f, -0.25f);


        public static float DroughtChance { get; private set; } = 5f;
        public static MinMax DroughtLength { get; private set; } = new MinMax(1, 2);
        public static ResourceAmount droughtFoodPenalty = new ResourceAmount();

        public static bool HappinessMods { get; private set; } = true;

        public static float FoodPenalty { get; private set; } = 2f;

        //TODO: convert to minmax
        public static int VikingRaidYearSpan { get; private set; } = 3;
        public static int VikingRaidYearRange { get; private set; } = 1;

        //DONE: Added a modifier for viking raid strength
        public static float VikingEscalationModifier { get; private set; } = 2.1f;

        public static MinMax DragonAttackYearSpan { get; private set; } = new MinMax(6, 11);
        public static MinMax DragonAmountPerAttack { get; private set; } = new MinMax(1, 20);

        public static MinMax FireBurnoutTimeRange { get; private set; } = new MinMax(8f, 15f);
        public static float FirePersistance { get; private set; } = 1f;

        public static int minArmyAmountForOppression { get; } = 3;
        public static int oppresssionRamp { get; } = 8;
        public static int PopulationOppressionUnit { get; private set; } = 100;
        public static int MinOppressionHappiness { get; private set; } = -40;
        public static int MaxOppressionHappiness { get; private set; } = 40;

        public static int riotMaxSize = 20;
        public static int riotStartSize = 10;

        #endregion

        public Settings()
        {
            droughtFoodPenalty.Set(FreeResourceType.Wheat, 4);
            droughtFoodPenalty.Set(FreeResourceType.Apples, 4);
        }

        public static void SetMinMax(InteractiveSliderSetting min, InteractiveSliderSetting max, MinMax newValue)
        {
            min.Value = newValue.Min;
            max.Value = newValue.Max;
        }

        public static void OnMinMaxUpdate(InteractiveSliderSetting min, InteractiveSliderSetting max)
        {
            if (min.Value > max.Value)
            {
                max.Value = min.Value;
                min.TriggerUpdate();
            }

            if(max.Value < min.Value)
            {
                min.Value = max.Value;
                max.TriggerUpdate();
            }
        }

        public static void UpdateSlider(SettingsEntry entry, float factor = 0.01f)
        {
            entry.slider.label = Util.RoundToFactor(entry.slider.value, 0.01f).ToString();
            Update(entry);
        }

        public static void Update(SettingsEntry entry)
        {
            Settings.inst.Proxy.UpdateSetting(entry, OnSuccesfulSettingUpdate, OnUnsuccesfulSettingUpdate);
        }



        public static void Setup()
        {
            var config = new InteractiveConfiguration<Settings>();
            Settings.inst = config.Settings;
            Settings.inst.Config = config;

            AddListeners();

            ModSettingsBootstrapper.Register(config.ModConfig,
                (proxy, saved) =>
                {
                    OnModRegistered(proxy, saved);
                },
                (ex) =>
                {
                    OnModRegistrationFailed(ex);
                });
        }

        private static void AddListeners()
        {

            inst.s_EarthquakeChance.OnUpdatedRemotely.AddListener((s) => { UpdateSlider(s); });
            inst.s_EarthquakeVariance.OnUpdatedRemotely.AddListener((s) => { UpdateSlider(s); });

            inst.s_EarthquakeStrengthMin.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(inst.s_EarthquakeStrengthMin, inst.s_EarthquakeStrengthMax);
            });

            inst.s_EarthquakeStrengthMax.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(inst.s_EarthquakeStrengthMin, inst.s_EarthquakeStrengthMax);
            });
        }


        #region Utils

        private static string GetLabelForMode(int mode)
        {
            return Modes.ContainsKey(mode) ? Modes[mode] : "Undefined Difficulty";
        }

        #endregion

        #region Handling

        private static void OnModRegistered(ModSettingsProxy proxy, SettingsEntry[] oldSettings)
        {
            Settings.inst.Proxy = proxy;
            Settings.inst.Config.Install(proxy, oldSettings);
            Mod.helper.Log("Mod registration to ModMenu Succesful");
        }

        private static void OnModRegistrationFailed(Exception ex)
        {
            Mod.helper.Log("Mod registration to ModMenu failed");
            DebugExt.HandleException(ex);
        }

        private static void OnSuccesfulSettingUpdate()
        {
            Mod.dLog("Setting Update Successful");
        }

        private static void OnUnsuccesfulSettingUpdate(Exception ex)
        {
            Mod.Log("Setting Update Unsuccsesful");
            DebugExt.HandleException(ex);
        }

        #endregion

        public static void UpdateMode(int mode)
        {
            switch (mode)
            {
                case 0:
                    RandomEvents = true;
                    HappinessMods = true;
                    FoodPenalty = 1;
                    EarthquakeChance = 0.05f;
                    EarthquakeStrength = new MinMax(1f, 4f);
                    DroughtChance = 3;
                    VikingRaidYearRange = 2;
                    VikingRaidYearSpan = 7;
                    FireBurnoutTimeRange = new MinMax(8f, 15f);
                    FirePersistance = 1f;
                    break;
                case 1:
                    RandomEvents = true;
                    HappinessMods = true;
                    EarthquakeChance = 0.10f;
                    EarthquakeStrength = new MinMax(2f, 5f);
                    DroughtChance = 7;
                    DroughtLength = new MinMax(1,2);
                    FoodPenalty = 1.2f;
                    VikingRaidYearRange = 1;
                    VikingRaidYearSpan = 6;
                    FireBurnoutTimeRange = new MinMax(5f, 15f);
                    FirePersistance = 1f;
                    break;
                case 2:
                    RandomEvents = true;
                    HappinessMods = true;
                    EarthquakeChance = 0.125f;
                    EarthquakeStrength = new MinMax(3f, 6f);
                    DroughtChance = 12;
                    DroughtLength = new MinMax(1, 2);
                    FoodPenalty = 1.5f;
                    VikingRaidYearRange = 2;
                    VikingRaidYearSpan = 5;
                    FireBurnoutTimeRange = new MinMax(5f, 13f);
                    FirePersistance = 2f;
                    break;
                case 3:
                    RandomEvents = true;
                    HappinessMods = true;
                    EarthquakeChance = 0.14f;
                    EarthquakeStrength = new MinMax(2f, 7f);
                    DroughtChance = 14;
                    DroughtLength = new MinMax(1, 3);
                    FoodPenalty = 1.8f;
                    VikingRaidYearRange = 2;
                    VikingRaidYearSpan = 4;
                    FireBurnoutTimeRange = new MinMax(5f, 12f);
                    FirePersistance = 4f;
                    break;
                case 4:
                    RandomEvents = true;
                    HappinessMods = true;
                    EarthquakeChance = 0.2f;
                    EarthquakeStrength = new MinMax(2f, 9f);
                    DroughtChance = 16;
                    DroughtLength = new MinMax(1, 4);
                    FoodPenalty = 2.5f;
                    VikingRaidYearRange = 2;
                    VikingRaidYearSpan = 4;
                    FireBurnoutTimeRange = new MinMax(2f, 10f);
                    FirePersistance = 10f;
                    break;
            }
            ApplyGameVars();
        }

        public static void ApplyGameVars() 
        {
            Player.inst.SecondsPerEat = 150f / FoodPenalty;

            RaiderSystem.inst.MinAttackYearSpan = VikingRaidYearSpan - VikingRaidYearRange;
            RaiderSystem.inst.MaxAttackYearSpan = VikingRaidYearSpan + VikingRaidYearRange;
        }

        [HarmonyPatch(typeof(Fire), "Init")]
        public class FireInitPatch
        {
            static void Prefix(Fire __instance)
            {
                __instance.BurnoutTimeRange = Settings.FireBurnoutTimeRange;
                __instance.life = Settings.FirePersistance;
            }
        }

        [HarmonyPatch(typeof(RaiderSystem),"SetupRaid")]
        class VikingRaidMagnitudePatch
        {
            static void Prefix()
            {
                float escPoints = (float)typeof(RaiderSystem).GetField("escalationPoints", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RaiderSystem.inst);
                escPoints *= VikingEscalationModifier;
                typeof(RaiderSystem).GetField("escalationPoints", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(RaiderSystem.inst, escPoints);
            }
        }



        [HarmonyPatch(typeof(DragonSpawn), "Update")]
        class DragonAmountPatch
        {
            static void Postfix()
            {
            }
        }


    }
}
