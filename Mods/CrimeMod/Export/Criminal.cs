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
    public abstract class Criminal : MonoBehaviour
    {

        #region Timing

        public float crimeInterval = 10f;
        private float crimeTime = 0f;

        protected MinMax crimeRandomOffset = new MinMax(0f, 1f);

        public bool CrimeInProgress { get; private set; } = false;
        public bool Active { get; private set; } = false;

        #endregion

        #region Criminal Type Settings

        public virtual float c_severity { get; } = 0.2f;
        public virtual string c_name { get; } = "Criminal";
        public abstract string c_id { get; } 

        #endregion

        #region Criminal Settings

        public float CrimeAccumulatedWeightage { get; protected set; }
        /// <summary>
        /// What to do right before the criminal is deactivated and the villager is escorted to a holding cell
        /// </summary>
        protected event Action OnCapture;

        #endregion

        public Villager Host { get; set; }

        private void Update()
        {

            crimeTime += Time.deltaTime;
            CheckCrimeInterval();
        }

        
        
        private void CheckCrimeInterval()
        {
            if (crimeTime > crimeInterval)
            {
                crimeTime = crimeRandomOffset.Rand();
                DoCrime();
            }
        }

        /// <summary>
        /// Begins a crime and suspends the host's job
        /// </summary>
        protected virtual void DoCrime()
        {
            this.MarkCrimeBegin();
        }

        protected void MarkCrimeBegin()
        {
            this.CrimeInProgress = true;
            SuspendVillagerJob(Host);
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
