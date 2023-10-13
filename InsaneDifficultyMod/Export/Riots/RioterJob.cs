using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod.Events
{
    public class RioterJob : Job
    {
        public enum Status
        {
            idle,
            rallying,
            waitingAtRally,
            rioting,
            waitingAtKeep
        }

        public Status status;

        //public Cell rallyPoint = null;
        public Riot riot;
        public Riot.RiotBuildingMeta rallyPoint;

        public int index = 0;

        public RioterJob(IEmployer e) : base(e)
        {
            status = Status.idle;
            employer = e;
            riot = ((Riot.RiotBuildingMeta)e).riot;
            ManualAssignment = true;
        }


        //public void SetRallyPoint(Cell cell)
        //{
        //    rallyPoint = cell;
        //    status = Status.rallying;
        //    base.Employee.MoveToDeferred(rallyPoint.Center);
        //}

        public void ReportArrivalAtRallyPoint()
        {
            DebugExt.Log("arrival", true, KingdomLog.LogStatus.Neutral, Employee.Pos);
            
        }

        public override void OnEmployeeQuit()
        {

            base.OnEmployeeQuit();
        }

        public override void UpdateWithEmployee(float dt)
        {
            base.UpdateWithEmployee(dt);
            //Employee.textThought = "<color=red>Rioting!</color>";

            float snapThresh = 1f;
            if (status == Status.rallying)
            {
                if (Vector3.Distance(base.Employee.GetPosition(), rallyPoint.target.Center()) <= snapThresh)
                {
                    status = Status.waitingAtRally;
                    ReportArrivalAtRallyPoint();
                }
                else
                {
                    //Employee.MoveToDeferred(rallyPoint.Center);
                }
            }


        }


    }
}
