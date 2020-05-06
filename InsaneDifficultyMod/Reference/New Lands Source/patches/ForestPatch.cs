using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace New_Lands.patches
{

    //TREE GENERATION PATCH
    // Increases the amount of placable trees.
    [HarmonyPatch(typeof(TreeSystem))]
    [HarmonyPatch("TryInit")]
    public static class TreeGenerationPatch
    {

        // treeLimit should be a number between 1 and 100 since it gets
        // multiplied with 1023 in the original code later on.  
        private static int treeLimit = (25000 / 1023);
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            { 
                if (codes[i].opcode == OpCodes.Ldc_I4_5)
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_S, treeLimit);
                else
                    // TODO Insert implementation later
                    codes[i].opcode = codes[i].opcode;
            }
            return codes.AsEnumerable();
        }
    }



    [HarmonyPatch(typeof(TreeGrowth))]
    [HarmonyPatch("TickGrowthForCell")]
    public static class TreeGrowPatch
    {

        public static float TileChancesMin = 0.1f; // Original is 0.4f
        public static float TilecChanceMax = 0.1f; // Original is 0.8f;
        public static MinMax TileChance;

        public static float NewTileChanceMin = 0.1f; // Original is 0f
        public static float NewTileChanceMax = 0.1f; // Original is 0.2f

        public static int MaxTrees = 200; // Original is 1200

        public static int MaxTreesPerTile = 4;


        public static bool PreFix()
        {
            TreeGrowth TreeSyst = (TreeGrowth)GameObject.FindObjectOfType(typeof(TreeGrowth));

            TreeSyst.FillTileChance.Min = TileChancesMin;
            TreeSyst.FillTileChance.Max = TilecChanceMax;

            TreeSyst.GrowToNewTileChance.Min = NewTileChanceMin;
            TreeSyst.GrowToNewTileChance.Max = NewTileChanceMax;

            TreeSyst.MaxTreesOnMap = MaxTrees;
            return true;
        }
        
        
    }
}
