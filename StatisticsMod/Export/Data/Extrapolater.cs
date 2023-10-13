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
                            text += "This year we were blessed with small-bellied peasants, however our food storage still proved insubstantial, our food situation is dire!";
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

        public static int FoodConsumptionCurrent() => Analyzer.GetEstimatedRequiredFoodForPeople(Analytics.GetPlayerKingdomPopulation(), DataContainer.GetAllYearData());

        public static string exp_FoodConsumptionCurrent()
        {
            bool enoughData = DataContainer.GetYearDataCount() > sampleDataSignificanceThreshold;
            return enoughData ? "Predicted food consumption with <color=yellow>" + 
                Analytics.GetPlayerKingdomPopulation().ToString() +
                "</color> people: " +
                Environment.NewLine + (FoodProductionCurrent() > FoodConsumptionCurrent() ? "<color=green>" : "<color=yellow>") + 
                Analyzer.GetEstimatedRequiredFoodForPeople(Analytics.GetPlayerKingdomPopulation(), DataContainer.GetAllYearData()).ToString() + "</color>" + 
                Environment.NewLine :
                $"Not enough sample data to predict food consumption (<color=yellow>{Analytics.GetPlayerKingdomPopulation()}</color> people)" + Environment.NewLine; 
        }

        public static int FoodProductionCurrent() => (-Analyzer.GetPredictedFoodInsufficiencyForPeople(Analytics.GetPlayerKingdomPopulation(), DataContainer.GetAllYearData()));

        public static string exp_FoodProductionCurrent()
        {
            bool enoughData = true;
            return enoughData ? ("Estimated food production: " +
                Environment.NewLine + (FoodProductionCurrent() > FoodConsumptionCurrent() ? "<color=green>" : "<color=yellow>") +
                (Analyzer.GetFoodProduction()).ToString() + "</color>" + 
                Environment.NewLine + (Analytics.FoodProductionPollutedByFishingHuts() ? $"<color=red>Figure does not include fishing huts due to irregular yields</color>" + Environment.NewLine : "")
                ) :
                $"Not enough sample data to predict food production" + Environment.NewLine;
        }


        public static string exp_FoodConsumptionMax()
        {
            bool enoughData = DataContainer.GetYearDataCount() > sampleDataSignificanceThreshold;
            return enoughData ? "Predicted food consumption with <color=yellow>" +
                Analytics.GetHousingForKingdom().ToString() +
                "</color> people: " +
                Environment.NewLine + (FoodProductionCurrent() > FoodConsumptionCurrent() ? "<color=green>" : "<color=yellow>") +
                Analyzer.GetEstimatedRequiredFoodForPeople(Analytics.GetHousingForKingdom(), DataContainer.GetAllYearData()).ToString() +
                Environment.NewLine :
                $"Not enough sample data to predict food consumption (<color=yellow>{Analytics.GetHousingForKingdom()}</color> people)" + Environment.NewLine;
        }


        public static string exp_FoodProductionMax()
        {
            bool enoughData = DataContainer.GetYearDataCount() > sampleDataSignificanceThreshold;
            return enoughData ? ("Predicted food production with " +
                Analytics.GetHousingForKingdom().ToString() +
                " people: " +
                Environment.NewLine +
                (Analyzer.GetFoodProduction()).ToString() +
                Environment.NewLine+ (Analytics.FoodProductionPollutedByFishingHuts() ? $"<color=red>Figure does not include fishing huts due to irregular yields</color>" + Environment.NewLine : "")):
                $"Not enough sample data to predict food production (<color=yellow>{Analytics.GetHousingForKingdom()}</color> people)" + Environment.NewLine;
        }

        #endregion

        public static float CalculatePercentChange(float from, float to)
        {
            return (to - from) / from;
        }

        private static string Stringify(float val)
        {
            return Util.RoundToFactor(val, 0.1f).ToString();
        }
    }
}
