
using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
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


        /// <summary>
        /// MACD
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="optInFastPeriod"></param>
        /// <param name="optInSlowPeriod"></param>
        /// <param name="optInSignalPeriod"></param>
        /// <returns></returns>
        public static (double[] Macd, double[] MacdSignal, double[] MacdHist) MACD(IEnumerable<StockTradeData> datas, 
            int optInFastPeriod = 12, int optInSlowPeriod = 26, int optInSignalPeriod = 9)
        {

            int count=datas.Count();

            int extraSize = 50;
            int extraCount = count + extraSize;

            var extraData = datas.OrderBy(p => p.TradeDateTime).Select(p => p.Close).ToList();
            var first= extraData.First();
            for(int i = 0;i < extraSize; i++)
            {
                extraData.Insert(0, first);
            }


            double[] inReal= extraData.ToArray();

            double[] outMacd = new double[extraCount];
            double[] outMacdSignal = new double[extraCount];
            double[] outMacdHist = new double[extraCount];


            (List<double> Macd, List<double> MacdSignal, List<double> MacdHist) macd = new( new List<double>(), new List<double>(), new List<double>());


            var r = TALib.Core.Macd(inReal, 0, inReal.Length-1, outMacd, outMacdSignal, outMacdHist, 
                out int outBegIdx, out int outNbElement, 
                optInFastPeriod, optInSlowPeriod, optInSignalPeriod);

            if (r == TALib.Core.RetCode.Success)
            {
                if (outNbElement > 0)
                {
                    for (int i = 0; i < extraCount; i++)
                    {
                        //没有数据
                        if (i < outBegIdx)
                        {
                            macd.Macd.Add(double.NaN);
                            macd.MacdSignal.Add(double.NaN);
                            macd.MacdHist.Add(double.NaN);

                        }
                        else
                        {
                            macd.Macd.Add(outMacd[i - outBegIdx]);
                            macd.MacdSignal.Add(outMacdSignal[i - outBegIdx]);
                            macd.MacdHist.Add(outMacdHist[i - outBegIdx] * 2);

                        }
                      
                    }

                }
                else
                {
                    for (int i = 0; i < datas.Count(); i++)
                    {
                        macd.Macd[i] = double.NaN;
                        macd.MacdSignal[i] = double.NaN;
                        macd.MacdSignal[i] = double.NaN;
                    }
                }
            }



            var reMacd=macd.Macd.Skip(extraSize).Take(count).ToArray();
            var reMacdSignal = macd.MacdSignal.Skip(extraSize).Take(count).ToArray();
            var reMacdHist = macd.MacdHist.Skip(extraSize).Take(count).ToArray();

            (double[] Macd, double[] MacdSignal, double[] MacdHist) rMacd = 
                new(reMacd, reMacdSignal, reMacdHist);

            return rMacd;
        }
    }
}
