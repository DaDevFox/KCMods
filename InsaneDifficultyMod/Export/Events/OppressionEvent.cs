using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneDifficultyMod.Events
{
    class OppressionEvent : IDModEvent
    {
        int timeElapsed = 0;
        int happiness = 40;

        public override void Init()
        {
            base.Init();
            testFrequency = 1;

            saveID = "oppression";
            saveObject = typeof(OppressionEventSaveData);

        }

        public override bool Test()
        {
            base.Test();

            int total = UnitSystem.inst.GetPlayerArmies();
            int MilitaryPresenceThresh = Math.Max(Player.inst.TotalWorkers() / 100, Settings.minArmyAmountForOppression);

            if (total >= MilitaryPresenceThresh)
            {
                timeElapsed += 1;
                happiness -= Math.Max(Settings.oppresssionRamp,-40);

                return true;
            }
            else 
            {
                timeElapsed = 0;
                happiness += Math.Min(Settings.oppresssionRamp,40);

                List<Player.HappinessMod> mods = Player.inst.happinessMods;
                for(int i = 0; i < mods.Count; i++)
                {
                    if(mods[i].id == "oppression")
                    {
                        Player.inst.happinessMods.RemoveAt(i);
                    }
                }

                return false;
            }
        }

        public override void Run()
        {
            base.Run();

            Player.inst.AddHappinessMod("oppression", 1000, happiness, "Oprression: " + getDescription(happiness), Util.GetPlayerStartLandmass(), false, 40, -40);

        }

        private String getDescription(int amount)
        {
            String description = "";
            if(30 < amount && amount <= 40)
            {
                description = "The peasants fear the lord's military presence!";
            }
            if(20 < amount && amount <= 30)
            {
                description = "Most fear the crown, though some rebel";
            }
            if(10 < amount && amount <= 20)
            {
                description = "The Peasants grow braver than they once were";
            }
            if(0 < amount && amount <= 10)
            {
                description = "Our soldiers no longer have much effect against the people";
            }
            if(amount <= 0)
            {
                description = "Anger stirs against the throne!";
            }

            return description;
        }

        #region LoadSave

        public class OppressionEventSaveData
        {
            public int happiness;
        }

        public override void OnLoaded(object saveData)
        {
            base.OnLoaded(saveData);

            OppressionEventSaveData data = saveData as OppressionEventSaveData;
            this.happiness = data.happiness;
        }

        public override object OnSave()
        {
            OppressionEventSaveData data = new OppressionEventSaveData();
            data.happiness = this.happiness;
            return data;
        }

        #endregion

    }
}
