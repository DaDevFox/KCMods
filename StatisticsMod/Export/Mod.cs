using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;

namespace StatisticsMod
{
    class Mod : MonoBehaviour
    {

        public static KCModHelper helper;
        public static string modID = "statisticsmod";

        void Preload(KCModHelper helper)
        {
            Mod.helper = helper;

            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        void Update()
        {
            if (Settings.debug)
            {
                if (Input.GetKeyDown(Settings.keycode_yearSummary))
                {
                    try
                    {
                        PrintYearStatsSummary();
                    }
                    catch(Exception ex)
                    {
                        DebugExt.HandleException(ex);
                    }
                }
            }

        }

        public static void OnYearEnd()
        {
            PrintYearStatsSummary();
        }
        

        public static void PrintYearStatsSummary()
        {
            string text = "";
            Data.YearData lastYear = Data.DataContainer.GetLastYearData();

            text += "Insight" + Environment.NewLine;
            text += "<color=yellow>------------</color>" + Environment.NewLine;

            text += Data.Extrapolater.GetInsightForYear(Data.DataContainer.GetLastYearData());

            text += "<color=yellow>------------</color>" + Environment.NewLine;

            text += "Stats Summary" + Environment.NewLine;
            text += "<color=yellow>------------</color>" + Environment.NewLine;

            

            text += Data.Extrapolater.exp_FoodInsufficiency(lastYear);

            text += Data.Extrapolater.exp_FoodProductionCurrent();

            text += Data.Extrapolater.exp_FoodProductionMax();

            text += "<color=yellow>------------</color>";

            DebugExt.Log(text);
        }


        [HarmonyPatch(typeof(Player), "OnNewYear")]
        public class YearPatch
        {

            static void Postfix()
            {
                try
                {
                    Data.DataContainer.OnYearEnd();
                    Mod.OnYearEnd();
                }
                catch(Exception ex)
                {
                    DebugExt.HandleException(ex);
                }
            }
        }

    }
}
