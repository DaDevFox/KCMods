using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevationExperiment
{
    public class TerrainFeature
    {

        public Cell origin;
        public List<Cell> affected;
        

        public virtual TerrainFeature Create(Cell origin)
        {
            this.origin = origin;
            return new TerrainFeature();
        }

        public virtual bool TestPlacement(Cell considering)
        {
            return false;
        }

        public virtual int Get(Cell cell) 
        {
            return 0;
        }
    }
}
