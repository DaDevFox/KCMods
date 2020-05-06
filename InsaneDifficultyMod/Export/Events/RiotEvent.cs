using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneDifficultyMod.Events
{
    class RiotEvent : IDModEvent
    {

        int happinessThreshold = 20;
        int accumTime = 0;
        int timeThreshold = 3;

        List<RiotSpawn> riots = new List<RiotSpawn>();

        #region Building Priorities
        //Building Priorities for being chosen as a riot rally location

        Dictionary<String, int> buildingPriorities = new Dictionary<string, int>()
        {
            { "townsquare", 6 },
            { "market", 6 },

            { "woodengate", 5 },
            { "gate", 5 },
            { "drawbridge", 5 },

            { "fountain", 4 },
            { "largefountain", 4 },

            { "road", 3 },
            { "stoneroad", 3 },
            { "bridge", 3 },
            { "stonebridge", 3 },

            { "garden", 2 },

            { "largestockpile", 1 },
            { "smallstockpile", 1 },
            { "dock", 1 },
            { "rubble", 1 },
        };
        #endregion

        public override void Init()
        {
            base.Init();

            testFrequency = 1;

            saveID = "riot";
            saveObject = typeof(RiotEventSaveData);
        }

        public override bool Test()
        {
            base.Test();
            if (Player.inst.KingdomHappiness < happinessThreshold)
            {
                accumTime += 1;
            }
            else 
            {
                accumTime = 0;
            }
            if (accumTime > timeThreshold) 
            {
                accumTime = 0;
                return true;
            }
            return false;
        }

        public override void Run()
        {
            base.Run();
            try
            {
                RiotSpawn riot = null;

                if (riots.Count > 0)
                {
                    foreach (RiotSpawn riotSpawn in riots)
                    {
                        if (riotSpawn.rioters.Count < Settings.riotMaxSize)
                        {
                            riot = riotSpawn;
                        }
                    }
                }
                if (riot == null)
                {
                    riot = new RiotSpawn(GetRallyPoint(Util.GetPlayerStartLandmass()));
                }

                ArrayExt<Villager> villagers = World.inst.GetVillagersForLandMass(Util.GetPlayerStartLandmass());

                int numRioters = Settings.riotStartSize;
                int num = (int)((float)numRioters / Settings.riotMaxSize);
                for (int k = 0; k < num + 1; k++)
                {
                    while (riot.rioters.Count < Settings.riotMaxSize && numRioters > 0)
                    {
                        Villager vil = Player.inst.Workers.data[SRand.Range(0,Player.inst.Workers.Count-1)];
                        RioterJob newRioter = new RioterJob(riot);
                        newRioter.AssignEmployee(vil);
                        newRioter.employer = riot;
                        JobSystem.inst.AddNewJob(newRioter, JobCategory.Homemakers);
                        numRioters -= 1; 
                    }
                    if (numRioters > 0)
                    {
                        riots.Add(riot);
                        riot = new RiotSpawn(GetRallyPoint(Util.GetPlayerStartLandmass()));
                    }
                    else
                    {
                        break;
                    }
                }


                KingdomLog.TryLog("riotAssembling", "My Lord, a riot has begun to assemble!", KingdomLog.LogStatus.Important, 20, riot.GetRallyPoint().Center);
            }catch(Exception ex)
            {
                KingdomLog.TryLog("Exception-" + SRand.Range(0, 100), ex.Message + "\n" + ex.StackTrace, KingdomLog.LogStatus.Neutral);
            }
        }

        private Cell GetRallyPoint(int landmass)
        {
            ArrayExt<Building> buildings = Player.inst.GetBuildingListForLandMass(landmass);
            int bestRallyPointPriority = 0;
            Building bestRallyPoint = buildings.data[0];
            foreach (Building b in buildings.data)
            {
                bool flag = false;
                foreach(RiotSpawn riot in riots)
                {
                    if (riot.rallyCell.BottomStructure.guid == b.guid)
                    {
                        flag = true;
                    }
                }

                if (b != null && !flag)
                {
                    if (buildingPriorities.ContainsKey(b.UniqueName))
                    {
                        if (buildingPriorities[b.UniqueName] > bestRallyPointPriority)
                        {
                            bestRallyPointPriority = buildingPriorities[b.UniqueName];
                            bestRallyPoint = b;
                        }
                    }
                    else
                    {
                        if (bestRallyPointPriority == 0)
                        {
                            bestRallyPoint = b;
                        }
                    }
                }
            }

            return bestRallyPoint.GetCell();
        }

        #region LoadSave


        public class RiotEventSaveData
        {
            public int accumTime;
            public List<RiotSpawn.RiotSpawnSaveData> riots;
        }


        //public override object OnSave()
        //{
        //    RiotEventSaveData data = new RiotEventSaveData();

        //    data.accumTime = this.accumTime;
        //    foreach(RiotSpawn riot in riots)
        //    {
        //        data.riots.Add(riot.GetSaveData());
        //    }
        //    return data;
        //}


        //public override void OnLoaded(object saveData)
        //{
        //    base.OnLoaded(saveData);
        //    RiotEventSaveData data = saveData as RiotEventSaveData;

        //    this.accumTime = data.accumTime;
        //    this.riots.Clear();
        //    foreach(RiotSpawn.RiotSpawnSaveData riotData in data.riots)
        //    {
        //        this.riots.Add(new RiotSpawn(riotData));
        //    }

        //}


        #endregion

    }
}
