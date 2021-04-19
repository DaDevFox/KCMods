using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elevation
{ 
    public class Dugout : MonoBehaviour
    {
        private Cell _cell;
        private Building _building;


        private void Start()
        {
            _cell = World.inst.GetCellDataClamped(this.transform.position);
            _building = GetComponent<Building>();
        }

        public void OnBuilt()
        {
            EffectsMan.inst.RemoveStonePoof.CreateAndPlay(base.transform.position);
            SfxSystem.inst.PlayFromBank("buildingcollapse", this._building.Center(), null);
            ElevationManager.TryProcessElevationChange(_cell, -1);
        }

        public void OnValidateJobs(Building building)
        {
            if (building.WorkersForFullYield == 0)
            {
                for (int i = 0; i < this._building.jobs.Count; i++)
                {
                    JobSystem.inst.RemoveJob(this._building.jobs[i], true);
                }
                this._building.jobs.Clear();
            }
        }



    }
}
