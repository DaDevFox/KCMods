using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMod.Data
{
    public struct YearData
    {
        public static YearData Empty
        {
            get
            {
                return new YearData
                {
                    foodConsumptionData = new FoodConsumptionData()
                };
            }
        }
        public float AvgFoodConsumptionPerPerson
        {
            get
            {
                return (float)population / (float)foodConsumptionData.timesEaten;
            }
        }

        public int population;
        public FoodConsumptionData foodConsumptionData;
    }


    public struct FoodConsumptionData
    {
        public float FoodInsufficiency
        {
            get
            {
                return timesEaten - timesSatisfied;
            }
        }

        public int timesEaten;
        public int timesSatisfied;
    }

    public struct WoodConsumptionData
    {
        public int numProduced_clearcut;
        public int numProduced_forestry;
        public int numUsed;
        public int numRequested;
    }



}
