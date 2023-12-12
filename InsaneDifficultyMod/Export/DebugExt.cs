using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
{
    class DebugExt
    {
        private static int currID = 0;

        public static void dLog(string message, bool repeateable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            if(Settings.debug)
                Log(message, repeateable, type, GameObjectOrVector3);
        }

        public static void Log(string message, bool repeatable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            KingdomLog.TryLog(Mod.modID + "_debugmsg-" 
                //+ message.GetHashCode() 
                + (repeatable ? Util.Randi().ToString() : ""), message, type, (repeatable ? 1 : 20), GameObjectOrVector3);
        }
        
        public static void HandleException(Exception ex)
        {
            Mod.helper.Log(ex.ToString());
        }

    }
}
