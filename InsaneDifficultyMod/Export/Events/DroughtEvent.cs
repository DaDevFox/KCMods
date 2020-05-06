using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;
using I2.Loc;

namespace InsaneDifficultyMod.Events
{
    class DroughtEvent : IDModEvent
    {
        private int timeRemaining = 0;
        public static bool droughtRunning = false;
        public static float originalWaterHeight;

        public static Dictionary<String, Assets.Code.ResourceAmount> foodBuildingYields = new Dictionary<string, Assets.Code.ResourceAmount>()
        {
            { "farm", Assets.Code.ResourceAmount.Make(FreeResourceType.Wheat,4)},
            { "orchard", Assets.Code.ResourceAmount.Make(FreeResourceType.Apples,18)},
        };


        public override bool Test()
        {
            base.Test();
            if (Settings.randomEvents)
            {
                if (SRand.Range(0, 100) < Settings.droughtChance)
                {
                    timeRemaining = Settings.droughtLength;
                    droughtRunning = true;
                    return true;
                }

                if (timeRemaining > 0)
                {
                    timeRemaining -= 1;
                    if (timeRemaining == 0)
                    {
                        onDroughtEnd();
                        droughtRunning = false;
                    }
                }
            }

            return false;
        }

        public override void Init()
        {
            base.Init();

            testFrequency = 1;

            saveObject = typeof(DroughtSaveData);
            saveID = "drought";
        }

        public override void Run()
        {
            base.Run();

            KingdomLog.TryLog("drought", "My Lord, a terrible drought has struck our land, for the next <color=yellow>" + Settings.droughtLength.ToString() + " " + (Settings.droughtLength == 1 ? "year" : "years") + "</color>, our harvest will be poor!", KingdomLog.LogStatus.Neutral);

            timeRemaining = Settings.droughtLength;
            droughtRunning = true;
            originalWaterHeight = -0.65f;

        }


        private void onDroughtEnd()
        {
            KingdomLog.TryLog("droughtend", "<color=green>The peasants rejoice! The blight upon our land has ended!</color>", KingdomLog.LogStatus.Neutral);

            timeRemaining = 0;
            droughtRunning = false;

        }

        #region Patches

        [HarmonyPatch(typeof(Weather))]
        [HarmonyPatch("Update")]
        class WeatherUpdatePatch
        {
            static void Postfix(Weather __instance)
            {
                if (droughtRunning)
                {
                    Vector3 vector = __instance.Water.transform.position;
                    vector = Vector3.Lerp(vector, new Vector3(vector.x, -0.78f, vector.z), Time.deltaTime * 0.25f);
                    __instance.Water.transform.position = vector;
                }
                else
                {
                    Vector3 vector = __instance.Water.transform.position;
                    vector = Vector3.Lerp(vector, new Vector3(vector.x, originalWaterHeight, vector.z), Time.deltaTime * 0.25f);
                    __instance.Water.transform.position = vector;
                }
            }
        }

        #region Food Building Patches

        [HarmonyPatch(typeof(Field))]
        [HarmonyPatch("Tick")]
        class FieldTickPatch
        {
            static void Postfix(Field __instance)
            {
                if (DroughtEvent.droughtRunning)
                {
                    Building b = __instance.GetComponent<Building>();
                    b.Yield = foodBuildingYields[b.UniqueName] - Settings.droughtFoodPenalty;
                }
                else
                {
                    Building b = __instance.GetComponent<Building>();
                    b.Yield = foodBuildingYields[b.UniqueName];
                }
            }
        }


        [HarmonyPatch(typeof(Orchard))]
        [HarmonyPatch("Tick")]
        class OrchardTickPatch
        {
            static void Postfix(Orchard __instance)
            {
                if (DroughtEvent.droughtRunning)
                {
                    Building b = __instance.GetComponent<Building>();
                    b.Yield = foodBuildingYields[b.UniqueName] - Settings.droughtFoodPenalty;
                }
                else
                {
                    Building b = __instance.GetComponent<Building>();
                    b.Yield = foodBuildingYields[b.UniqueName];
                }
            }
        }



        //Disable Fishing Boats during a drought
        [HarmonyPatch(typeof(FishingShip))]
        [HarmonyPatch("Tick")]
        class FishingShipTickPatch
        {
            static void Prefix(FishingShip __instance)
            {
                if (droughtRunning)
                {
                    __instance.status = FishingShip.Status.WaitAtHutFull;
                }
            }
        }

        //Change fishing hut text during drought
        [HarmonyPatch(typeof(FishingHut))]
        [HarmonyPatch("GetExplanation")]
        class FishingHutTextPatch
        {
            static void Postfix(FishingHut __instance, String __result)
            {
                if (droughtRunning)
                {
                    __result = "<color=yellow> Waters too shallow to fish: waiting until drought ends to resume </color>";
                }
            }
        }

        #endregion

        #endregion

        #region LoadSave

        public class DroughtSaveData 
        {
            public bool droughtRunning;
            public int timeRemaining;
        }


        public override void OnLoaded(object saveData)
        {
            base.OnLoaded(saveData);
            DroughtSaveData data = saveData as DroughtSaveData;

            this.timeRemaining = data.timeRemaining;
            DroughtEvent.droughtRunning = data.droughtRunning;
        }

        public override object OnSave()
        {
            DroughtSaveData data = new DroughtSaveData();
            data.timeRemaining = this.timeRemaining;
            data.droughtRunning = DroughtEvent.droughtRunning;
            return data;
        }

        #endregion
    }
}
