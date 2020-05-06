using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

namespace CrimeMod
{
    public class Criminal : MonoBehaviour
    {

        #region Timing

        public float crimeInterval = 10f;
        private float crimeTime = 0f;

        protected MinMax crimeRandomOffset = new MinMax(0f, 1f);

        public bool CrimeInProgress { get; private set; } = false;

        #endregion

        #region Criminal Type Settings

        public float c_severity = 0.2f;
        public string c_name = "Criminal";
        public string c_id = "criminal_default";

        #endregion

        #region Criminal Settings

        public float CrimeAccumulatedWeightage { get; protected set; }
        protected delegate void OnCapturedEventHandler();
        protected event OnCapturedEventHandler OnCapture;

        #endregion

        public Villager Host { get; set; }

        void Update(float dt)
        {

            crimeTime += Time.deltaTime;
            CheckCrimeInterval();
        }

        
        
        void CheckCrimeInterval()
        {
            if (crimeTime > crimeInterval)
            {
                crimeTime = crimeRandomOffset.Rand();
                DoCrime();
            }
        }


        protected virtual void DoCrime()
        {
            this.MarkCrimeBegin();
            SuspendVillagerJob(Host);
        }

        protected void MarkCrimeBegin()
        {
            this.CrimeInProgress = true;
        }

        protected void MarkCrimeEnded()
        {
            this.CrimeInProgress = false;
            UnsuspendVillagerJob(Host);
        }


        public virtual Criminal CreateInHost(Villager host)
        {
            this.Host = host;
            return this;
        }

        public virtual void OnVillagerFixedTick()
        {

        }

        private void SuspendVillagerJob(Villager villager)
        {
            villager.job.Employee = null;
            typeof(Job).GetField("assigned", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(villager.job, true);
        }

        private void UnsuspendVillagerJob(Villager villager)
        {
            villager.job.Employee = villager;
        }







    }
}
