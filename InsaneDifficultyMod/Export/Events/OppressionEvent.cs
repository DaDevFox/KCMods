using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneDifficultyMod.Events
{
    public class OppressionEvent : IDModEvent
    {
        private int timeElapsed = 0;
        private int happiness = 40;

        public static List<string> OppressionDescriptions { get; private set; } = new List<string>()
        {
            "Anger stirs against the throne!",
            "Our soldiers no longer have much effect against people",
            "Small bands of peasants have begun speaking out",
            "The peasants grow braver than they once were",
            "The lord's name is whispered on the streets",
            "The crown is feared",
            "Military presence has suppresed any of the peoples' grievences"
        };

        public static string oppressionHappinessModId = "oppression";




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
            int MilitaryPresenceThresh = Math.Max(Player.inst.TotalWorkers() / Settings.PopulationOppressionUnit, Settings.minArmyAmountForOppression);

            if (total >= MilitaryPresenceThresh)
            {
                timeElapsed += 1;
                happiness -= Settings.oppresssionRamp;

                happiness = Math.Max(happiness, Settings.MinOppressionHappiness);

                return true;
            }
            else 
            {
                timeElapsed = 0;
                happiness += Settings.oppresssionRamp;

                happiness = Math.Min(happiness, Settings.MaxOppressionHappiness);

                List<Player.HappinessMod> mods = Player.inst.happinessMods;

                for(int i = 0; i < mods.Count; i++)
                {
                    if(mods[i].id == oppressionHappinessModId)
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

            Player.inst.AddHappinessMod(oppressionHappinessModId, 1000, happiness, "Oppression: " + GetDescription(happiness), Util.GetPlayerStartLandmass(), false, Settings.MaxOppressionHappiness, -Settings.MinOppressionHappiness);
        }

        private string GetDescription(int amount)
        {
            string description = "";

            float normalizedAmount = amount - Settings.MinOppressionHappiness;
            float normalizedFactor = normalizedAmount / (Settings.MaxOppressionHappiness - Settings.MinOppressionHappiness);

            float min = float.MaxValue;
            int idx = -1;

            for(int i = 0; i < OppressionDescriptions.Count; i++)
            {
                float normalized = ((float)i) / OppressionDescriptions.Count;
                if(Math.Abs(normalized - normalizedFactor) < min)
                {
                    min = Math.Abs(normalized - normalizedAmount);
                    idx = i;
                }
            }

            if (idx != -1)
                description = OppressionDescriptions[idx];
            else
                description = "No descriptions found for oppression event";

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
