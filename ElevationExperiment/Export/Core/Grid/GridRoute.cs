using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevation
{
    class GridRoute
    {
        public string StartGridID;

        public string ClusterGridID;

        public string EndGridID;

        public int ClusterRouteStart;

        public int ClusterRouteEnd;

        public GridRoute(int start, int end, string clusterID)
        {
            this.ClusterRouteStart = start;

            this.ClusterRouteEnd = end;

            this.ClusterGridID = clusterID;
        }
    }
}
