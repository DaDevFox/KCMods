using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevationExperiment
{
    static class PathingManager
    {
        public static int tierPathingCost = 50;
        public static int tierPathingMin = 40;

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
