using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Assets.Code;
using UnityEngine;

namespace Fox.Loot
{
    public class Settings
    {
        public static bool individualModifier = false;
        public static MinMax dropPercentage = new MinMax(0.1f, 0.9f);
        public static MinMax tileVariation = new MinMax(-2f, 2f);
    }

    public class Mod
    {
        public static KCModHelper helper { get; private set; }

        private void Preload(KCModHelper helper)
        {
            var harmony = HarmonyInstance.Create("harmony");
            harmony.PatchAll();

            Broadcast.BuildingAddRemove.ListenAny((sender, data) =>
           {
               try
               {
                   if (!data.added)
                       OnBuildingDestroy(data.targetBuilding);
               }catch(Exception ex)
               {
                   helper.Log(ex.ToString());
               }
           });

            Application.logMessageReceived += (condition, stackTrace, type) =>
            {
                //if (type == LogType.Exception)
                //    helper.Log(condition + "\n" + stackTrace);
            };

            Mod.helper = helper;
        }

        private static void OnBuildingDestroy(Building building)
        {
            ResourceAmount loot = new ResourceAmount();
            if (!Settings.individualModifier)
                loot = building.GetCost() * Settings.dropPercentage.Rand();
            else
            {
                for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                    loot.Set(i, (int)(building.GetCost().Get((FreeResourceType)i) * Settings.dropPercentage.Rand()));
            }

            building.GetBoundingFootprint(out int minX, out int minZ, out int maxX, out int maxZ);
            
            int area = Math.Abs(maxX - minX) * Math.Abs(maxZ - minZ);
            if (area == 1)
            {
                for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                    FreeResourceManager.inst.GetAutoStackFor((FreeResourceType)i, loot.Get((FreeResourceType)i)).transform.position = building.transform.position;
                return;
            }

            int remaining = area;
            int lastVariation = 0;
            ResourceAmount iteration = new ResourceAmount();
            for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                iteration.Set(i, (int)(loot.Get((FreeResourceType)i) / area));

            building.ForEachTileInBounds((x, z, cell) =>
            {
                if (lastVariation == 0)
                    lastVariation = (int)Settings.tileVariation.Rand();
                else
                    lastVariation = -lastVariation;

                for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                    FreeResourceManager.inst.GetAutoStackFor((FreeResourceType)i, iteration.Get((FreeResourceType)i) + lastVariation).transform.position = cell.Center;
            });
        }
    }
}
