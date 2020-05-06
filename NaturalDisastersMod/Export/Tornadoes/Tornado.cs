using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NaturalDisastersMod
{
    class Tornado : MonoBehaviour
    {
        public bool active = true;
        public float deathSpeedThreshold = 0.05f;

        public Vector3 spawnPos = Vector3.zero;

        public float deathTimeMax = 20f;
        private float deathTime = 0f;

        public float timeToKillMax = 20;
        private float timeToKill = 0f;

        public float tornadoMaxTime = 60f;
        private float timeRunning = 0;

        public float rotationSpeed = 2f;
        public float rotationRadius = 1f;

        public float turnSpeed = 0.15f;

        protected Vector3 directionalVelocity;
        protected Vector3 intendedDirectionalVelocity;
        public float directionalVelocityStrength = 2f;

        public MinMax directionalVelocityRange = new MinMax(-1, 1);

        public float drag = 0.87f;

        

        private float rotation;
        private float originalRotationSpeed;

        private ParticleSystem tornadoParticles = null;
        

        


        public void Update()
        {
            if (active)
            {
                Vector3 movement = CalcMovement();
                transform.position += movement;
                CalcDirectionalVelocity();

                CheckDirectionalStrength();
                timeToKill = 0f;
            }
            else
            {
                timeToKill += Time.deltaTime;
            }

            UpdateParticles();
            timeRunning += Time.deltaTime;
        }

        public void Start()
        {
            spawnPos = transform.position;
            DebugExt.Log("Tornado Spawn",true,KingdomLog.LogStatus.Neutral,gameObject);
            originalRotationSpeed = rotationSpeed;
            if (gameObject.transform.Find("Base") != null)
            {
                tornadoParticles = gameObject.transform.Find("Base").GetComponent<ParticleSystem>();
            }
            TryRandomDirectionalVelocity();
            UpdateParticles();
        }


        

        protected void CheckDirectionalStrength()
        {
            float max = Util.Vector3MaxValue(directionalVelocity);
            if(max < deathSpeedThreshold)
            {
                deathTime += Time.deltaTime;
            }
            else
            {
                deathTime -= Time.deltaTime;
            }
            if(deathTime > deathTimeMax)
            {
                BeginDeath();
            }
            float percent = directionalVelocityRange.Max / Math.Abs(max);
            rotationSpeed = originalRotationSpeed * percent;
        }

        protected void CheckDeath() 
        {
            if (active)
            {
                if (timeRunning > tornadoMaxTime)
                {
                    BeginDeath();
                }
            }
            else
            {
                if (deathTime > deathTimeMax)
                {
                    FinishDeath();
                }
            }
        
        }


        protected void BeginDeath()
        {
            active = false;
            DebugExt.Log("Tornado ded", true, KingdomLog.LogStatus.Neutral, transform.position);
            Events.TornadoEvent.OnTornadoDeath(gameObject);
            
        }


        protected void FinishDeath()
        {
            GameObject.Destroy(this);
        }


        protected void UpdateParticles()
        {
            if (active)
            {
                //DebugExt.Log("Active", true);
                if (!tornadoParticles.isPlaying)
                {
                    DebugExt.Log("Playing particles");
                    tornadoParticles.Play();
                }
            }
            else
            {
                if (!tornadoParticles.isStopped)
                {
                    tornadoParticles.Stop();
                }

            }
        }


        #region Movement

        protected void CalcDirectionalVelocity()
        {
            directionalVelocity = Vector3.Lerp(directionalVelocity, intendedDirectionalVelocity, Time.deltaTime * turnSpeed);
            if (directionalVelocity == intendedDirectionalVelocity)
            {
                directionalVelocity *= drag;
            }
        }

        protected void SetDirectionalVelocity(Vector3 value)
        {
            intendedDirectionalVelocity = value;
        }

        protected void TryRandomDirectionalVelocity()
        {
            float x = directionalVelocityRange.Rand();
            float z = directionalVelocityRange.Rand();
            SetDirectionalVelocity( new Vector3(x, 0, z) * directionalVelocityStrength);
        }

        public void TryTurnAwayFrom(Vector3 pos)
        {
            Vector3 diff = pos - transform.position;
            SetDirectionalVelocity(Vector3.ClampMagnitude(diff, directionalVelocityRange.Max));
        }



        protected Vector3 CalcMovement()
        {
            Vector3 movement = Vector3.zero;
            movement += CalcRotationalMovement();
            movement += directionalVelocity;

            movement *= Time.deltaTime;

            return movement;
        }


        protected Vector3 CalcRotationalMovement()
        {
            Vector3 movement = Vector3.zero;

            float rotation = Util.DegreesToRadians(rotationSpeed % 360);
            this.rotation += rotation;

            float xVal = (float)Math.Sin(this.rotation);
            float zVal = (float)Math.Cos(this.rotation);


            movement.x += float.IsNaN(xVal) ? 0 : xVal;
            movement.z += float.IsNaN(zVal) ? 0 : zVal;
            movement *= rotationRadius;

            return movement;
        }

        #endregion

    }
}
