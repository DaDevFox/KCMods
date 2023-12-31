using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;

namespace TimelapseRecorderMod
{
    public class CameraTrack
    {
        public List<int> Keys { get; set; }
        public Dictionary<string, AnimationCurve> Curves { get; private set; }
        

        public CameraTrack(Dictionary<string,AnimationCurve> curves)
        {
            Curves = curves;
        }

        public float Get(string property, float key)
        {
            if (Curves.ContainsKey(property))
                return Curves[property].Evaluate(key);
            else
                return -1f;
        }
    }



    public class Mod : MonoBehaviour
    {
        #region Mod Vars

        //Mod
        public static KCModHelper helper;
        public static string modID = "crimemod";

        //AssetBundle
        public static string assetBundleName = "crimemodassets";
        public static string assetBundlePath = "/assetbundle/";

        #endregion

        private static TimelapseGif gifMaker;

        #region Timelapse Settings

        public static bool Recording 
        {
            get
            {
                return recording;
            }
            set
            {
                if (recording && value == false)
                    SaveFrames();

                if (!recording && value == true)
                    BeginTimelapse();

                recording = value;
            }
        }
        private static bool recording = false;

        public static float frameRate = 2f;
        public static float maxLengthSeconds = 5f;

        public static int width = 400;
        public static int height = 300;

        public static CameraTrack Track { get; set; }

        #endregion

        private static Camera cam;

        private static Vector3 position;
        private static Quaternion rotation;

        private static float timeElapsed = 0f;
        private static readonly float interval = 1f / frameRate;

        private static int currentFrame = 0;


        public static Dictionary<int, string> CurrentSessionFramePaths { get; private set; } = new Dictionary<int, string>();
        public static Dictionary<int, ImageData> CurrentSessionFrames { get; private set; } = new Dictionary<int, ImageData>();


        void Preload(KCModHelper helper)
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Mod.helper = helper;
        }


        void SceneLoaded()
        {
            Mod.helper.Log("Scene Loaded");

            gifMaker = new TimelapseGif();
        }

        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Recording = !Recording;
            }

            if (!Recording)
                return;

            currentFrame = Mathf.FloorToInt(timeElapsed / interval);

            string relativePath = "/Recorded/" + TownNameUI.inst.townName;

            

            if (!CurrentSessionFrames.ContainsKey(currentFrame))
            {
                CurrentSessionFrames.Add(currentFrame, GetFrame());
                CurrentSessionFramePaths.Add(currentFrame, helper.modPath + relativePath + "/" + currentFrame.ToString());
            }

            
            timeElapsed += Time.deltaTime;
        }

        private static void BeginTimelapse()
        {
            gifMaker.gifPath = "/Timelapses/" + TownNameUI.inst.townName + ".gif";

            if (cam != null)
            {
                GameObject obj = new GameObject();
                GameObject.Instantiate(obj, World.inst.transform);
                cam = obj.AddComponent<Camera>();
            }
        }


        private static void SaveFrames()
        {
            gifMaker.Create();
            foreach(int frame in CurrentSessionFrames.Keys)
            {
                gifMaker.AddFrame(CurrentSessionFrames[frame]);
            }
            gifMaker.CloseGif();

            Mod.helper.Log("Gif created");
        }

        private ImageData GetFrame()
        {
            ImageData data = new ImageData();
            Texture2D capture = CaptureFrame();

            data.width = capture.width;
            data.height = capture.height;

            data.pixelData = new byte[3 * capture.width * capture.height];

            int i = 0;

            for (int y = data.height - 1; y >= 0; y--)
            {
                for (int x = 0; x < data.width; x++)
                {
                    Color pixel = capture.GetPixel(x, y);
                    data.pixelData[i] = (byte)(pixel.r * 255f);
                    i++;
                    data.pixelData[i] = (byte)(pixel.g * 255f);
                    i++;
                    data.pixelData[i] = (byte)(pixel.b * 255f);
                    i++;
                }
            }

            return data;
        }

        private static Texture2D CaptureFrame()
        {
            return World.inst.CaptureWorldShot(width,height);
        }
    }
}
