using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMod.Data
{
    public static class DataContainer
    {
        #region Yearly Data

        private static List<YearData> data = new List<YearData>();

        public static YearData currentYearData;

        #endregion

        public static YearData GetLastYearData()
        {
            return data[data.Count - 1];
        }

        public static YearData GetYearData(int yearIdx)
        {
            bool valid = yearIdx > 0 && yearIdx < data.Count - 1;
            return valid ? data[yearIdx] : YearData.Empty;
        }

        public static int GetYearDataCount()
        {
            return data.Count;
        }

        public static List<YearData> GetAllYearData()
        {
            List<YearData> list = new List<YearData>();
            foreach(YearData yData in data)
            {
                list.Add(yData);
            }
            return list;
        }

        public static void OnYearEnd()
        {
            currentYearData.population = Analytics.GetPlayerKingdomPopulation();
            currentYearData.populationEatingTime = Analytics.GetPlayerKingdomEatingPopulationTime();

            //DebugExt.Log($"End of year, player pop: {currentYearData.population}, player pop eating time: {currentYearData.populationEatingTime}");

            data.Add(currentYearData);

            currentYearData = YearData.Empty;
        }

        public class DataContainerSaveData
        {
            public List<YearData> data;

            public YearData currentYearData;
        }



    }
}
