using DawnQuant.App.Core.Models.AShare.EssentialData;

namespace DawnQuant.App.Core.Utils
{
    public static class TechnicalIndicatorUtil
    {
        /// <summary>
        /// 计算简单均线
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="timePeriod"></param>
        /// <returns></returns>
        public static List<double> SMA(IEnumerable<StockTradeData> datas, int timePeriod)
        {

            List<double> ma = new List<double>();
            double[] inReal = datas.OrderBy(p=>p.TradeDateTime).Select(p => p.Close).ToArray();
            double[] outReal = new double[inReal.Length];
            var r = TALib.Core.Sma(inReal, 0, inReal.Length - 1, outReal, out int outBegIdx, out int outNbElement, timePeriod);
            if (r == TALib.Core.RetCode.Success)
            {
                int i = 0;
                foreach (var d in datas)
                {
                    //没有数据
                    if (i < outBegIdx)
                    {
                        ma.Add(double.NaN);

                    }
                    else
                    {
                        ma.Add(outReal[i - outBegIdx]);
                    }
                    i++;
                }

            }
            return ma;
        }
    }
}
