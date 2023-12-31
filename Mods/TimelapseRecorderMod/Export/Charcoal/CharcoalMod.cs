using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Assets.Code;

namespace Fox.Charcoal
{
    public class CharcoalMod
    {
        public static float CharcoalPerWood { get; } = 0.5f;

        private void Preload()
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Fire), "Burnout")]
    public class DestroyPatch
    {
        static void Prefix(Fire __instance, bool burndown)
        {
            if (!burndown)
                return;

            Cell cell = typeof(Fire).GetField("cell", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance) as Cell;
            
            Building building = cell.FindHighestNotFireImmune();
            if (building)
            {

                ResourceAmount cost = (ResourceAmount)typeof(Building).GetField("Cost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(building);

                int wood = cost.Get(FreeResourceType.Tree);
                int charcoal = (int)((float)wood * CharcoalMod.CharcoalPerWood);

                FreeResourceManager.inst.GetAutoStackFor(FreeResourceType.Charcoal, charcoal).transform.position = building.transform.position;
            }

            int trees = cell.TreeAmount;
            if(trees > 0)
            {
                float charcoal = (trees * 2f) * CharcoalMod.CharcoalPerWood;
                FreeResourceManager.inst.GetAutoStackFor(FreeResourceType.Charcoal, (int)charcoal).transform.position = cell.Center;
            }

        }

    }
}
