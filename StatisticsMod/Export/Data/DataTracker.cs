using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace StatisticsMod.Data
{
    class DataTracker
    {



        [HarmonyPatch(typeof(Villager))]
        [HarmonyPatch("TryEat")]
        public class VillagerMissedMealTracker
        {
            static void Postfix(Villager __instance, bool __result) 
            {
                DataContainer.currentYearData.foodConsumptionData.timesEaten++;
                if (__result)
                {
                    DataContainer.currentYearData.foodConsumptionData.timesSatisfied++;
                }
            }
        }


    }
}
