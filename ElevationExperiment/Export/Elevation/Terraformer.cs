using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace ElevationExperiment
{
    static class Terraformer
    {
        public static void SetCellHeight(Cell cell, float height)
        {
			SetCellMeshHeight(cell, height);
			
		}

		public static void SetCellMeshHeight(Cell cell, float height)
		{
			//	int x =	cell.x;
			//	int z = cell.z;

			//	int num = x * 2 + 1;
			//	int num2 = z * 2 + 1;


			//	TerrainGen.inst.SetVert(num - 1, num2 - 1, height);
			//	TerrainGen.inst.SetVert(num, num2 - 1, height);
			//	TerrainGen.inst.SetVert(num + 1, num2 - 1, height);
			//	TerrainGen.inst.SetVert(num - 1, num2, height);
			//	TerrainGen.inst.SetVert(num, num2, height);
			//	TerrainGen.inst.SetVert(num + 1, num2, height);
			//	TerrainGen.inst.SetVert(num - 1, num2 + 1, height);
			//	TerrainGen.inst.SetVert(num, num2 + 1, height);
			//	TerrainGen.inst.SetVert(num + 1, num2 + 1, height);

			//DebugExt.Log(
			//	TerrainGen.inst.GetVertY(num - 1, num2 - 1).ToString() + ", " +
			//	TerrainGen.inst.GetVertY(num, num2 - 1).ToString() + ", " +
			//	TerrainGen.inst.GetVertY(num + 1, num2 - 1).ToString() + "|"
			//);
		}

		public static void SetChunkVert(TerrainChunk chunk, int id, float height)
		{
			Vector3[] vertices = typeof(TerrainChunk).GetField("verticies", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(chunk)as Vector3[];
			vertices[id].y = height;

			typeof(TerrainChunk).GetField("verticies", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(chunk, vertices);
		}






		public static void SetLocalEdgeHeight(Cell cell, float height)
		{





		}





		private static void SetVert(int meshX, int meshZ, float y)
		{
			for (int i = 0; i < TerrainGen.inst.terrainChunks.Count; i++)
			{
				TerrainChunk terrainChunk = TerrainGen.inst.terrainChunks[i];
				int num = meshX - terrainChunk.x;
				int num2 = meshZ - terrainChunk.z;
				if (num >= 0 && num <= terrainChunk.SizeX * 2 && num2 >= 0 && num2 <= terrainChunk.SizeZ * 2)
				{
					terrainChunk.SetVert(num, num2, y);
				}
			}
		}


		private static void SetChunkVert(TerrainChunk chunk, int gridX, int gridZ, float y)
		{
			ArrayExt<ArrayExt<int>> indexLookup = typeof(TerrainChunk).GetProperty("indexLookup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(chunk) as ArrayExt<ArrayExt<int>>;
			int meshSizeX = (int)typeof(TerrainChunk).GetProperty("meshSizeX", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(chunk);
				

			ArrayExt<int> arrayExt = indexLookup.data[gridX + gridZ * meshSizeX];
			int i = 0;
			int count = arrayExt.Count;
			while (i < count)
			{
				int num = arrayExt.data[i];
				//Vector3 vector = chunk.verticies[num];
				//chunk.verticies[num] = new Vector3(vector.x, y, vector.z);
				i++;
			}
			chunk.dirtyVerts = true;
		}


    }




}
