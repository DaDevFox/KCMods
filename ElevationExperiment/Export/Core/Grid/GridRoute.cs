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

        public int X;

        public int Z;

        public 

        public GridRoute(int x, int z, string StartGridID, string EndGridID)
        {
            this.X = x;

            this.Z = z;

            this.StartGridID = StartGridID;

            this.EndGridID = EndGridID;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
