using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Indicators;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 均线多头排序
    /// </summary>
    public class SMABullFactorExecutor:IFactorExecutor
    {
        private readonly Func<string, KCycle, IStockTradeDataRepository>
           _stdrFunc;
        private readonly ILogger<SMABullFactorExecutor> _logger;

        public SMABullFactorExecutor(Func<string, KCycle, IStockTradeDataRepository> stdrFunc,
            ILogger<SMABullFactorExecutor> logger)
        {
            _stdrFunc = stdrFunc;
            _logger = logger;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {

            SMABullFactorExecutorParameter p = (SMABullFactorExecutorParameter)Parameter;

            if (!(p.ThreeSMACycle > p.SecondSMACycle && p.SecondSMACycle > p.FirstSMACycle))
            {
                throw new Exception("三条均线参数设置错误");
            }
            List<string> rtsCodes = new List<string>();

            if (tsCodes != null && tsCodes.Count > 0)
            {
                foreach (string tscode in tsCodes)
                {
                    var r = _stdrFunc(tscode, KCycle.Day);

                    List<StockTradeData> datas = null;

                    if (p.KCycle == KCycle.Day)
                    {
                        int size = p.ThreeSMACycle + p.KCycleCount;
                        datas = r.Entities.OrderByDescending(p => p.TradeDateTime).Take(size).ToList();
                        datas.Reverse();

                        AdjustCalculator.CalculatePrePrice(datas);
                    }
                    else if (p.KCycle == KCycle.Week)
                    {
                        int size = (p.ThreeSMACycle + p.KCycleCount) * 5;
                        var temp = r.Entities.OrderByDescending(p => p.TradeDateTime).Take(size).ToList();

                        AdjustCalculator.CalculatePrePrice(temp);

                        datas = ResampleBasedOnDailyData.ToWeekCycle(temp);
                    }
                    else
                    {
                        throw new NotSupportedException("只支持 日线 周线");
                    }

                    //计算均线
                    var first = TechnicalIndicator.SMA(datas, p.FirstSMACycle);
                    var second = TechnicalIndicator.SMA(datas, p.SecondSMACycle);
                    var three = TechnicalIndicator.SMA(datas, p.ThreeSMACycle);

                    int firstLastIndex = first.Count - 1;
                    int secondLastIndex = second.Count - 1;
                    int threeLastIndex = three.Count - 1;

                    bool isMeet = true;

                    //数据要足
                    if(first.Count>=p.KCycleCount && second.Count>=p.KCycleCount && three.Count>=p.KCycleCount)
                    {
                        for (int i = 0; i < p.KCycleCount; i++)
                        {
                            // try
                            {
                                if (!(first[firstLastIndex - i] >= second[secondLastIndex - i] &&
                                    second[secondLastIndex - i] >= three[threeLastIndex - i]))
                                {
                                    isMeet = false;
                                    break;
                                }
                            }
                            //catch(Exception e)
                            //{
                            //    _logger.LogError(e.Message);
                            //    _logger.LogError(e.StackTrace);
                            //}

                        }
                        if (isMeet)
                        {
                            rtsCodes.Add(tscode);
                        }
                    }

                   
                }
            }

            return rtsCodes;
        }
    }
}
