using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using Zat.Shared.InterModComm;
using Zat.Shared.ModMenu.API;
using Zat.Shared.ModMenu.Interactive;
using Newtonsoft.Json;


namespace ElevationExperiment
{

    public enum ElevationBiasType
    {
        Rounded,
        Min,
        Max
    }

    

    [Mod("Elevation", "v0.1", "Agentfox")]
    public class Settings
    {
        public InteractiveConfiguration<Settings> Config { get; private set; }
        public ModSettingsProxy Proxy { get; private set; }
        public static Settings inst { get; private set; }

        #region Interactive

        [Category("Terrain Generation")]
        public Generator c_Generator { get; private set; }

        [Category("Coloring")]
        public Coloring c_Coloring { get; private set; }

        [Category("Camera Controls")]
        public CameraControls c_CameraControls { get; private set; }

        //[Category("Visual")]
        public Visual c_Visual { get; private set; }

        public class Visual
        {

            [Setting("Pathfinding Indicator Enabled", "Wether or not the pathfinding indicator shown while building is enabled")]
            [Toggle(false,"")]
            public InteractiveToggleSetting s_VisualPathfindingIndicatorEnabled { get; private set; }

        }

        public class Generator
        {
            [Setting("Regenerate Terrain", "Regenerates elevated terrain")]
            [Button("Regenerate")]
            public InteractiveButtonSetting s_Regenerate { get; private set; }

            [Category("Advanced")]
            public Advanced c_Advanced { get; private set; }

            public class Advanced
            {
                [Setting("Bias")]
                [Select(1,"Rounded", "Min", "Max")]
                public InteractiveSelectSetting s_ElevationBiasType { get; private set; }
                public ElevationBiasType ElevationBiasType {
                    get
                    {
                        return (ElevationBiasType)s_ElevationBiasType.Value;
                    }
                    set
                    {
                        s_ElevationBiasType.Value = (int)value;
                    }
                }

                [Category("Noise")]
                public Noise c_Noise { get; private set; }

                public class Noise
                {
                    //[Setting("Scale", "The scale of the noise used to generate elevation")]
                    //[Slider(0.1f, 100f, 50f, "50", false)]
                    //public InteractiveSliderSetting s_Scale { get; private set; }
                    //public float Scale
                    //{
                    //    get
                    //    {

                    //        return s_Scale.Value;
                    //    }
                    //    set
                    //    {
                    //        s_Scale.Value = value;
                    //        MapGenerator.Scale = value;
                    //    }
                    //}

                    //[Setting("Amplitude", "The amplitude multiplier of the y-values")]
                    //[Slider(0.1f, 2f, 0.7f, "0.7", false)]
                    //public InteractiveSliderSetting s_Amplitue { get; private set; }
                    //public float Amplitude
                    //{
                    //    get
                    //    {
                    //        return s_Amplitue.Value;
                            
                    //    }
                    //    set
                    //    {
                    //        s_Amplitue.Value = value;
                    //        MapGenerator.Amplitude = value;
                    //    }
                    //}

                    //[Setting("Smoothing", "Whether or not to smooth elevated terrain after generation; disabling can provide small performance boosts. ")]
                    //[Toggle(true,"")]
                    //public InteractiveToggleSetting s_Smoothing { get; private set; }
                    //public bool Smoothing 
                    //{
                    //    get
                    //    {
                    //        return s_Smoothing.Value;
                    //    }
                    //    set
                    //    {
                    //        s_Smoothing.Value = value;
                    //        MapGenerator.doSmoothing = value;
                    //    }
                    //}


                }
            }
        }

        public class Coloring
        {

            //[Setting("Copy Colors", "Copies the current color configuration to the clipboard in text form so you can share it with other people!")]
            //[Button("Copy")]
            //public InteractiveButtonSetting s_copyColorsToClipboard { get; private set; }

            //[Setting("Paste Colors", "Updates your color configuration to match the one copied. ")]
            //[Button("Paste")]
            //public InteractiveButtonSetting s_PasteColorsFromClipboard { get; private set; }


            [Setting("Color Preset", "A set of predetremined colors that can be used for elevation coloring")]
            [Select(0,"Default", "Sandy", "Mesa", "Experimental")]
            public InteractiveSelectSetting s_preset { get; private set; }
            public Dictionary<int, UnityEngine.Color> preset
            {
                get
                {
                    foreach(string opt in s_preset.Options)
                        Mod.dLog(opt);
                    return elevationColorPresets.ContainsKey(s_preset.Options[s_preset.Value]) ?
                        elevationColorPresets[s_preset.Options[s_preset.Value]] :
                        null;
                }
            }


            [Category("Tiers")]
            public Tiers c_Tiers { get; private set; }

            public class Tiers
            {
                [Setting("1","")]
                [Color(0.662f, 0.854f, 0.564f)]
                public InteractiveColorSetting t_1 { get; private set; }
                public UnityEngine.Color color1
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_1.Color);
                    }
                    set
                    {
                        t_1.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("2","")]
                [Color(0.709f, 0.807f, 0.533f)]
                public InteractiveColorSetting t_2 { get; private set; }
                public UnityEngine.Color color2
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_2.Color);
                    }
                    set
                    {
                        t_2.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("3", "")]
                [Color(0.803f, 0.764f, 0.596f)]
                public InteractiveColorSetting t_3 { get; private set; }
                public UnityEngine.Color color3
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_3.Color);
                    }
                    set
                    {
                        t_3.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("4", "")]
                [Color(0.819f, 0.811f, 0.780f)]
                public InteractiveColorSetting t_4 { get; private set; }
                public UnityEngine.Color color4
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_4.Color);
                    }
                    set
                    {
                        t_4.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("5", "")]
                [Color(0.647f, 0.639f, 0.611f)]
                public InteractiveColorSetting t_5 { get; private set; }
                public UnityEngine.Color color5
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_4.Color);
                    }
                    set
                    {
                        t_5.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("6", "")]
                [Color(0.549f, 0.549f, 0.549f)]
                public InteractiveColorSetting t_6 { get; private set; }
                public UnityEngine.Color color6
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_6.Color);
                    }
                    set
                    {
                        t_6.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("7", "")]
                [Color(0.690f, 0.690f, 0.690f)]
                public InteractiveColorSetting t_7 { get; private set; }
                public UnityEngine.Color color7
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_7.Color);
                    }
                    set
                    {
                        t_7.Color = Settings.UnityColorToZat(value);
                    }
                }

                [Setting("8", "")]
                [Color(0.866f, 0.886f, 0.854f)]
                public InteractiveColorSetting t_8 { get; private set; }
                public UnityEngine.Color color8
                {
                    get
                    {
                        return Settings.ZatColorToUnity(t_8.Color);
                    }
                    set
                    {
                        t_8.Color = Settings.UnityColorToZat(value);
                    }
                }
            }
            
        }

        public class CameraControls
        {
            [Setting("Activation Key", "The key pressed to activate the top-down view camera. ")]
            [Hotkey(KeyCode.T,false,false,false)]
            public InteractiveHotkeySetting s_activateKey { get; private set; }

            [Setting("Snap", "The snap multiple for the top-down view camera; setting to 0 eliminates all snap. ")]
            [Slider(0f, 5f, 0f, "0", true)]
            public InteractiveSliderSetting s_snap { get; private set; }
            [Setting("Speed", "The maximum speed of the top-down view camera. ")]
            [Slider(0.1f, 2f, 0.5f, "0.5", false)]
            public InteractiveSliderSetting s_speed { get; private set; }
            [Setting("Speed Boost", "The speed boost the camera gains when the camera speed (default of SHIFT) key is pressed. ")]
            [Slider(1f,4f,2f,"2",false)]
            public InteractiveSliderSetting s_shiftSpeed { get; private set; }



        }

        #endregion

        #region Debug

        public static bool debug = true;

        public static KeyCode keycode_raise = KeyCode.R;
        public static KeyCode keycode_lower = KeyCode.F;

        
        public static KeyCode keycode_sampleCell = KeyCode.G;

        #endregion

        internal static string Clipboard
        {//https://flystone.tistory.com/138
            get
            {
                TextEditor _textEditor = new TextEditor();
                _textEditor.Paste();
                return _textEditor.text;
            }
            set
            {
                TextEditor _textEditor = new TextEditor
                { text = value };

                _textEditor.OnFocus();
                _textEditor.Copy();
            }
        }




        public static KeyCode keycode_topDownView = KeyCode.T;
        public static ElevationBiasType elevationBiasType = ElevationBiasType.Min;

        public static Dictionary<string, Dictionary<int,UnityEngine.Color>> elevationColorPresets = new Dictionary<string, Dictionary<int, UnityEngine.Color>>() 
        {
            { "Default", new Dictionary<int,UnityEngine.Color>()
                {
                    { 1, new UnityEngine.Color(0.662f, 0.854f, 0.564f) },
                    { 2, new UnityEngine.Color(0.709f, 0.807f, 0.533f) },
                    { 3, new UnityEngine.Color(0.803f, 0.764f, 0.596f) },
                    { 4, new UnityEngine.Color(0.819f, 0.811f, 0.780f) },
                    { 5, new UnityEngine.Color(0.647f, 0.639f, 0.611f) },
                    { 6, new UnityEngine.Color(0.549f, 0.549f, 0.549f) },
                    { 7, new UnityEngine.Color(0.690f, 0.690f, 0.690f) },
                    { 8, new UnityEngine.Color(0.866f, 0.886f, 0.854f) }
                }
            },
            { "Sandy", new Dictionary<int, UnityEngine.Color>()
                {
                    { 1, new UnityEngine.Color(0.890f, 0.850f, 0.670f) },
                    { 2, new UnityEngine.Color(0.843f, 0.741f, 0.258f) },
                    { 3, new UnityEngine.Color(0.592f, 0.537f, 0.266f) },
                    { 4, new UnityEngine.Color(0.623f, 0.388f, 0.137f) },
                    { 5, new UnityEngine.Color(0.949f, 0.513f, 0.050f) },
                    { 6, new UnityEngine.Color(0.690f, 0.533f, 0.368f) },
                    { 7, new UnityEngine.Color(0.419f, 0.419f, 0.419f) },
                    { 8, new UnityEngine.Color(0.658f, 0.658f, 0.658f) },

                }
            },
            { "Mesa", new Dictionary<int, UnityEngine.Color>()
                {
                    {1, new UnityEngine.Color(0.847f, 0.635f, 0.431f) },
                    {2, new UnityEngine.Color(0.819f, 0.525f, 0.239f) },
                    {3, new UnityEngine.Color(0.682f, 0.376f, 0.254f) },
                    {4, new UnityEngine.Color(0.631f, 0.443f, 0.368f) },
                    {5, new UnityEngine.Color(0.588f, 0.588f, 0.588f) },
                    {6, new UnityEngine.Color(0.439f, 0.439f, 0.439f) },
                    {7, new UnityEngine.Color(0.619f, 0.619f, 0.619f) },
                    {8, new UnityEngine.Color(0.749f, 0.749f, 0.682f) }
                } 
            },
            { "Experimental", new Dictionary<int, UnityEngine.Color>()
                {
                    {1, new UnityEngine.Color(0.486f, 0.552f, 0.298f) },
                    {2, new UnityEngine.Color(0.709f, 0.729f, 0.380f) },
                    {3, new UnityEngine.Color(0.447f, 0.329f, 0.156f) },
                    {4, new UnityEngine.Color(0.501f, 0.501f, 0.501f) },
                    {5, new UnityEngine.Color(0.360f, 0.360f, 0.360f) },
                    {6, new UnityEngine.Color(0.250f, 0.250f, 0.250f) },
                    {7, new UnityEngine.Color(0.509f, 0.509f, 0.509f) },
                    {8, new UnityEngine.Color(0.803f, 0.796f, 0.796f) }
                }
            }
        };

        public static float topDownViewCamSnap = 1f;


        #region Base

        public static void Setup()
        {
            var config = new InteractiveConfiguration<Settings>();
            Settings.inst = config.Settings;
            Settings.inst.Config = config;

            AddListeners();

            ModSettingsBootstrapper.Register(config.ModConfig,
                (proxy, oldSettings) =>
                {
                    OnModRegistered(proxy, oldSettings);
                },
                (ex) => {
                    OnModRegistrationFailed(ex);
                });
        }

        private static void AddListeners()
        {
            // Generator
            Settings.inst.c_Generator.s_Regenerate.OnButtonPressed.AddListener(OnRegenerateButtonClicked);

            //Settings.inst.c_Generator.c_Advanced.c_Noise.s_Amplitue.OnUpdate.AddListener(UpdateSlider);
            //Settings.inst.c_Generator.c_Advanced.c_Noise.s_Scale.OnUpdate.AddListener(UpdateSlider);

            // Coloring
            //Settings.inst.c_Coloring.s_copyColorsToClipboard.OnButtonPressed.AddListener(CopyColorsToClipboard);
            //Settings.inst.c_Coloring.s_PasteColorsFromClipboard.OnButtonPressed.AddListener(PasteColorsFromClipboard);


            Settings.inst.c_Coloring.s_preset.OnUpdate.AddListener(OnColorPresetChanged);

            Settings.inst.c_Coloring.c_Tiers.t_1.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_2.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_3.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_4.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_5.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_6.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_7.OnUpdate.AddListener(OnColorChanged);
            Settings.inst.c_Coloring.c_Tiers.t_8.OnUpdate.AddListener(OnColorChanged);

            // Camera Controls
            Settings.inst.c_CameraControls.s_shiftSpeed.OnUpdate.AddListener(UpdateSlider);
            Settings.inst.c_CameraControls.s_snap.OnUpdate.AddListener(UpdateSlider);
            Settings.inst.c_CameraControls.s_speed.OnUpdate.AddListener(UpdateSlider);
        }

        #endregion

        #region Listener Callbacks

        private static void OnRegenerateButtonClicked()
        {
            ElevationManager.SetupCellMarks();
            MapGenerator.Generate();
        }

        private static void OnColorPresetChanged(SettingsEntry entry)
        {
            Dictionary<int, UnityEngine.Color> preset = inst.c_Coloring.preset;
            foreach(int tier in preset.Keys)
            {
                Settings.inst.c_Coloring.c_Tiers.GetType().GetProperty("color" + tier.ToString()).SetValue(Settings.inst.c_Coloring.c_Tiers, preset[tier]);
                ColorManager.SetColor(tier, preset[tier]);
            }

            ColorManager.Update();
        }

        private static void OnColorChanged(SettingsEntry entry)
        {
            int tier = int.Parse(entry.GetName());
            ColorManager.SetColor(tier, ZatColorToUnity(entry.color));

            ColorManager.Update();
        }

        public static void Update(SettingsEntry entry)
        {
            Settings.inst.Proxy.UpdateSetting(entry, OnSuccesfulSettingUpdate, OnUnsuccesfulSettingUpdate);
        }


        private static void UpdateSlider(SettingsEntry entry)
        {
            entry.slider.label = Utils.Util.RoundToFactor(entry.slider.value, 0.01f).ToString();
            Update(entry);
        }

        private static void CopyColorsToClipboard()
        {
            string json = "{";
            json += JsonConvert.SerializeObject(Settings.inst.c_Coloring.c_Tiers.color1);
            json += "}";

            Clipboard = "sample text";
        }


        private static void PasteColorsFromClipboard()
        {
            string json = Clipboard;

            DebugExt.dLog(json);
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

        #region Utils

        private static UnityEngine.Color ZatColorToUnity(Zat.Shared.ModMenu.API.Color color)
        {
            return new UnityEngine.Color() 
            {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a
            };
        }

        private static Zat.Shared.ModMenu.API.Color UnityColorToZat(UnityEngine.Color color)
        {
            return new Zat.Shared.ModMenu.API.Color()
            {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a
            };
        }


        #endregion

    }
}
