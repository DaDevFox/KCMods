using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;
using I2.Loc;
using Assets.Code;

namespace InsaneDifficultyMod.Events
{
    class DroughtEvent : IDModEvent
    {
        private int timeRemaining = 0;
        public static bool droughtRunning = false;
        public static float originalWaterHeight = -0.39f;
        public static float droughtWaterHeight = -0.78f;

        public static Dictionary<String, Assets.Code.ResourceAmount> foodBuildingYields = new Dictionary<string, Assets.Code.ResourceAmount>()
        {
            { "farm", Assets.Code.ResourceAmount.Make(FreeResourceType.Wheat,4)},
            { "orchard", Assets.Code.ResourceAmount.Make(FreeResourceType.Apples,18)},
        };


        public override bool Test()
        {
            base.Test();
            if (Settings.RandomEvents)
            {
                if (SRand.Range(0, 100) < Settings.DroughtChance)
                {
                    timeRemaining = (int)Settings.DroughtLength.Rand();
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


            timeRemaining = (int)Settings.DroughtLength.Rand();
            droughtRunning = true;

            KingdomLog.TryLog("drought", "My Lord, a terrible drought has struck our land, for the next <color=yellow>" + timeRemaining.ToString() + " " + (timeRemaining == 1 ? "year" : "years") + "</color>, our harvest will be poor!", KingdomLog.LogStatus.Neutral);
        }


        private void onDroughtEnd()
        {
            KingdomLog.TryLog("droughtend", "<color=green>The peasants rejoice! The blight upon our land has ended!</color>", KingdomLog.LogStatus.Neutral);

            timeRemaining = 0;
            droughtRunning = false;

        }

        #region Patches

        // DONE Fixed fish height being misaligned with water height during drought. 
        [HarmonyPatch(typeof(Weather))]
        [HarmonyPatch("Update")]
        class WeatherUpdatePatch
        {
            static void Postfix(Weather __instance)
            {
                if (droughtRunning)
                {
                    Vector3 vector = __instance.Water.transform.position;
                    vector = Vector3.Lerp(vector, new Vector3(vector.x, droughtWaterHeight, vector.z), Time.deltaTime * 0.25f);
                    FishSystem.inst.fishHeightY = vector.y;
                    __instance.Water.transform.position = vector;
                }
                else
                {
                    Vector3 vector = __instance.Water.transform.position;
                    vector = Vector3.Lerp(vector, new Vector3(vector.x, originalWaterHeight, vector.z), Time.deltaTime * 0.25f);
                    FishSystem.inst.fishHeightY = vector.y;
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
                    if(__instance.status != FishingShip.Status.SailingToHut)
                        __instance.status = FishingShip.Status.WaitAtHutFull;
                }
            }
        }

        //DONE: Added text to fishing hut during drought to indicate fishing boat is inactive. 
        //Change fishing hut text during drought
        [HarmonyPatch(typeof(FishingHut))]
        [HarmonyPatch("GetExplanation")]
        class FishingHutTextPatch
        {
            static void Postfix(FishingHut __instance, ref String __result)
            {
                if (droughtRunning)
                {
                    __result = "<color=yellow>Waters too shallow to fish. </color>";
                    __result += Environment.NewLine;
                    __result += Environment.NewLine + ScriptLocalization.MarketTitle;
                    __result += Environment.NewLine;
                    int num = __instance.fishStack.Count();
                    int num2 = __instance.fishStack.MaxCapacity();
                    __result = String.Concat(new object[]
                    {
                        __result,
                        ResourceAmount.FriendlyName(FreeResourceType.Fish),
                        ": ",
                        num,
                        " / ",
                        num2,
                        Environment.NewLine
                    });
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
