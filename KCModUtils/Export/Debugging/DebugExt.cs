using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCModUtils.Debugging
{
    public class DebugExt
    {
        private static List<int> IDs = new List<int>();

        public static void Log(string message, bool repeatable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            if (ModUtilsMain.debug)
            {
                KingdomLog.TryLog(ModUtilsMain.ModID + "_debugmsg-" + IDs.Count + (repeatable ? SRand.Range(0, 1).ToString() : ""), message, type, (repeatable ? 1 : 20), GameObjectOrVector3);
                IDs.Add(1);
            }
        }

        public static void HandleException(Exception ex)
        {
            if (ModUtilsMain.debug)
                DebugExt.Log(ex.Message + "\n" + ex.StackTrace);
            else
                ModUtilsMain.Helper.Log(ex.Message + "\n" + ex.StackTrace);
        }
    }
}
