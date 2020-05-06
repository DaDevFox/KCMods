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


        public static void OnYearEnd()
        {
            PrintYearStatsSummary();
        }
        

        public static void PrintYearStatsSummary()
        {
            string text = "";

            text += "Insight" + Environment.NewLine;
            text += "<color=yellow>-------</color>" + Environment.NewLine;

            text += Data.Extrapolater.GetInsightForYear(Data.DataContainer.GetLastYearData());

            text += "<color=yellow>-------</color>" + Environment.NewLine;

            text += "Stats Summary" + Environment.NewLine;
            text += "<color=yellow>------------</color>" + Environment.NewLine;

            text += "Food insufficiency last year: " + 
                Data.Analyzer.GetRequiredFoodForYear(Data.DataContainer.GetLastYearData()).ToString() +
                Environment.NewLine;
            
            text += "Estimated Food insufficiency for " +
                Data.Analytics.GetPlayerKingdomPopulation().ToString() +
                " people: " +
                Data.Analyzer.GetEstimatedFoodInsufficiencyForPeople(Data.Analytics.GetPlayerKingdomPopulation(),Data.DataContainer.GetAllYearData()).ToString() + 
                Environment.NewLine;

            text += "Estimated Food insufficiency for " +
                Data.Analytics.GetHousingForKingdom().ToString() +
                " people: " +
                Data.Analyzer.GetEstimatedFoodInsufficiencyForPeople(Data.Analytics.GetHousingForKingdom(), Data.DataContainer.GetAllYearData()).ToString() +
                Environment.NewLine;

            text += "<color=yellow>------------</color>";

            DebugExt.Log(text);
        }


        [HarmonyPatch(typeof(Player), "OnNewYear")]
        public class YearPatch
        {

            static void Postfix()
            {
                Data.DataContainer.OnYearEnd();
                Mod.OnYearEnd();
            }
        }

    }
}
