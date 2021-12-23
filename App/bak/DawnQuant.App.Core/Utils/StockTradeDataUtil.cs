using DawnQuant.App.Core.Models.AShare.EssentialData;
using System.Globalization;

namespace DawnQuant.App.Core.Utils
{
   public class StockTradeDataUtil
    {
        public static string GetStockTradeDataFileName(string tscode, KCycle kCycle)
        {
            //日线
            if (kCycle == KCycle.Day)
            {
                string dailyTDPath = Path.Combine(Environment.CurrentDirectory, "Data\\DailyTD\\");

                //确保目录已经创建

                if (!Directory.Exists(dailyTDPath))
                {
                    Directory.CreateDirectory(dailyTDPath);
                }


                return Path.Combine(dailyTDPath, tscode + "_" + kCycle.ToString() + ".bin");
            }
            else
            {
                throw new NotSupportedException("只支持日线和五分钟数据");
            }
        }


        /// <summary>
        /// 转换为周线
        /// </summary>
        /// <param name="tradeDatas"></param>
        /// <returns></returns>
        public static List<StockTradeData> ToWeekCycle(List<StockTradeData> tradeDatas)
        {
            var tmpData = tradeDatas.OrderBy(p => p.TradeDateTime);
            //日线转周线
            var weekData = tmpData.GroupBy(p => new
            {
                Year = p.TradeDateTime.Year,
                Week = GetWeekNumOfTheYear(p.TradeDateTime)
            }).Select(p => new StockTradeData
            {
                Open = p.First().Open,
                Close = p.Last().Close,
                High = p.Max(i => i.High),
                Low = p.Min(i => i.Low),
                Volume = p.Sum(i => i.Volume),
                Amount = p.Sum(i => i.Amount),
                TradeDateTime = p.Last().TradeDateTime
            }).ToList();

            //计算前收盘价格
            for (int i = 0; i < weekData.Count() - 1; i++)
            {
                weekData[i + 1].PreClose = weekData[i].Close;
            }

            return weekData;

        }


        /// <summary>
        /// 转换为月线
        /// </summary>
        /// <param name="tradeDatas"></param>
        /// <returns></returns>
        public static List<StockTradeData> ToMonthCycle(List<StockTradeData> tradeDatas)
        {
            var tmpData = tradeDatas.OrderBy(p => p.TradeDateTime);
            //日线转月线
            var monthData = tmpData.GroupBy(p => new
            {
                Year = p.TradeDateTime.Year,
                Month = p.TradeDateTime.Month
            }).Select(p => new StockTradeData
            {
                Open = p.First().Open,
                Close = p.Last().Close,
                High = p.Max(i => i.High),
                Low = p.Min(i => i.Low),
                Volume = p.Sum(i => i.Volume),
                Amount = p.Sum(i => i.Amount),
                TradeDateTime = p.Last().TradeDateTime
            }).ToList();

            for (int i = 0; i < monthData.Count() - 1; i++)
            {
                monthData[i + 1].PreClose = monthData[i].Close;
            }
            return monthData;
        }

        /// <summary>
        /// 获取一年中第几周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static int GetWeekNumOfTheYear(DateTime date)
        {
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            Calendar myCal = cultureInfo.Calendar;
            CalendarWeekRule calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            int weekOfYear = myCal.GetWeekOfYear(date, calendarWeekRule, dayOfWeek);
            return weekOfYear;
        }

    }
}
