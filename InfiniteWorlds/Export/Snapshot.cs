using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Harmony;
using System.Reflection;


namespace Elevation.InfiniteWorlds
{
    public class Snapshot
    {
        public static Vector2Int dimensions = new Vector2Int(2, 2);
        public static int generations { get => dimensions.x * dimensions.y; }
        public static List<Texture2D> shots { get; internal set; } = new List<Texture2D>();


        private static void Complete()
        {
            for(int i = 0; i < shots.Count; i++) 
            {
                string path = $"{Mod.helper.modPath}/{i.ToString()}.jpg";
                Texture2D shot = shots[i];
                World.inst.SaveTexture(path, shot);

                Mod.helper.Log(path);
            }
            Mod.helper.Log("completed " + shots.Count + " snapshots");
        }

        public static Texture2D Capture(int screenWidth, int screenHeight)
        {
            Camera cam = Cam.inst.cam;
            int width = World.inst.GridWidth;
            int height = World.inst.GridHeight;


            Texture2D result;
            if (cam == null)
            {
                result = null;
                Mod.helper.Log("Cam.inst.cam unavailable");
            }
            else
            {
                bool fog = RenderSettings.fog;
                Vector3 position = cam.transform.position;
                Quaternion rotation = cam.transform.rotation;
                RenderSettings.fog = false;
                float num = (float)((height >= width) ? height : width) / 2f;
                cam.transform.position = new Vector3(-num / 2f, num, -num / 2f);
                cam.transform.LookAt(new Vector3((float)width * 0.33f, 0f, (float)height* 0.33f));
                if (cam.GetComponent<GlobalFog>() != null)
                {
                    cam.GetComponent<GlobalFog>().enabled = false;
                }
                Texture2D texture2D = World.inst.CaptureCameraShot(screenWidth, screenHeight);
                if (cam.GetComponent<GlobalFog>() != null)
                {
                    cam.GetComponent<GlobalFog>().enabled = true;
                }
                RenderSettings.fog = fog;
                cam.transform.position = position;
                cam.transform.rotation = rotation;
                result = texture2D;
            }
            return result;
        }



        [HarmonyPatch(typeof(World), "Generate")]
        private static class GeneratePatch
        {
            public static int generation = 0;

            static void Postfix(int _seed = 0)
            {
                if (generation < Snapshot.generations)
                {
                    generation++;
                    SamplingPatch.location.x += 1;
                    World.inst.Generate(_seed);
                    shots.Add(Capture(1200, 1200));

                    Mod.helper.Log("iteration");
                    return;
                }
                else
                {
                    Complete();
                }
                generation++;
            }
        }

        [HarmonyPatch(typeof(Mathf), "PerlinNoise")]
        private static class SamplingPatch
        {
            public static Vector2Int location = new Vector2Int(0,0);

            static void Prefix(ref float x, ref float y)
            {
                int xSize = (int)typeof(TerrainGen).GetField("xSize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(TerrainGen.inst);
                int ySize = (int)typeof(TerrainGen).GetField("ySize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(TerrainGen.inst);


                x += location.x * 120;
                y += location.y * ySize;
            }
        }
    }
}
