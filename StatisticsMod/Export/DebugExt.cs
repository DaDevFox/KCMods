using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StatisticsMod
{
    class DebugExt
    {
        

        private static List<int> IDs = new List<int>();

        public static string modID = "someModId";
        public static bool Debug;

        static KCModHelper helper;

        /// <summary>
        /// Set this to recieve output messages if there were issues loading the AssetBundle. 
        /// </summary>
        public static KCModHelper Helper
        {
            get
            {
                return helper;
            }
            set
            {
                helper = value;
                hasHelper = helper != null;
            }
        }

        private static bool hasHelper = false;

        public static void Log(string message, bool repeatable = false, KingdomLog.LogStatus type = KingdomLog.LogStatus.Neutral, object GameObjectOrVector3 = null)
        {
            KingdomLog.TryLog(modID + "_debugmsg-" + IDs.Count + (repeatable ? SRand.Range(0, 1).ToString() : ""), message, type, (repeatable ? 1 : 20), GameObjectOrVector3);
            IDs.Add(1);
        }

        public static void HandleException(Exception ex)
        {
            if (Debug || helper == null)
                DebugExt.Log(ex.Message + "\n" + ex.StackTrace);
            else
                helper.Log(ex.Message + "\n" + ex.StackTrace);
        }


    }
}
