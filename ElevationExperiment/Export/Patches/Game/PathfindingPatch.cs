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

	enum Diagonal
	{
		NorthEast,
		NorthWest,
		SouthEast,
		SouthWest
	}


	[HarmonyPatch(typeof(Pathfinder),
		"AddToOpenSet",
		new Type[] {
			typeof(Pathfinder.Node),
			typeof(Pathfinder.Node),
			typeof(int),
			typeof(int)
		})]
	public class PathfindingBlockerCheckPatch
	{
		static bool Prefix(Pathfinder.Node node, Pathfinder.Node parent)
		{
			if (BlocksPathDirectional(parent.cell, node.cell))
			{
				return false;
			}
			return true;
		}

		public static bool BlocksPathDirectional(Cell from, Cell to)
		{
			try
			{
				CellMark markFrom = ElevationManager.GetCellMark(from);
				CellMark markTo = ElevationManager.GetCellMark(to);

				Dictionary<Vector3, Direction> dirs = new Dictionary<Vector3, Direction>()
				{
					{ new Vector3(1f, 0f, 0f), Direction.East },
					{ new Vector3(0f, 0f, 1f), Direction.South },
					{ new Vector3(-1f, 0f, 0f), Direction.West },
					{ new Vector3(0f, 0f, -1f), Direction.North },
				};

				Dictionary<Vector3, Diagonal> diagonals = new Dictionary<Vector3, Diagonal>()
				{
					{ new Vector3(1f,0f,1f), Diagonal.SouthEast },
					{ new Vector3(1f,0f,-1f), Diagonal.NorthEast },
					{ new Vector3(-1f,0f,1f), Diagonal.SouthWest },
					{ new Vector3(-1f,0f,-1f), Diagonal.NorthWest },
				};


				if (markFrom != null && markTo != null)
				{
					if (markFrom.elevationTier > 0 || markTo.elevationTier > 0)
					{
						Vector3 diff = from.Center - to.Center;
						Vector3 diffNormalized = Vector3.ClampMagnitude(new Vector3(diff.x, 0f, diff.z), 1f);

						bool validCardinal = false;
						Direction dir = Direction.North;

						if (dirs.ContainsKey(diffNormalized))
						{
							validCardinal = true;
							dir = dirs[diffNormalized];
						}


						if (validCardinal)
						{
							if (markFrom.blockers.Contains(dir) || markTo.blockers.Contains(dir))
								return true;
							else
								return false;
						}
						else
							return true;
					}
				}
			}
			catch (Exception ex)
			{
				DebugExt.HandleException(ex);
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(Pathfinder), "SearchForClosestUnblockedCell")]
	public class BlockedSearchCellPatch
	{
		static void Postfix(Pathfinder __instance, ref Pathfinder.Node __result, int sx, int sz, Vector3 start, Vector3 end, int teamId)
		{
			try
			{
				Cell cell = null;
				float num = float.MaxValue;
				int num2 = 1;
				int num3 = Mathf.Clamp(sx - num2, 0, World.inst.GridWidth - 1);
				int num4 = Mathf.Clamp(sx + num2, 0, World.inst.GridWidth - 1);
				int num5 = Mathf.Clamp(sz - num2, 0, World.inst.GridHeight - 1);
				int num6 = Mathf.Clamp(sz + num2, 0, World.inst.GridHeight - 1);
				for (int i = num3; i <= num4; i++)
				{
					for (int j = num5; j <= num6; j++)
					{
						Cell cellDataUnsafe = World.inst.GetCellDataUnsafe(i, j);
						if (cellDataUnsafe != null)
						{
							if (!__instance.blocksPath(cellDataUnsafe, teamId) && !PathingManager.BlockedCompletely(cellDataUnsafe))
							{
								float num7 = Mathff.DistSqrdXZ(cellDataUnsafe.Center, start);
								if (num7 < num)
								{
									num = num7;
									cell = cellDataUnsafe;
								}
							}
						}
					}
				}
				if (cell != null)
				{
					__result = __instance.GetFieldValue<Pathfinder.Node[,]>("pathGrid")[cell.x, cell.z];
					return;
				}
				__result = null;
			}
			catch(Exception ex)
			{
				DebugExt.HandleException(ex);
			}
		}


		public static void DoElevationBlock(Cell cell, ref bool result)
		{
			
			CellMark mark = ElevationManager.GetCellMark(cell);
			if (mark != null)
			{
				if (PathingManager.BlockedCompletely(cell))
				{
					result = true;
				}
			}
		}

		static void Finalizer(Exception __exception)
		{
			DebugExt.HandleException(__exception);
		}

	}



	[HarmonyPatch(typeof(Pathfinder), "FindPath")]
	class FindPathRecalculatePatch
	{
		static void Prefix(Pathfinder __instance, ref Vector3 startPos, ref Vector3 endPos, int teamId)
		{
			bool flagStart = false;
			bool flagEnd = false;

			Cell start = World.inst.GetCellData(startPos);
			Cell end = World.inst.GetCellData(endPos);

			Pathfinder.Node newStart = null;
			Pathfinder.Node newEnd = null;

			if(start != null)
			{
				if (PathingManager.BlockedCompletely(start))
				{
					newStart = typeof(Pathfinder).GetMethod("SearchForClosestUnblockedCell",BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[]
					{
						(int)startPos.x,
						(int)startPos.z,
						startPos,
						endPos,
						teamId
					}) as Pathfinder.Node;

					if (newStart != null) 
						flagStart = true;
				}
			}
			if (end != null)
			{
				if (PathingManager.BlockedCompletely(end))
				{
					newEnd = typeof(Pathfinder).GetMethod("SearchForClosestUnblockedCell", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[]
					{
						(int)endPos.x,
						(int)endPos.z,
						endPos,
						startPos,
						teamId
					}) as Pathfinder.Node;

					if(newEnd != null)
						flagEnd = true;
				}
			}

			if (flagStart)
			{
				startPos = newStart.cell.Center;
			}
			if (flagEnd)
			{
				endPos = newEnd.cell.Center;
			}

		}


		static void Finalizer(Exception __exception)
		{
			DebugExt.HandleException(__exception);
		}


	}


	[HarmonyPatch(typeof(Pathfinder), "FindPathRaw")]
	class FindPathRawRecalculatePatch
	{
		static void Prefix(Pathfinder __instance, ref Vector3 startPos, ref Vector3 endPos, int teamId)
		{
			bool flagStart = false;
			bool flagEnd = false;

			Cell start = World.inst.GetCellData(startPos);
			Cell end = World.inst.GetCellData(endPos);

			Pathfinder.Node newStart = null;
			Pathfinder.Node newEnd = null;

			if (start != null)
			{
				if (PathingManager.BlockedCompletely(start))
				{
					newStart = typeof(Pathfinder).GetMethod("SearchForClosestUnblockedCell", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[]
					{
						(int)startPos.x,
						(int)startPos.z,
						startPos,
						endPos,
						teamId
					}) as Pathfinder.Node;

					flagStart = true;
				}
			}
			if (end != null)
			{
				if (PathingManager.BlockedCompletely(end))
				{
					newEnd = typeof(Pathfinder).GetMethod("SearchForClosestUnblockedCell", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance, new object[]
					{
						(int)endPos.x,
						(int)endPos.z,
						endPos,
						startPos,
						teamId
					}) as Pathfinder.Node;

					flagEnd = true;
				}
			}

			if (flagStart)
			{
				startPos = newStart.cell.Center;
			}
			if (flagEnd)
			{
				endPos = newEnd.cell.Center;
			}
		}

		static void Finalizer(Exception __exception)
		{
			DebugExt.HandleException(__exception);
		}

	}


	#region Path Blockers

	[HarmonyPatch(typeof(World), "GetBlocksFootPath")]
	class WorldFootPathBlockerPatch
	{
		static void Postfix(ref bool __result, Cell cell)
		{
			BlockedSearchCellPatch.DoElevationBlock(cell, ref __result);
			
		}
	}

	[HarmonyPatch(typeof(World), "GetBlocksFootPathForArmies")]
	class WorldArmyPathBlockerPatch
	{
		static void Postfix(ref bool __result, Cell c)
		{
			BlockedSearchCellPatch.DoElevationBlock(c, ref __result);
		}
	}

	[HarmonyPatch(typeof(World), "GetBlocksFootPathForOgres")]
	class WorldOgreFootPathBlockerPatch
	{
		static void Postfix(ref bool __result, Cell cell)
		{
			
			BlockedSearchCellPatch.DoElevationBlock(cell, ref __result);
		}
	}

	[HarmonyPatch(typeof(PlacementMode), "blocksPath")]
	class PlacementModePathBlockerPatch
	{
		static void Postfix(ref bool __result, Cell c)
		{
			BlockedSearchCellPatch.DoElevationBlock(c, ref __result);
		}
	}

	#endregion
}
