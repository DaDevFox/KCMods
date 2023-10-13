using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code;

namespace StatisticsMod
{ 

    static class Util
    {
        public static float RoundToFactor(float a, float factor)
        {
            return Mathf.Round(a / factor) * factor;
        }

    }
    public static class BuildingUtils
    {
        /// <summary>
        /// Returns a reference to the yield of a building
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        public static ResourceAmount Yield(this Building building)
        {
            YieldProducer producer = building.GetComponent<YieldProducer>();
            YieldProducerSeason seasonalProducer = building.GetComponent<YieldProducerSeason>();

            if (producer)
                return producer.Yield;
            if (seasonalProducer)
                return ResourceAmount.Make(seasonalProducer.YieldType, seasonalProducer.YieldAmt);

            return new ResourceAmount();
        }

        /// <summary>
        /// Sets the yield of a building to a new ResourceAmount. 
        /// <para>May only be a ResourceAmount with one resource type if targeting a seasonal producer (field/agricultural producer typically). </para>
        /// </summary>
        /// <param name="building"></param>
        /// <param name="amount"></param>
        public static void Yield(this Building building, ResourceAmount amount)
        {
            YieldProducer producer = building.GetComponent<YieldProducer>();
            YieldProducerSeason seasonalProducer = building.GetComponent<YieldProducerSeason>();
            if (producer)
                producer.Yield = amount;
            if (seasonalProducer)
            {
                FreeResourceType type = FreeResourceType.Apples;
                for (int i = 0; i < (int)FreeResourceType.NumTypes; i++)
                    if (amount.Get((FreeResourceType)i) > 0)
                        type = (FreeResourceType)i;
                seasonalProducer.YieldType = type;
                seasonalProducer.YieldAmt = amount.Get(type);
            }
        }
    }
}
