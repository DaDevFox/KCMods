﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReskinEngine.Engine
{
    public static class Settings
    {
        public enum PriorityType
        {
            Absolute,
            WeightedRandom
        }


        private static int defaultPriority = 1;

        // TODO: Update PrioritizedMods on ModPriority update and trigger event
        public static Dictionary<string, int> ModPriority { get; set; } = new Dictionary<string, int>();
        public static List<string> PrioritizedMods { get; private set; }
        public static PriorityType priorityType { get; set; } = PriorityType.Absolute;

        public static void Setup()
        {
            foreach(string mod in Engine.ModIndex.Keys)
            {
                ModPriority.Add(mod, defaultPriority);
                PrioritizedMods.Add(mod);
            }
            PrioritizedMods.OrderBy((mod) => ModPriority[mod]);
        }






    }
}
