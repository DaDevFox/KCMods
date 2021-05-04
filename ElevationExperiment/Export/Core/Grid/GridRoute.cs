using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevation
{
    public class GridRoute : IEnumerable
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

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
