using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using Assets.Code;
using Assets.Code.Jobs;
using System.Reflection;
using System.CodeDom;

namespace Fox.ForestFires
{
    public class Mod : MonoBehaviour
    {
        public static KCModHelper helper;

        private void Preload(KCModHelper helper)
        {
            Mod.helper = helper;
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(FireManager), "StartFireAt")]
    class FireCreatePatch
    {
        static void Postfix(ref Fire __result, Cell cell, float _forestChance = -1f, float _meadowChance = -1f, float _castleChance = -1f, float _buildingChance = -1f)
        {
            if (cell == null)
            {
                __result = null;
            }
            else
            {
                Building x = cell.FindHighestNotFireImmune();
                if (x == null && cell.TreeAmount == 0)
                {
                    __result = null;
                }
                else if (cell.Type == ResourceType.Water && x == null)
                {
                    __result = null;
                }
                else if (cell.Type == ResourceType.IronDeposit || cell.Type == ResourceType.Stone || cell.Type == ResourceType.UnusableStone)
                {
                    __result = null;
                }
                else
                {
                    if (!(cell.busyObj is Fire))
                    {
                        if (cell.busyObj != null && cell.busyObj is Job)
                        {
                            Job job = cell.busyObj as Job;
                            if (job.Employee != null)
                            {
                                Player.inst.DestroyPerson(job.Employee, true);
                            }
                            if (job is ClearCutterJob)
                            {
                                JobSystem.inst.RemoveJob(job, true);
                            }
                            cell.busyObj = null;
                        }
                        if (!cell.Busy)
                        {
                            //if (cell.TreeAmount != 0)
                            //	Mod.helper.Log("Forest Fire");

                            Fire fire = FireManager.inst.FirePrefab.Create(cell.Position, FireManager.inst.FirePrefab.transform.rotation);
                            fire.overrideForestChance = _forestChance;
                            fire.overrideBuildingChance = _buildingChance;
                            fire.overrideMeadowChance = _meadowChance;
                            fire.overrideCastleChance = _castleChance;
                            __result = fire;
                            return;
                        }
                    }
                }

                __result = null;
            }
        }
    }

    [HarmonyPatch(typeof(Fire), "Tick")]
    static class FireTickPatch
    {
        static void Postfix(Fire __instance, float ___burnTime, float ___burnoutTime, bool ___Extinguishable)
        {
            if (__instance.Cell == null)
                return;

            if (__instance.Cell.TreeAmount > 0)
            {
                float interval = ___burnoutTime / 4f;

                if (___burnTime > interval && __instance.Cell.TreeAmount > 3)
                {
                    TreeSystem.inst.FellTree(__instance.Cell, __instance.Cell.FirstTree());
                }
                if (___burnTime > 2f * interval && __instance.Cell.TreeAmount > 2)
                {
                    TreeSystem.inst.FellTree(__instance.Cell, __instance.Cell.FirstTree());
                }
                if (___burnTime > 3f * interval && __instance.Cell.TreeAmount > 1)
                {
                    TreeSystem.inst.FellTree(__instance.Cell, __instance.Cell.FirstTree());
                }
                if (___burnTime > 3.5f * interval && __instance.Cell.TreeAmount == 1)
                {
                    TreeSystem.inst.FellTree(__instance.Cell, __instance.Cell.FirstTree());
                }
            }
            else if(__instance.FireColor == Fire.FireColorVariant.YellowFire && !__instance.Dying && __instance.Cell.OccupyingStructure.Count == 0)
                typeof(Fire).GetMethod("Burnout", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[] { false });
        }
    }

    [HarmonyPatch(typeof(Fire))]
    static class FireBurnoutPatch
    {
        static MethodBase TargetMethod()
        {
            return typeof(Fire).GetMethod("Burnout", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        static bool Prefix(Fire __instance, bool burndown,

            ref bool ___dying,
            ParticleSystem ___fireParticles,
            ref float ___burnInternalVolume,
            ref float ___burnSoundFadeSpeed)
        {
            if (__instance.Dying)
                return false;

            if (burndown && __instance.Cell.TreeAmount > 0)
            {
                __instance.RemoveJobs();
                ___dying = true;

                ___fireParticles.Stop();
                ___burnInternalVolume = 0f;
                ___burnSoundFadeSpeed = 3f;

                TreeSystem.inst.FellAllTrees(__instance.Cell);
                __instance.Cell.Type = ResourceType.None;
                __instance.Cell.StorePostGenerationType();

                __instance.RemoveBusy();

                return false;
            }
            return true;
        }
    }
}
