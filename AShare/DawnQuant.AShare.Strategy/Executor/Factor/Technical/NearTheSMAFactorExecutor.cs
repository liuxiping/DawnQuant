using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Indicators;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 股价在均线附近
    /// </summary>
    public class NearTheSMAFactorExecutor: IFactorExecutor
    {
        private readonly Func<string, IStockDailyIndicatorRepository> _sdirFunc;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;
        private readonly IBasicStockInfoRepository _bsinfoRepository;
        private readonly ITradingCalendarRepository _tcRepository;

        public NearTheSMAFactorExecutor(Func<string, IStockDailyIndicatorRepository> sdirFunc,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc,
            ITradingCalendarRepository tcRepository, IBasicStockInfoRepository bsinfoRepository)
        {
            _sdirFunc = sdirFunc;
            _stdrFunc = stdrFunc;
            _bsinfoRepository = bsinfoRepository;
            _tcRepository = tcRepository;
        }

        public object Parameter { get; set; }


        public List<string> Execute(List<string> tsCodes)
        {

            List<string> rtsCodes = new List<string>();
            NearTheSMAFactorExecutorParameter pa = (NearTheSMAFactorExecutorParameter)Parameter;
            if (tsCodes != null && tsCodes.Count > 0)
            {
                foreach (string tscode in tsCodes)
                {
                    using (var r = _stdrFunc(tscode, KCycle.Day))
                    {

                        List<StockTradeData> datas = null;

                        foreach (var p in pa.NearTheSMAFactors)
                        {
                            //日线
                            if (p.KCycle == KCycle.Day)
                            {
                               //获取最大的均线周期

                                int size = p.SMACycle;
                                datas = r.Entities.OrderByDescending(p => p.TradeDateTime).Take(size).ToList();
                                datas.Reverse();
                                AdjustCalculator.CalculatePrePrice(datas);
                                //计算均线
                            }
                            else if (p.KCycle == KCycle.Week)
                            {
                                int size = (p.SMACycle + 2) * 5;
                                var temp = r.Entities.OrderByDescending(p => p.TradeDateTime).Take(size).ToList();

                                AdjustCalculator.CalculatePrePrice(temp);

                                datas = ResampleBasedOnDailyData.ToWeekCycle(temp);

                            }
                            else
                            {
                                throw new NotSupportedException("只支持 日线 周线 ");
                            }

                            //计算均线
                            var sma = TechnicalIndicator.SMA(datas, p.SMACycle);
                            double lastsma = sma[sma.Count - 1];
                            double close = datas[datas.Count - 1].Close;

                            double max = lastsma * (1 + p.Precision / 100);
                            double min = lastsma * (1 - p.Precision / 100);

                            if (close >= min && close <= max)
                            {
                                //或的关系只要一个满足就返回
                                rtsCodes.Add(tscode);
                                break;
                            }
                        }
                    }
                }
            }

            return rtsCodes;
        }
        
    }
}
