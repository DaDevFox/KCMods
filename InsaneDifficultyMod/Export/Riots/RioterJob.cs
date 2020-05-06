using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
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

        public Cell rallyPoint = null;
        public RiotSpawn riot;

        public RioterJob(IEmployer e) : base(e)
        {
            status = Status.idle;
            employer = e;
            riot = (RiotSpawn)e;
        }


        public void SetRallyPoint(Cell cell)
        {
            rallyPoint = cell;
            status = Status.rallying;
            base.Employee.MoveToDeferred(rallyPoint.Center);
        }

        public void ReportArrivalAtRallyPoint()
        {
            KingdomLog.TryLog("arrival", "arrival", KingdomLog.LogStatus.Neutral);
            riot.ReportArrivalAtRallyPoint(base.Employee);
        }

        public override void OnEmployeeQuit()
        {

            base.OnEmployeeQuit();
        }

        public override void UpdateWithEmployee(float dt)
        {
            base.UpdateWithEmployee(dt);
            Employee.textThought = "<color=red>Rioting!</color>";

            float snapThresh = 1f;
            if (status == Status.rallying)
            {
                if (Vector3.Distance(base.Employee.GetPosition(), rallyPoint.Center) <= snapThresh)
                {
                    status = Status.waitingAtRally;
                    ReportArrivalAtRallyPoint();
                }
            }


        }


    }
}
