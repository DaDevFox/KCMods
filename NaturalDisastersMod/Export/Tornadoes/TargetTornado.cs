using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NaturalDisastersMod
{
    class TargetTornado : Tornado
    {
        public Vector3 target;


        public void Start()
        {
            base.Start();
            
        }

        public void Update()
        {
            base.Update();
            UpdateDirectionalVelocity();
        }


        protected void UpdateDirectionalVelocity()
        {
            Vector3 diff = target = transform.position;
            Vector3 diffClamped = Vector3.ClampMagnitude(diff, directionalVelocityRange.Max);

            SetDirectionalVelocity(diffClamped);
        }

        


    }
}
