using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KCModUtils.Debugging;
{
    class DebugExt
    {

        private static List<int> IDs = new List<int>();

        public static void Log(string message, bool repeatable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            KingdomLog.TryLog(Mod.modID + "_debugmsg-" + IDs.Count + (repeatable ? Util.Randi().ToString() : ""), message, type, (repeatable ? 1 : 20), GameObjectOrVector3);
            IDs.Add(1);
        }


    }
}
