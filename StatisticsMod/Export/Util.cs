using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StatisticsMod
{ 

    static class Util
    {
        public static float RoundToFactor(float a, float factor)
        {
            return Mathf.Round(a / factor) * factor;
        }

    }
}
