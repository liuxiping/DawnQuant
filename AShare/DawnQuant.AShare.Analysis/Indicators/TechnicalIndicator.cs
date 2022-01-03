
using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Analysis.Indicators
{
    public static class TechnicalIndicator
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

            double[] inReal = datas.OrderBy(p => p.TradeDateTime).Select(p => p.Close).ToArray();
            double[] outReal = new double[inReal.Length];

            var r = TALib.Core.Sma(inReal, 0, inReal.Length - 1, outReal,
                out int outBegIdx, out int outNbElement, timePeriod);

            if (r == TALib.Core.RetCode.Success)
            {
                if (outNbElement > 0)
                {
                    for (int i = 0; i < datas.Count(); i++)
                    {
                        //前面没有数据
                        if (i < outBegIdx)
                        {
                            ma.Add(double.NaN);

                        }
                        else
                        {
                            ma.Add(outReal[i - outBegIdx]);
                        }

                    }
                }
                else//数据不够没有输出
                {
                    for (int i = 0; i < datas.Count(); i++)
                    {
                        ma.Add(double.NaN);
                    }
                }
            }
            return ma;
        }
    }
}
