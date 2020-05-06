using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CrimeMod.Utils;


namespace CrimeMod.CriminalTypes
{
    class Thief : Criminal
    {
        private ThroneRoom target;
        private int stealAmount = 10;

        public int AmountStolen { get; private set; }

        public Thief() 
        { 
            this.c_name = "Thief";
            this.c_id = "thief";
            this.c_severity = 0.2f;

            OnCapture += Thief_OnCapture;
        }

        private void Thief_OnCapture()
        {
            KingdomLog.TryLog("thiefcaptured", "My lord, a thief has been caught, in their crimes they have robbed " + AmountStolen.ToString() + " of the royal treasury, they will return the gold and anwser for their crimes", KingdomLog.LogStatus.Neutral);
            Player.inst.PlayerLandmassOwner.Gold += AmountStolen;
        }

        protected override void DoCrime()
        {
            base.DoCrime();

            if(FindTargetTreasury() != null)
                target = FindTargetTreasury();

            if (target)
            {
                Host.MoveTo(target.transform.position);
            }
        }

        private ThroneRoom FindTargetTreasury()
        {
            ThroneRoom best = null;
            if(Player.inst.DoesAnyBuildingHaveUniqueNameOnLandMass("throneroom", Host.landMass))
            {
                ArrayExt<Building> throneRooms = Player.inst.GetBuildingListForLandMass(Host.landMass, "throneroom".GetHashCode());

                best = throneRooms.RandomElement().GetComponent<ThroneRoom>();
            }
            return best;
        }

        public override Criminal CreateInHost(Villager host)
        {
            Criminal instance = new Thief();
            return instance;
        }

        public override void OnVillagerFixedTick()
        {
            base.OnVillagerFixedTick();

            if(Vector3.Distance(Host.transform.position,target.transform.position) < 1f)
            {
                DoSteal();
                MarkCrimeEnded();
            }
        }

        private void DoSteal()
        {
            if(Player.inst.PlayerLandmassOwner.Gold > stealAmount)
            {
                Player.inst.PlayerLandmassOwner.Gold -= stealAmount;
                AmountStolen += stealAmount;
            }
            else
            {
                AmountStolen += Player.inst.PlayerLandmassOwner.Gold;
                Player.inst.PlayerLandmassOwner.Gold = 0;
            }
            DebugExt.Log("Thief has stolen " + stealAmount.ToString() + " from a treasury. ",false, KingdomLog.LogStatus.Neutral,target.transform.position);
        }

    }
}
