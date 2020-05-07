using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;

namespace ElevationExperiment
{
    class DebugExt : MonoBehaviour
    {

        private static List<int> IDs = new List<int>();
        private static List<LineRenderer> drawnLines = new List<LineRenderer>();
        private static List<float> drawnLineDurations = new List<float>();

        public static void dLog(string message, bool repeatable = false, object GameObjectOrVector3 = null)
        {
            if (Settings.debug)
            {
                KingdomLog.TryLog(Mod.modID + "_debugmsg-" + IDs.Count + (repeatable ? SRand.Range(0, 1).ToString() : ""), message, KingdomLog.LogStatus.Neutral, (repeatable ? 1 : 20), GameObjectOrVector3);
                IDs.Add(1);
            }
        }

        public static void Log(string message, bool repeatable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            KingdomLog.TryLog(Mod.modID + "_debugmsg-" + IDs.Count + (repeatable ? SRand.Range(0, 1).ToString() : ""), message, type, (repeatable ? 1 : 20), GameObjectOrVector3);
            IDs.Add(1);
        }

        public static void HandleException(Exception ex)
        {
            try
            {
                Log(ex.Message + "\n" + ex.StackTrace);
            }
            catch
            {
                Mod.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static void DrawLine(Vector3 from, Vector3 to, float duration = 10)
        {
            LineRenderer lineRenderer = new LineRenderer();
            lineRenderer.transform.SetParent(World.inst.transform);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);

            lineRenderer.endColor = Color.white;
            lineRenderer.startColor = Color.white;

            drawnLines.Add(lineRenderer);
            drawnLineDurations.Add(duration);
        }


        void Update()
        {
            List<int> idsToDestroy = new List<int>();
            for(int i = 0; i < drawnLines.Count; i++)
            {
                
               drawnLineDurations[i] -= Time.deltaTime;
                if (drawnLineDurations[i] < 0)
                    idsToDestroy.Add(i);
            }

            foreach(int idx in idsToDestroy)
            {
                GameObject.Destroy(drawnLines[idx]);
                drawnLines.RemoveAt(idx);
                drawnLineDurations.RemoveAt(idx);
            }
        }

    }
}
