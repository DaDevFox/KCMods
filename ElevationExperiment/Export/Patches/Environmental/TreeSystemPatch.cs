using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using System.Reflection;

namespace ElevationExperiment.Patches
{

	//[HarmonyPatch(typeof(TreeSystem),"SetTree")]
	class TreeSystemPatch
	{
		static void Prefix(Cell cell, ref Vector3 pos)
		{
			CellMark mark = ElevationManager.GetCellMark(cell);
			if(mark != null)
			{
				pos.y = mark.Elevation;
			}
		}

		public static void UpdateTrees()
		{
			TreeSystem.inst.Reset();
			List<Cell> tracked = new List<Cell>();
			foreach(ArrayExt<Cell> landmass in World.inst.cellsToLandmass)
			{
				foreach(Cell cell in landmass.data)
				{
					if (cell != null)
					{
						for (int i = 0; i < cell.TreeAmount; i++)
						{
							tracked.Add(cell);
						}

						TreeSystem.inst.DeleteTreesAt(cell);
					}
				}
			}
			foreach(Cell cell in tracked)
			{
				TreeSystem.inst.PlaceTree(cell);
			}

			
		}

	}





}
