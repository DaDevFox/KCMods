using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReskinEngine.API
{
    public class WindmillSkin : GenericBuildingSkin
    {
        internal override string UniqueName => "windmill";

        public GameObject blades;

        protected override void PackageInternal(Transform dropoff, GameObject _base)
        {
            base.PackageInternal(dropoff, _base);

            AppendModel(_base, blades, "blades");
        }
    }
}
