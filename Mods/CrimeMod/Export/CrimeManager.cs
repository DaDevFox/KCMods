using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrimeMod.CriminalTypes;
using CrimeMod.Utils;
using Harmony;
using UnityEngine;

namespace CrimeMod
{
    public class CrimeManager
    {

        #region Crime Settings

        public static int crimeHappinessThreshold = 30;
        public static float criminalPickBuffer = 0.2f;

        #endregion

        #region Criminal Tracking

        private static Dictionary<Guid, Criminal> trackedCriminals = new Dictionary<Guid, Criminal>();
        public static Dictionary<Guid,Criminal> TrackedCriminals
        {
            get
            {
                return trackedCriminals;
            }
        }

        #endregion

        #region Criminal Types

        public static List<Criminal> CriminalTypes { get; set; } = new List<Criminal>() 
        {
            new Robber(),
        };

        #endregion

        public static void CheckHomesForCrime()
        {
            try
            {
                List<IResidence> residences = Player.inst.Residentials;
                foreach (IResidence residence in residences)
                {
                    CalcCrimeForHome(residence);
                }
            }
            catch(Exception ex)
            {
                DebugExt.HandleException(ex);
            }
        }

        static void CalcCrimeForHome(IResidence residence)
        {
            int happiness = residence.GetHappiness();
            int severity = (crimeHappinessThreshold - happiness);
            if (severity > 0)
            {
                float severityNormalized = (float)severity / (float)crimeHappinessThreshold;
                Mod.helper.Log("found home with potential crime: " + severityNormalized.ToString());
                foreach (Villager resident in residence.GetResidents())
                {
                    bool flaggedCriminal = SRand.Range(0f, 1f) < severityNormalized;

                    if (flaggedCriminal)
                    {
                        Mod.helper.Log("trying create criminal");
                        Criminal bestCriminal = GetBestCriminalForSeverity(severityNormalized);
                        
                        if (bestCriminal)
                        {
                            CreateCriminal(bestCriminal, resident);
                        }
                    }
                }
            }
        }

        
        public static void CreateCriminal(Criminal criminalType, Villager host)
        {
            if (trackedCriminals.ContainsKey(host.guid))
                return;

            Criminal instance = criminalType.CreateInHost(host);
            trackedCriminals.Add(host.guid, instance);

            Mod.helper.Log("created criminal: " + criminalType.c_name);
        }


        public static Criminal GetBestCriminalForSeverity(float severity)
        {
            Criminal best = null;

            foreach (Criminal criminalType in CriminalTypes)
            {
                Mod.helper.Log(criminalType.c_name + ", " + criminalType.c_severity.ToString());
                Debug.LogError("asdf");
                if (criminalType.c_severity < severity)
                {
                    bool flagSelected = SRand.Range(0f, 1f) < (1f + criminalPickBuffer) - criminalType.c_severity;

                    if (flagSelected)
                    {
                        if (best == null)
                        {
                            best = criminalType;
                        }
                        else
                        {
                            if (
                                Settings.criminalAssignmentPriority == Settings.CriminalAssignmentPriority.PrioritizeDangerousCriminals &&
                                criminalType.c_severity > best.c_severity
                                )
                            {
                                best = criminalType;
                            }
                            if (
                                Settings.criminalAssignmentPriority == Settings.CriminalAssignmentPriority.PrioritizeHarmlessCriminals &&
                                criminalType.c_severity < best.c_severity
                                )
                            {
                                best = criminalType;
                            }
                        }
                    }
                }
            }

            return best;
        }



        [HarmonyPatch(typeof(Villager), "FixedTick")]
        static class VillagerTickPatch
        {
            static void Postfix(Villager __instance)
            {
                if (trackedCriminals.ContainsKey(__instance.guid))
                {
                    trackedCriminals[__instance.guid].OnVillagerFixedTick();
                }
            }
        }


    }
}
