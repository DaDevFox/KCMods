using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ElevationExperiment
{
    static class PathingManager
    {
        public static int tierPathingCost = 50;
        public static int tierPathingMin = 40;

		public static bool GetCardinal(Cell from, Cell to, out Direction direction)
		{
			Dictionary<Vector3, Direction> dirs = new Dictionary<Vector3, Direction>()
			{
				{ new Vector3(1f, 0f, 0f), Direction.East },
				{ new Vector3(0f, 0f, 1f), Direction.South },
				{ new Vector3(-1f, 0f, 0f), Direction.West },
				{ new Vector3(0f, 0f, -1f), Direction.North },
			};

			Vector3 diff = from.Center.xz() - to.Center.xz();
			diff = Vector3.ClampMagnitude(diff, 1f);

			if (dirs.ContainsKey(diff))
			{
				direction = dirs[diff];
				return true;
			}
			direction = Direction.North;
			return false;
		}




		public static bool BlockedCompletely(Cell cell)
		{
			bool blocked = false;
			if (cell != null)
			{

				CellMark mark = ElevationManager.GetCellMark(cell);
				if (mark != null)
					if (mark.blockers.Count == 4)
						blocked = true;

				if (BlockedTilePruner.Pruned)
				{
					if (BlockedTilePruner.Unreachable.Contains(cell))
						blocked = true;
				}

				if (BlocksForBuilding(cell))
					blocked = true;
			}
			return blocked;
		}

		public static bool BlocksForBuilding(Cell c)
		{
			return c.Type == ResourceType.IronDeposit || 
				c.Type == ResourceType.Stone || 
				c.Type == ResourceType.UnusableStone || 
				c.Type == ResourceType.WolfDen || 
				c.Type == ResourceType.WitchHut;
		}



	}
}
