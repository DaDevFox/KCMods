using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod.Events
{
    public class RiotRallyMarker : MonoBehaviour, ISelectable
    {
        public Vector3 basePosition;
        public Vector3 pos;
        private Assets.ObjectHighlighter highlighter;
        public Riot riot;

        public float hoverHeight = 1f;
        public float hoverAmplitude = 0.2f;


        public bool ExclusiveSelect()
        {
            return true;
        }

        public bool ValidToSelect()
        {
            return true;
        }

        public void SetHighlightImmediate(int colorIdx)
        {
            highlighter.SetOutlineImmediate(colorIdx);
        }

        public void OnSelected()
        {
            UI.riotDemandsUI.Open(riot);
        }

        public bool InsideSelectionBounds(int minx, int minz, int maxx, int maxz)
        {
            KingdomLog.TryLog("test-1-212-12","test1212113",KingdomLog.LogStatus.Neutral);
            Vector3 c = transform.position;
            return (minx < c.x && c.x < maxx) && (minz < c.z && c.z < maxz);
        }

        public RiotRallyMarker()
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            renderers.Add(GetComponent<MeshRenderer>());

            this.highlighter = Assets.ObjectHighlighter.SetupOutlines(gameObject,renderers,null);
        }

        public Vector3 GetPos()
        {
            return pos;
        }

        public int TeamID()
        {
            return 0;
        }


        public void OnRiotEnd() {
            if (this.highlighter != null)
            {
                this.highlighter.Release();
            }
        }

        void Update()
        {
            transform.localPosition = new Vector3(basePosition.x, basePosition.y + ((float)Math.Sin(Time.time) * hoverAmplitude) + hoverHeight, basePosition.z);
        }

        public bool IntersectsRay(Ray ray, out float hitDist)
        {
            hitDist = float.MaxValue;
            float radius = 0.45f;
            Vector3 vector = base.transform.position;
            if (Mathff.RaySphereIntersect(ray, vector, radius))
            {
                hitDist = Mathff.Dist(ray.origin, vector) - radius;
                return true;
            }
            return false;
        }
    }
}
