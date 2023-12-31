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
using PixelCrushers.DialogueSystem;

namespace InsaneDifficultyMod
{
    [Mod("Insane Difficulty Mod", "0.4", "Agentfox")]
    public class Settings
    {
        public static bool debug = false;
        
        
        public static Settings instance { get; private set; }
        public InteractiveConfiguration<Settings> Config { get; private set; }
        public ModSettingsProxy Proxy { get; private set; }


        public static Dictionary<int, string> Modes { get; } = new Dictionary<int, string>()
        {
            { 0, "Easy" },
            { 1, "Difficult" },
            { 2, "<color=yellow>Challenging</color>" },
            { 3, "<color=yellow>Expert</color>" },
            { 4, "<color=red>Insane</color>" }
        };


        #region Interactive

        public static int Mode {
            get
            {
                if (Settings.instance != null)
                    return (int)Settings.instance?.s_Mode.Value;
                else
                    return 0;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_Mode.Value = value;
            }
        }

        [Setting("Difficulty Preset", "A preset of settings tailored to a specific difficulty")]
        [Slider(0f, 4f, 0f, "Easy", true)]
        public InteractiveSliderSetting s_Mode { get; private set; }



        [Setting("Events/Random Events", "Enables or disables random events (turn off if also using Natural Disasters)")]
        [Toggle(true, "")]
        public InteractiveToggleSetting s_RandomEvents { get; private set; }

        public static bool RandomEvents
        {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_RandomEvents.Value;
                else
                    return true;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_RandomEvents.Value = value;
            }
        }



        [Setting("Events/Earthquakes/Formation Chance", "The chance of an earthquake happening at the beggining of a year")]
        [Slider(0.05f, 0.95f, 0.05f, "5%")]
        public InteractiveSliderSetting s_EarthquakeChance { get; private set; }

        public static float EarthquakeChance {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_EarthquakeChance.Value;
                else
                    return 0.15f;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_EarthquakeChance.Value = value;

            }
        }

        [Setting("Events/Earthquakes/Minimum Strength", "")]
        [Slider(1f, 10f, 1f, "1", true)]
        public InteractiveSliderSetting s_EarthquakeStrengthMin { get; private set; }

        [Setting("Events/Earthquakes/Maximum Strength", "")]
        [Slider(1f, 10f, 4f, "4", true)]
        public InteractiveSliderSetting s_EarthquakeStrengthMax { get; private set; }

        public static MinMax EarthquakeStrength {
            get
            {
                if (Settings.instance != null)
                    return new MinMax(Mathf.Clamp(Settings.instance.s_EarthquakeStrengthMin.Value, 0, Settings.instance.s_EarthquakeStrengthMax.Value - 1f), Settings.instance.s_EarthquakeStrengthMax.Value);
                else
                    return null;
            }
            set
            {
                if (Settings.instance != null)
                    SetMinMax(Settings.instance.s_EarthquakeStrengthMin, Settings.instance.s_EarthquakeStrengthMax, value);
            }
        }

        //[Setting("Events/Earthquakes/Randomness", "The randomness of the earthquake line")]
        //[Slider(0.05f, 0.95f, 0.8f, "0.8", false)]
        public InteractiveSliderSetting s_EarthquakeVariance { get; private set; }

        public static MinMax EarthquakeVariance
        {
            get
            {
                if (Settings.instance != null)
                    return new MinMax(0.5f - instance.s_EarthquakeVariance.Value / 2f, 0.5f + instance.s_EarthquakeVariance.Value / 2f);
                else
                    return new MinMax(0.1f, 0.9f);
            }
            set
            {
                if (Settings.instance != null)
                    instance.s_EarthquakeVariance.Value = value.Max - value.Min;
            }
        }

        public static MinMax EarthquakeLandElevation = new MinMax(0f, 0.15f);
        public static MinMax EarthquakeWaterElevation = new MinMax(-2f, -0.25f);

        [Setting("Events/Droughts/Formation Chance", "The chance of a drought occuring at the beggining of a year")]
        [Slider(0.05f, 0.95f, 0.03f, "3%")]
        public InteractiveSliderSetting s_DroughtChance { get; private set; }

        public static float DroughtChance
        {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_DroughtChance.Value;
                else
                    return 0.05f;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_DroughtChance.Value = value;
            }
        }

        [Setting("Events/Droughts/Minimum Length", "")]
        [Slider(1f, 10f, 1f, "1", true)]
        public InteractiveSliderSetting s_DroughtLengthMin { get; private set; }

        [Setting("Events/Droughts/Maximum Length", "")]
        [Slider(1f, 10f, 2f, "2", true)]
        public InteractiveSliderSetting s_DroughtLengthMax { get; private set; }

        public static MinMax DroughtLength
        {
            get
            {
                if (Settings.instance != null)
                    return new MinMax(Mathf.Clamp(Settings.instance.s_DroughtLengthMin.Value, 0, Settings.instance.s_DroughtLengthMax.Value - 1f), Settings.instance.s_DroughtLengthMax.Value);
                else
                    return null;
            }
            set
            {
                if (Settings.instance != null)
                    SetMinMax(Settings.instance.s_DroughtLengthMin, Settings.instance.s_DroughtLengthMax, value);
            }
        }

        public static bool DroughtsAffectWeather { get; private set; } = true;
        public static bool DroughtsDisableFishing { get; private set; } = true;
        public static ResourceAmount DroughtFoodPenalty { get; private set; } = new ResourceAmount();

        [Setting("Events/Droughts/Fires", "How much faster fires spread during droughts")]
        [Slider(1f, 6f, 1f, "100%")]
        public InteractiveSliderSetting s_DroughtFirePenalty { get; private set; }

        public static float DroughtFirePenalty
        {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_DroughtFirePenalty.Value;
                else
                    return 1f;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_DroughtFirePenalty.Value = value;
            }
        }

        public static bool HappinessMods { get; private set; } = true;

        [Setting("Hunger Modifier", "How much peasants try to eat")]
        [Slider(1f, 4f, 1f, "100%")]
        public InteractiveSliderSetting s_FoodPenalty { get; private set; }
        public static float FoodPenalty
        {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_FoodPenalty.Value;
                else
                    return 1f;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_FoodPenalty.Value = value;
            }
        }

        //TODO: convert to minmax
        public static int VikingRaidYearSpan { get; private set; } = 3;
        public static int VikingRaidYearRange { get; private set; } = 1;

        //DONE: Added a modifier for viking raid strength
        public static float VikingEscalationModifier { get; private set; } = 2.1f;

        public static MinMax DragonAttackYearSpan { get; private set; } = new MinMax(6, 11);
        public static MinMax DragonAmountPerAttack { get; private set; } = new MinMax(1, 20);

        public static MinMax FireBurnoutTimeRange { get; private set; } = new MinMax(8f, 15f);

        [Setting("Fire Persistence", "How difficult fires are to put out")]
        [Slider(1f, 10f, 1f, "100%")]
        public InteractiveSliderSetting s_FirePersistance { get; private set; }

        public static float FirePersistance
        {
            get
            {
                if (Settings.instance != null)
                    return Settings.instance.s_FirePersistance.Value;
                else
                    return 1f;
            }
            set
            {
                if (Settings.instance != null)
                    Settings.instance.s_FirePersistance.Value = value;
            }
        }

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
            DroughtFoodPenalty.Set(FreeResourceType.Wheat, 4);
            DroughtFoodPenalty.Set(FreeResourceType.Apples, 4);
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
            Settings.instance.Proxy.UpdateSetting(entry, OnSuccesfulSettingUpdate, OnUnsuccesfulSettingUpdate);
        }



        public static void Setup()
        {
            var config = new InteractiveConfiguration<Settings>();
            Settings.instance = config.Settings;
            Settings.instance.Config = config;


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
            instance.s_Mode.OnUpdatedRemotely.AddListener((s) =>
            {
                int mode = Mathf.CeilToInt(s.slider.value);
                s.slider.label = Modes[mode];
                Update(s);

                UpdateMode(mode);
            });


            instance.s_EarthquakeChance.OnUpdatedRemotely.AddListener(s => SetPercentageSlider(s, s.slider.value * 100f));

            instance.s_EarthquakeStrengthMin.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(instance.s_EarthquakeStrengthMin, instance.s_EarthquakeStrengthMax);
            });

            instance.s_EarthquakeStrengthMax.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(instance.s_EarthquakeStrengthMin, instance.s_EarthquakeStrengthMax);
            });

            instance.s_DroughtChance.OnUpdatedRemotely.AddListener(s => SetPercentageSlider(s, s.slider.value * 100f));

            instance.s_DroughtLengthMin.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(instance.s_DroughtLengthMin, instance.s_DroughtLengthMax);
            });

            instance.s_DroughtLengthMax.OnUpdatedRemotely.AddListener((s) =>
            {
                UpdateSlider(s);
                OnMinMaxUpdate(instance.s_DroughtLengthMin, instance.s_DroughtLengthMax);
            });

            instance.s_DroughtFirePenalty.OnUpdatedRemotely.AddListener((s) => 
            {
                SetPercentageSlider(s, s.slider.value * 100f, 10f);
            });


            instance.s_FirePersistance.OnUpdatedRemotely.AddListener(entry => SetPercentageSlider(entry, entry.slider.value * 100f, 10f));
            instance.s_FoodPenalty.OnUpdatedRemotely.AddListener(entry => SetPercentageSlider(entry, entry.slider.value * 100f, 10f));
        }


        #region Utils

        /// <summary>
        /// Sets a slider to label it with the percentage value <c>percent</c> (1xx not 0.xx)
        /// </summary>
        /// <param name="slider"></param>
        /// <param name="percent"></param>
        /// <param name="factor">rounds the value to that factor</param>
        public static void SetPercentageSlider(SettingsEntry slider, float percent, float factor = 0.1f)
        {
            slider.slider.label = $"{Util.RoundToFactor(percent, factor)}%";
            Update(slider);
        }

        private static string GetLabelForMode(int mode)
        {
            return Modes.ContainsKey(mode) ? Modes[mode] : "Undefined Difficulty";
        }

        #endregion

        #region Handling

        private static void OnModRegistered(ModSettingsProxy proxy, SettingsEntry[] oldSettings)
        {
            Settings.instance.Proxy = proxy;
            Settings.instance.Config.Install(proxy, oldSettings);
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
                    DroughtChance = 0.03f;
                    DroughtLength = new MinMax(1, 2);
                    DroughtFirePenalty = 1f;
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
                    DroughtChance = 0.07f;
                    DroughtLength = new MinMax(1, 2);
                    DroughtFirePenalty = 1.5f;
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
                    EarthquakeStrength = new MinMax(2f, 6f);
                    DroughtChance = 0.12f;
                    DroughtLength = new MinMax(1, 3);
                    DroughtFirePenalty = 2f;
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
                    EarthquakeStrength = new MinMax(3f, 7f);
                    DroughtChance = 0.14f;
                    DroughtLength = new MinMax(2, 3);
                    DroughtFirePenalty = 3f;
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
                    EarthquakeStrength = new MinMax(3f, 9f);
                    DroughtChance = 0.16f;
                    DroughtLength = new MinMax(2, 5);
                    DroughtFirePenalty = 6f;
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
