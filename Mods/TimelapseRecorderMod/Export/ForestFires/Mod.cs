using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using Assets.Code;
using Assets.Code.Jobs;

namespace Fox.ForestFires
{
	public class Mod : MonoBehaviour
	{
		public static KCModHelper helper;

		private void Preload(KCModHelper helper)
		{
			Mod.helper = helper;
		}
	}

    [HarmonyPatch(typeof(FireManager), "StartFireAt")]
    class FirePatch
    {
        static void Postfix(ref Fire __result, Cell cell, float _forestChance = -1f, float _meadowChance = -1f, float _castleChance = -1f, float _buildingChance = -1f)
        {
			if (cell == null)
			{
				__result = null;
			}
			else
			{
				Building x = cell.FindHighestNotFireImmune();
				if (x == null && cell.TreeAmount == 0)
				{
					__result = null;
				}
				else if (cell.Type == ResourceType.Water && x == null)
				{
					__result = null;
				}
				else if (cell.Type == ResourceType.IronDeposit || cell.Type == ResourceType.Stone || cell.Type == ResourceType.UnusableStone)
				{
					__result = null;
				}
				else
				{
					//if (cell.TreeAmount != 0)
					//	Mod.helper.Log("Forest Fire Initing");

					if (!(cell.busyObj is Fire))
					{
						if (cell.busyObj != null && cell.busyObj is Job)
						{
							Job job = cell.busyObj as Job;
							if (job.Employee != null)
							{
								Player.inst.DestroyPerson(job.Employee, true);
							}
							if (job is ClearCutterJob)
							{
								JobSystem.inst.RemoveJob(job, true);
							}
							cell.busyObj = null;
						}
						if (!cell.Busy)
						{
							//if (cell.TreeAmount != 0)
							//	Mod.helper.Log("Forest Fire");

							Fire fire = FireManager.inst.FirePrefab.Create(cell.Position, FireManager.inst.FirePrefab.transform.rotation);
							fire.overrideForestChance = _forestChance;
							fire.overrideBuildingChance = _buildingChance;
							fire.overrideMeadowChance = _meadowChance;
							fire.overrideCastleChance = _castleChance;
							__result = fire;
							return;
						}
					}
				}

				__result = null;
			}        
		}
    }
}
