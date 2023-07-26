using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMod.Data
{
    static class Analyzer
    {

        #region Food

        public static int GetRequiredFoodForYear(YearData yearData)
        {
            return yearData.foodConsumptionData.timesEaten - yearData.foodConsumptionData.timesSatisfied;
        }

        public static int GetRequiredFoodForYear(FoodConsumptionData consumptionData)
        {
            return consumptionData.timesEaten - consumptionData.timesSatisfied;
        }

        public static int GetRequiredFoodForYear(int yearNum)
        {
            YearData yearData = DataContainer.GetYearData(yearNum);
            return yearData.foodConsumptionData.timesEaten - yearData.foodConsumptionData.timesSatisfied;
        }

        public static int GetPredictedFoodInsufficiencyForPeople(int numPeople, List<YearData> sampleData)
        {
            int estRequiredFood = GetEstimatedRequiredFoodForPeople(numPeople, sampleData);
            Player.Production production = Analytics.GetGameCalculatedProductionForKingdom();

            return estRequiredFood - (int)GetFoodProduction();
        }

        public static int GetFoodProduction()
        {   
            float estFoodProductionW = Analytics.GetProductionPowerForResourceInKingdom(FreeResourceType.Wheat);
            float estFoodProductionA = Analytics.GetProductionPowerForResourceInKingdom(FreeResourceType.Apples);
            float estFoodProductionF = Analytics.GetProductionPowerForResourceInKingdom(FreeResourceType.Fish);
            float estFoodProductionP = Analytics.GetProductionPowerForResourceInKingdom(FreeResourceType.Pork);


            float estFoodProduction =
                estFoodProductionW +
                estFoodProductionA +
                estFoodProductionF +
                estFoodProductionP;

            return (int)estFoodProduction;
        }

        public static int GetEstimatedRequiredFoodForPeople(int numPeople, List<YearData> sampleData)
        {
            float average = CalcAverageFoodConsumptionPerPerson(sampleData);
            return (int)Math.Ceiling((float)numPeople * average);
        }

        public static float CalcAverageFoodConsumptionPerPerson(List<YearData> sampleData)
        {
            if (Settings.useWeightedCalculations)
            {
                int numPeople = 0;
                int amountFoodConsumed = 0;
                foreach (YearData sample in sampleData)
                {
                    numPeople += sample.population;
                    amountFoodConsumed += sample.foodConsumptionData.timesEaten;
                }

                return (float)amountFoodConsumed / (float)numPeople;
            }
            else
            {
                float total = 0;
                foreach(YearData sample in sampleData)
                {
                    total += sample.AvgFoodConsumptionPerPerson;
                }


                return total / sampleData.Count;
            }

        }

        #endregion

    }
}
