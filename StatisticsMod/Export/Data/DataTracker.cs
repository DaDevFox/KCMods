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
        public static Dictionary<Guid, float> yearsOfVillagerCreation = new Dictionary<Guid, float>();

        [HarmonyPatch(typeof(Villager))]
        [HarmonyPatch("Init")]
        public class VillagerCreatedPatch
        {
            static void Postfix(Villager __instance)
            {
                yearsOfVillagerCreation.Add(__instance.guid, (float)Player.inst.CurrYear + Weather.inst.GetYearProgress());
            }
        }

        [HarmonyPatch(typeof(Player.PlayerSaveData))]
        [HarmonyPatch("Unpack")]
        public class VillagerUnpackPatch
        {
            static void Postfix()
            {
                foreach(Villager villager in Player.inst.Workers.data)
                    if(villager != null)
                        yearsOfVillagerCreation.Add(villager.guid, (float)Player.inst.CurrYear + Weather.inst.GetYearProgress());
            }
        }

        [HarmonyPatch(typeof(Villager))]
        [HarmonyPatch("Shutdown")]
        public class VillagerRemovedPatch
        {
            static void Postfix(Villager __instance)
            {
                yearsOfVillagerCreation.Remove(__instance.guid);
            }
        }

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
