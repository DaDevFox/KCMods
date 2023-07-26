using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CrimeMod.Utils;


namespace CrimeMod.CriminalTypes
{
    public abstract class Thief : Criminal
    {
        private Building target;
        private int stealAmount = 10;

        public abstract FreeResourceType stealResource { get; }
        public abstract string[] targetBuildingUniqueNames { get; }

        public override string c_name => "Thief";
        public override float c_severity => 0.2f;

        public int AmountStolen { get; private set; }

        public Thief()
        {
            OnCapture += OnCapture;
        }

        protected virtual void OnCapture()
        {
            if(stealResource == FreeResourceType.Gold)
                Player.inst.PlayerLandmassOwner.Gold += AmountStolen;
            else
                // add resource back
            AmountStolen = 0;
        }

        protected override void DoCrime()
        {
            base.DoCrime();

            if (FindTarget() != null)
                target = FindTarget();

            if (target)
            {
                Host.MoveTo(target.transform.position);
            }
        }

        private Building FindTarget()
        {
            Building best = null;
            foreach (string uniqueName in targetBuildingUniqueNames)
            {
                if (Player.inst.DoesAnyBuildingHaveUniqueNameOnLandMass(uniqueName, Host.landMass))
                {
                    ArrayExt<Building> targets = Player.inst.GetBuildingListForLandMass(Host.landMass, uniqueName.GetHashCode());

                    best = targets.RandomElement();

                    foreach(Building building in targets)
                    {
                        // TODO: Check if building has storage for resource

                    }
                }
            }
            return best;
        }

        public override Criminal CreateInHost(Villager host)
        {
            Criminal instance = new Robber() { Host = host };
            return instance;
        }

        public override void OnVillagerFixedTick()
        {
            base.OnVillagerFixedTick();
            if (CrimeInProgress && target)
            {
                // TODO: Check if host is travelling to target and set destination to there if not already

                if (Vector3.Distance(Host.transform.position, target.transform.position) < 1f)
                {
                    DoSteal();
                    MarkCrimeEnded();
                }
            }
        }

        private void DoSteal()
        {
            if (stealResource == FreeResourceType.Gold)
            {
                if (Player.inst.PlayerLandmassOwner.Gold > stealAmount)
                {
                    Player.inst.PlayerLandmassOwner.Gold -= stealAmount;
                    AmountStolen += stealAmount;
                }
                else
                {
                    AmountStolen += Player.inst.PlayerLandmassOwner.Gold;
                    Player.inst.PlayerLandmassOwner.Gold = 0;
                }
            }
            DebugExt.Log("Thief has stolen " + stealAmount.ToString() + " from a treasury. ", false, KingdomLog.LogStatus.Neutral, target.transform.position);
        }
    }

    public class Robber : Thief
    {

        public override string c_name => "Robber";
        public override string c_id => "robber";
        public override float c_severity => 0.4f;

        public override FreeResourceType stealResource => FreeResourceType.Gold;
        public override string[] targetBuildingUniqueNames => new string[]{ "throneroom" };

        protected override void OnCapture()
        {
            base.OnCapture();

            KingdomLog.TryLog("thiefcaptured", "My lord, a robber has been caught, in their crimes they have robbed " + AmountStolen.ToString() + " of the royal treasury, they will return the gold and anwser for their crimes", KingdomLog.LogStatus.Neutral);
        }


        public override Criminal CreateInHost(Villager host)
        {
            Criminal instance = new Robber() { Host = host };
            return instance;
        }
    }

    public class GrainThief : Thief
    {
        public override float c_severity => 0.1f;

        public override FreeResourceType stealResource => throw new NotImplementedException();
        public override string[] targetBuildingUniqueNames => throw new NotImplementedException();
        public override string c_id => "grain_thief";
    }
}
