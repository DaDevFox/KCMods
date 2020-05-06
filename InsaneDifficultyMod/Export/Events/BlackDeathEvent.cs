using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneDifficultyMod.Events
{
    class BlackDeathEvent : IDModEvent
    {
        public override void Init()
        {
            base.Init();
            testFrequency = 1;

            saveID = "blackDeath";
            saveObject = typeof(BlackDeathEventSave);

        }

        public override bool Test() 
        {
            return false;
        }

        #region LoadSave

        public class BlackDeathEventSave 
        {




        }

        #endregion

    }
}
