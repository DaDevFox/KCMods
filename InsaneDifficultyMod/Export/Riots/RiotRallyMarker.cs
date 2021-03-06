﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
{
    public class RiotRallyMarker : MonoBehaviour, ISelectable
    { 
        public Vector3 pos;
        private Assets.ObjectHighlighter highlighter;
        public RiotSpawn riot;

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
            Vector3 c = riot.rallyPos;
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
            transform.localPosition = new Vector3(riot.rallyPos.x, riot.rallyPos.y + ((float)Math.Sin(Time.time) * hoverAmplitude) + hoverHeight, riot.rallyPos.z);
        }
    }
}
