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

        public string EndGridID;

        public int ClusterRouteStart;

        public int ClusterRouteEnd;

        public GridRoute(int start, int end, string StartGridID, string EndGridID)
        {
            this.ClusterRouteStart = start;

            this.ClusterRouteEnd = end;

            this.StartGridID = StartGridID;

            this.EndGridID = EndGridID;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
