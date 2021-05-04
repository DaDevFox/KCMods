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
        public string id;

        public string StartGridID;

        public string EndGridID;

        public int X;

        public int Z;

        public float Value;

        public GridRoute(int x, int z, string StartGridID, string EndGridID)
        {
            this.X = x;

            this.Z = z;

            this.StartGridID = StartGridID;

            this.EndGridID = EndGridID;

            this.id = this.StartGridID + ":" + this.X + "_" + this.Z;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
