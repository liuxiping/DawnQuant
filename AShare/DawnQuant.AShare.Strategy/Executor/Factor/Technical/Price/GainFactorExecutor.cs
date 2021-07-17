
using DawnQuant.AShare.Analysis.Common;
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
    /// 涨幅
    /// </summary>
    public class GainFactorExecutor : IFactorExecutor
    {
        private readonly ITradingCalendarRepository _tradingCalendar;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;

        public GainFactorExecutor(ITradingCalendarRepository tradingCalendar,
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
            GainFactorExecutorParameter p = (GainFactorExecutorParameter)Parameter;

            //日线
            if (p.KCycle == KCycle.Day)
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
            else if (p.KCycle == KCycle.Week)
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
                throw new NotSupportedException("只支持日线 周线");
            }

            return rtsCodes;
        }


        private bool IsMeet(GainFactorExecutorParameter p, List<StockTradeData> datas)
        {

            //计算涨幅
            double pre = datas[0].PreClose;
            double last = datas[datas.Count - 1].Close;

            double gain = ((last - pre) / pre)*100;
            if (gain >= p.MinGain && gain <= p.MaxGain)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        ///最后一个交易周期
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        //private List<string> OnlyComputeLatestCycleData(GainSelectorParameter p, List<string> tsCodes)
        //{

        //    List<string> rtsCodes = new List<string>();

        //    //最近交易日
        //    var tradedate = _tradingCalendar.Entities.Where(p => p.Date == DateTime.Now.Date &&
        //      p.Exchange == StockEssentialDataConst.SSE).FirstOrDefault();
        //    if (tradedate != null)
        //    {
        //        var opendate = tradedate.IsOpen ? tradedate.Date : tradedate.PreDate;

        //        StockTradeData data = null;
        //        foreach (var tscode in tsCodes)
        //        {
        //            //获取日线数据
        //            if (p.KCycle == StockKCycle.Day)
        //            {
        //                var tdr = _stdrFunc(tscode, StockKCycle.Day);
        //                data = tdr.Entities.Where(p => p.TradeDateTime == opendate).FirstOrDefault();
        //            }
        //            //获取周线数据
        //            else if (p.KCycle == StockKCycle.Week)
        //            {
        //                //周的开始与结束
        //                opendate.Value.GetWeekSpan(out DateTime curStart, out DateTime curEnd);
        //                var tdr = _stdrFunc(tscode, StockKCycle.Day);
        //                //取前3个月数据，
        //                var tempdata = tdr.Entities.Where(p => p.TradeDateTime >= curEnd.AddMonths(-3) && p.TradeDateTime <= curEnd).ToList();
        //                var weekdata = ResampleBaseOnDailyData.ToWeekCycle(tempdata);

        //                //取最后一条数据
        //                data = weekdata[weekdata.Count - 1];
        //            }
        //            //获取月线数据
        //            else if (p.KCycle == StockKCycle.Month)
        //            {
        //                //获取月的开始与结束
        //                opendate.Value.GetMonthSpan(out DateTime curStart, out DateTime curEnd);

        //                var tdr = _stdrFunc(tscode, StockKCycle.Day);

        //                //取前3个月数据，
        //                var tempdata = tdr.Entities.Where(p => p.TradeDateTime >= curStart.AddMonths(-3) && p.TradeDateTime <= curEnd).ToList();
        //                var weekdata = ResampleBaseOnDailyData.ToMonthCycle(tempdata);

        //                //取最后一条数据
        //                data = weekdata[weekdata.Count - 1];
        //            }
        //            else
        //            {
        //                throw new NotSupportedException("只支持日线 周线 月线");
        //            }

        //            if (data != null)
        //            {
        //                double gain = (data.Close - data.PreClose) / data.PreClose;
        //                if (gain >= p.MinGain && gain <= p.MaxGain)
        //                {
        //                    rtsCodes.Add(tscode);
        //                }
        //            }


        //        }

        //    }

        //    return rtsCodes;
        //}



        /// <summary>
        /// 区间数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        //private List<string> ComputeTimeSpanCycleData(GainSelectorParameter p, List<string> tsCodes)
        //{
        //    List<string> rtsCodes = new List<string>();
        //    //当天最后时间
        //    DateTime end = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
        //    if (p.End != null)
        //    {
        //        end = p.End.Value;
        //    }
        //    foreach (var tscode in tsCodes)
        //    {
        //        List<StockTradeData> datas = null;
        //        //获取日线数据
        //        if (p.KCycle == StockKCycle.Day)
        //        {
        //            var tdr = _stdrFunc(tscode, StockKCycle.Day);
        //            datas = tdr.Entities.Where(pt => pt.TradeDateTime >= p.Start && pt.TradeDateTime <= end).ToList();
        //        }
        //        //获取周线数据
        //        else if (p.KCycle == StockKCycle.Week)
        //        {
        //            //开始日期周的开始与结束
        //            p.Start.GetWeekSpan(out DateTime sStart, out DateTime sEnd);
        //            //结束日期周的开始与结束
        //            end.GetWeekSpan(out DateTime eStart, out DateTime eEnd);
        //            var tdr = _stdrFunc(tscode, StockKCycle.Day);

        //            //往前取三月数据转发周线

        //            var temp = tdr.Entities.Where(pt => pt.TradeDateTime >= sStart.AddMonths(-3)
        //            && pt.TradeDateTime <= eEnd).ToList();

        //            datas = ResampleBaseOnDailyData.ToWeekCycle(temp).Where(pt => pt.TradeDateTime >= sStart
        //             && pt.TradeDateTime <= eEnd).ToList();

        //        }
        //        //获取月线数据
        //        else if (p.KCycle == StockKCycle.Month)
        //        {
        //            //开始日期月开始与结束
        //            p.Start.GetMonthSpan(out DateTime sStart, out DateTime sEnd);
        //            //结束日期月的开始与结束
        //            end.GetMonthSpan(out DateTime eStart, out DateTime eEnd);
        //            var tdr = _stdrFunc(tscode, StockKCycle.Day);

        //            //往前取三月数据转发周线

        //            var temp = tdr.Entities.Where(pt => pt.TradeDateTime >= sStart.AddMonths(-3)
        //            && pt.TradeDateTime <= eEnd).ToList();

        //            datas = ResampleBaseOnDailyData.ToMonthCycle(temp).Where(pt => pt.TradeDateTime >= sStart
        //             && pt.TradeDateTime <= eEnd).ToList();

        //        }
        //        else
        //        {
        //            throw new NotSupportedException("只支持日线 周线 月线");
        //        }

        //        //计算涨幅
        //        double pre = datas[0].PreClose;
        //        double last = datas[datas.Count - 1].Close;

        //        double gain = (last - pre) / pre;
        //        if (gain >= p.MinGain && gain <= p.MaxGain)
        //        {
        //            rtsCodes.Add(tscode);
        //        }

        //    }
        //    return rtsCodes;
        //}

    }
}
