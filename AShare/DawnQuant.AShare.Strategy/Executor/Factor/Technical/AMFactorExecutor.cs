using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 振幅选择器，最近交易日期
    /// </summary>
    public class AMFactorExecutor : IFactorExecutor
    {
        private readonly ITradingCalendarRepository _tradingCalendar;
        private readonly Func<string, KCycle, IStockTradeDataRepository>_stdrFunc;

        public AMFactorExecutor(ITradingCalendarRepository tradingCalendar,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _tradingCalendar = tradingCalendar;
            _stdrFunc = stdrFunc;
        }

        public object Parameter { get; set; }
        public List<string> Execute(List<string> tsCodes)
        {
            List<string> rtsCodes = new List<string>();
            List<StockTradeData> datas = new List<StockTradeData>();
            AMFactorExecutorParameter p = (AMFactorExecutorParameter)Parameter;

            //日线
            if (p.KCycle ==  KCycle.Day)
            {
                foreach (var tsCode in tsCodes)
                {
                    var tdr = _stdrFunc(tsCode, KCycle.Day);
                    datas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(p.LookBackCycleCount).ToList();

                    //正序排列
                    datas.Reverse();


                    AdjustCalculator.CalculatePrePrice(datas);

                    if (datas.Count == p.LookBackCycleCount)
                    {
                        if (IsMeet(p, datas))
                        {
                            rtsCodes.Add(tsCode);
                        }
                    }
                }
            }
            //周线
            else if (p.KCycle ==KCycle.Week)
            {

                foreach (var tsCode in tsCodes)
                {
                    int size = (p.LookBackCycleCount + 2) * 5;
                    var tdr = _stdrFunc(tsCode, KCycle.Day);
                    var tempdatas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(size).ToList();

                    AdjustCalculator.CalculatePrePrice(tempdatas);

                    var temp = ResampleBasedOnDailyData.ToWeekCycle(tempdatas);
                    if (temp.Count >= p.LookBackCycleCount)
                    {
                        datas = temp.Skip(temp.Count - p.LookBackCycleCount)
                            .Take(p.LookBackCycleCount).ToList();

                        if (IsMeet(p, datas))
                        {
                            rtsCodes.Add(tsCode);
                        }
                    }

                }
            }
            else
            {
                throw new NotSupportedException("只支持日线 周线 ");
            }

            return rtsCodes;
        }


        private bool IsMeet(AMFactorExecutorParameter p ,List<StockTradeData> datas)
        {
            //计算振幅
            double pre = datas[0].PreClose;
            double max = datas.Max(p => p.High);
            double min = datas.Min(p => p.Low);
            double am = (max - min) / pre;
            if (am*100 >= p.MinAM && am <= p.MaxAM)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
