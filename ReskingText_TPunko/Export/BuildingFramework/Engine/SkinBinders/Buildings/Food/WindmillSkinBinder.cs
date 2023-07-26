using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReskinEngine.Engine
{
    // UNTESTED
    public class WindmillSkinBinder : GenericBuildingSkinBinder
    {
        public override string UniqueName => "windmill";

        public GameObject blades;

        public override void Read(GameObject obj)
        {
            base.Read(obj);

            ReadModel(obj, "blades");
        }

        public override void BindToBuildingBase(Building building)
        {
            base.BindToBuildingBase(building);

            if (blades)
            {
                Windmill mill = building.GetComponent<Windmill>();
                mill.blades.GetChild(0).gameObject.SetActive(false);
                mill.blades.GetChild(1).gameObject.SetActive(false);
                mill.blades.GetChild(2).gameObject.SetActive(false);
                mill.blades.GetChild(3).gameObject.SetActive(false);

                GameObject.Instantiate(blades, mill.blades);
            }
        }
    }
}
