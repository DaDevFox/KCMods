using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsaneDifficultyMod
{
    public class RiotSpawn : MonoBehaviour, IEmployer
    {

        public Assets.Code.ResourceAmount demand;

        public List<Villager> rioters = new List<Villager>();

        public GameObject rallyMarkerGO;
        public RiotRallyMarker rallyMarker;

        public List<Villager> riotersAtRally;
        public bool allPresentAtRally = false;

        public Cell rallyCell;
        public Vector3 rallyPos;

        

        public RiotSpawn(RiotSpawnSaveData data)
        {
            this.rioters.Clear();
            foreach(Guid guid in data.rioterGuids)
            {
                this.rioters.Add(Player.inst.GetWorker(guid));
            }

            this.riotersAtRally.Clear();
            foreach (Guid guid in data.riotersAtRallyGuids)
            {
                this.riotersAtRally.Add(Player.inst.GetWorker(guid));
            }

            allPresentAtRally = data.allAtRally;

            SetRallyPoint(World.inst.GetCellData(data.rallyPointPos));
        }

        public RiotSpawn(Cell cell)
        {
            SetRallyPoint(cell);
        }

        public RiotSpawn(Vector3 pos)
        {
            SetRallyPoint(World.inst.GetCellData(pos));
        }

        private void SetupRallyMarker() 
        {
            GameObject markerPrefab = AssetBundleManager.GetAsset("RiotRallyMarker.prefab") as GameObject;
            rallyMarkerGO = GameObject.Instantiate(markerPrefab, new Vector3(rallyPos.x,rallyPos.y,rallyPos.z), Quaternion.identity);
            rallyMarker = rallyMarkerGO.AddComponent<RiotRallyMarker>();
            rallyMarker.riot = this;
        }
        

        public void SetRallyPoint(Cell cell)
        {
            rallyCell = cell;
            rallyPos = cell.Center;
            foreach (Villager rioter in rioters)
            {
                KingdomLog.TryLog("vil", "vil", KingdomLog.LogStatus.Neutral);
                ((RioterJob)rioter.job).SetRallyPoint(cell);
            }

            SetupRallyMarker();
        }

        public Cell GetRallyPoint()
        {
            return rallyCell;
        }

        public int GetLandmass() 
        {
            return rallyCell.landMassIdx;
        }

        public void ReportArrivalAtRallyPoint(Villager person)
        {
            KingdomLog.TryLog("arrival2", "arrival2", KingdomLog.LogStatus.Neutral);
            riotersAtRally.Add(person);
            CheckRallyAttendance();
        }

        public void CheckRallyAttendance()
        {
            bool flag = false;
            foreach (Villager vil in rioters)
            {
                if (((RioterJob)vil.job).status != RioterJob.Status.waitingAtRally)
                {
                    flag = true;
                }
            }

            if (!flag)
            {
                allPresentAtRally = true;
                OnRallyOrganized();
            }
            else
            {
                allPresentAtRally = false;
            }
        }

        public void OnRallyOrganized()
        {
            KingdomLog.TryLog("riotAssembled", "My lord, the peasants demand that we pay tribute to them by the close of the year, else they shall march on your Keep!", KingdomLog.LogStatus.Warning, 20, GetRallyPoint());
        }

        Vector3 IEmployer.GetPositionForPerson(Villager rioter)
        {

            foreach (Villager vil in rioters)
            {
                if (vil.guid == rioter.guid)
                {

                    return vil.GetPosition();

                }
            }
            return Vector3.zero;
        }

        bool IEmployer.IsOpen()
        {
            return true;
        }

        void IEmployer.OnAssigned(Villager p)
        {
            rioters.Add(p);
            ((RioterJob)p.job).SetRallyPoint(rallyCell);
        }

        void IEmployer.OnUnAssigned()
        {
        }

        String IEmployer.GetUsedSkill()
        {
            return "";
        }

        int IEmployer.LandMass()
        {
            return this.rallyCell.landMassIdx;
        }

        public JobCategory GetJobCategory()
        {
            return JobCategory.Undefined;
        }

        #region Patches



        #endregion


        #region LoadSave

        public class RiotSpawnSaveData
        {
            public List<Guid> rioterGuids;
            public List<Guid> riotersAtRallyGuids;
            public bool allAtRally;
            public SerializableVector3 rallyPointPos;
        }

        public RiotSpawnSaveData GetSaveData()
        {
            RiotSpawnSaveData data = new RiotSpawnSaveData();
            data.allAtRally = allPresentAtRally;
            foreach(Villager vil in rioters)
            {
                data.rioterGuids.Add(vil.guid);
            }
            foreach(Villager vil in riotersAtRally)
            {
                data.riotersAtRallyGuids.Add(vil.guid);
            }
            data.rallyPointPos = this.rallyPos;
            return data;
        }
        

        #endregion



    }
}
