using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMod.Data
{
    static class Extrapolater
    {

        private static float thresh_foodConsumptionRateIrregularity = 0.3f;
        private static float thresh_foodInsufficiency = 30f;
        private static float thresh_foodProductionInsufficent = 20f;

        private static int sampleDataSignificanceThreshold = 3;


        public static string GetInsightForYear(YearData data)
        {
            string text = "";

            float overallAvgFoodConsumption = Analyzer.CalcAverageFoodConsumptionPerPerson(DataContainer.GetAllYearData());
            float percentChangeFromOverall = CalculatePercentChange(overallAvgFoodConsumption, data.AvgFoodConsumptionPerPerson);

            float predictedFoodProductionCurrPop = Analyzer.GetPredictedFoodInsufficiencyForPeople(Analytics.GetPlayerKingdomPopulation(), DataContainer.GetAllYearData());
            float predictedFoodProductionMaxPop = Analyzer.GetPredictedFoodInsufficiencyForPeople(Analytics.GetHousingForKingdom(), DataContainer.GetAllYearData());

            bool mention_foodConsumptionChange =
                Math.Abs(percentChangeFromOverall) > thresh_foodConsumptionRateIrregularity;
            bool mention_foodInsufficiency = data.foodConsumptionData.FoodInsufficiency > thresh_foodInsufficiency;
            bool mention_productionInsufficentCurrPop = predictedFoodProductionCurrPop > thresh_foodProductionInsufficent;
            bool mention_productionInsufficientMaxPop = predictedFoodProductionMaxPop > thresh_foodProductionInsufficent;


            if (DataContainer.GetYearDataCount() >= sampleDataSignificanceThreshold)
            {
                if (mention_foodConsumptionChange)
                {
                    text += "This year people ate " +
                        Stringify(data.AvgFoodConsumptionPerPerson) +
                        " food per person. " +
                        Environment.NewLine;
                    text += "People usually eat " +
                        Stringify(overallAvgFoodConsumption) +
                        " food per person. " +
                        Environment.NewLine;

                    text += "People were " +
                         Stringify(Math.Abs(percentChangeFromOverall * 100)) + "%";
                    text += (percentChangeFromOverall > 0) ? " more " : " less "; ;
                    text += "hungry than normal this year. It should be noted as irregular. " + Environment.NewLine;
                }

                if (mention_foodInsufficiency)
                {
                    if (!mention_foodConsumptionChange)
                    {
                        text += "This year, our food stores didn't contain enough to feed everyone, we needed " + data.foodConsumptionData.FoodInsufficiency + " more food. Consider increasing food production. ";
                    }
                    else
                    {
                        if(percentChangeFromOverall > 0)
                        {
                            text += "This year we did not have enough food for all the peasants. Food demand rose considerably, which could have attributed to our lack of food reserve. ";
                        }
                        else
                        {
                            text += "This year we were blessed with small bellied peasants, however our food storage still proved insubstantial, our food situation is dire!";
                        }
                    }
                    text += Environment.NewLine;
                }

                if (mention_productionInsufficentCurrPop)
                {
                    text += "We are losing too much food each year! Consider more food production. " + Environment.NewLine;
                }

                if (mention_productionInsufficientMaxPop)
                {
                    if (!mention_productionInsufficentCurrPop)
                    {
                        text += "While food production is currently enough to feed all, our city contains beds for more than we can feed!" + Environment.NewLine;
                    }
                    else
                    {
                        text += "Our city's food production will be too weak to sustain food when the city is full! " + Environment.NewLine;
                    }
                }

                if(text == "")
                {
                    text += "This year was uneventful, lord" + Environment.NewLine;
                }
            }
            else
            {
                text = "We have not yet collected enough sample data to give an accurate insight. " + Environment.NewLine;
            }

            return text;
        }

        #region Stat Explanations

        public static string exp_FoodInsufficiency(YearData data)
        {
            return "Food insufficiency this year: " +
                Analyzer.GetRequiredFoodForYear(data).ToString() +
                Environment.NewLine;
        }

        public static string exp_FoodProductionCurrent()
        {
            bool enoughData = DataContainer.GetYearDataCount() > sampleDataSignificanceThreshold;
            return enoughData ? ("Predicted food production with " +
                Analytics.GetPlayerKingdomPopulation().ToString() +
                " people: " + 
                Environment.NewLine +
                (-Analyzer.GetPredictedFoodInsufficiencyForPeople(Analytics.GetPlayerKingdomPopulation(), DataContainer.GetAllYearData())).ToString() +
                Environment.NewLine):
                "Not enough sample data to predict food consumption";
        }


        public static string exp_FoodProductionMax()
        {
            bool enoughData = DataContainer.GetYearDataCount() > sampleDataSignificanceThreshold;
            return enoughData ? ("Predicted food production with " +
                Analytics.GetHousingForKingdom().ToString() +
                " people: " +
                Environment.NewLine +
                (-Analyzer.GetPredictedFoodInsufficiencyForPeople(Analytics.GetHousingForKingdom(), DataContainer.GetAllYearData())).ToString() +
                Environment.NewLine):
                "Not enough sample data to predict food consumption";
        }

        #endregion

        public static float CalculatePercentChange(float from, float to)
        {
            return (to - from) / from;
        }

        private static string Stringify(float val)
        {
            return Utils.Util.RoundToFactor(val, 0.1f).ToString();
        }
    }
}
