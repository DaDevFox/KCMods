using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using Elevation;
using System.Diagnostics;

namespace Elevation
{
    public abstract class ElevationPathfinder
    {
        public static ElevationPathfinder current { get; set; } = new ExternalPathfinder();

        public abstract void Init(int width, int height);

        public abstract void Path(
            Vector3 startPos,
            bool upperGridStart,
            Vector3 endPos,
            bool upperGridEnd,

            ref List<Vector3> path,

            Pathfinder.blocksPathTest blocksPath,
            Pathfinder.blocksPathTest pull,
            Pathfinder.applyExtraCost extraCost,

            int team,

            bool doDiagonal,
            bool doTrimming,
            bool allowIntergridTravel);
    }

    // Base pathfinding inits before any terrain is generated; this is not possible with Elevation due to the fact that the shape of the terrain affects how the pathfinding needs to be setup
    

}
