using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Indicators;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Utils;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 形态选股
    /// </summary>
    public class CorrelationFactorExecutor : IFactorExecutor
    {
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;

        public CorrelationFactorExecutor( Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _stdrFunc = stdrFunc;
        }

        public object Parameter { get; set; }


        public List<string> Execute(List<string> tsCodes)
        {

            List<string> r = new List<string>();
            CorrelationFactorExecutorParameter p = (CorrelationFactorExecutorParameter)Parameter;
            if (p.KCycle == KCycle.Day)
            {
                if (!string.IsNullOrEmpty(p.TSCode))
                {
                    r.AddRange(CalculateOnDailyData(p, tsCodes));
                }

            }
            else if (p.KCycle == KCycle.Week)
            {
                r.AddRange(CalculateOnWeekData(p, tsCodes));
            }
            else 
            {
                throw new NotSupportedException("只支持日线 周线 ");
            }
            return r;
        }


        /// <summary>
        /// 日线
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        private List<string> CalculateOnDailyData(CorrelationFactorExecutorParameter p, List<string> tsCodes)
        {
            List<string> r = new List<string>();

            //获取匹配目标数据 
            var stdr = _stdrFunc(p.TSCode, KCycle.Day);

            //匹配的K线数量
            int count = stdr.Entities.Where(pt => pt.TradeDateTime >= p.Start && pt.TradeDateTime <= p.End).OrderBy(pt => pt.TradeDateTime).Count();

            //需要获取的数据大小
            int dataCount = count;
            if (p.IsCalculateSMA)
            {
                dataCount = count + p.SMACycleCount;
            }
            

            //获取数据
            var tempdatas = stdr.Entities.Where(pt=>pt.TradeDateTime<=p.End)
                .OrderByDescending(pt=>pt.TradeDateTime).Take(dataCount).ToList();

            //正序排列
            tempdatas = tempdatas.OrderBy(pt => pt.TradeDateTime).ToList();

            //前复权
            AdjustCalculator.CalculatePrePrice(tempdatas);

            List<double> tcloseDatas = null;
            List<double> tmaSMADatas = null;
            List<double> tvolDatas = null;


            //计算收盘价
            if (p.IsCalculateClosePrice)
            {
                tcloseDatas = tempdatas.Where(pt => pt.TradeDateTime >= p.Start && pt.TradeDateTime <= p.End)
                    .OrderBy(pt => pt.TradeDateTime).Select(pt => pt.Close).ToList();
            }
            //计算均线
            if (p.IsCalculateSMA)
            {
                //计算均线
                var ma5 = TechnicalIndicator.SMA(tempdatas, p.SMACycleCount);

                tmaSMADatas = ma5.Skip(ma5.Count - count).Take(count).ToList();
            }
            //成交量
            if (p.IsCalculateVol)
            {
                tvolDatas = tempdatas.Where(pt => pt.TradeDateTime >= p.Start && pt.TradeDateTime <= p.End)
                    .OrderBy(pt => pt.TradeDateTime).Select(pt => pt.Volume).ToList();
            }

            foreach (string tsCode in tsCodes)
            {
                bool isMeet = true;

                var pstdr = _stdrFunc(tsCode, KCycle.Day);
                var ptempdatas = pstdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(dataCount).ToList();
                ptempdatas.Reverse();
                AdjustCalculator.CalculatePrePrice(ptempdatas);

                //数据不够
                if(ptempdatas.Count<count)
                {
                    isMeet = false;
                    continue;
                }

                //收盘价
                if (p.IsCalculateClosePrice)
                {
                    var closeDatas = ptempdatas.Skip(ptempdatas.Count-count).Take(count).Select(pt => pt.Close).ToList();

                    var c = Correlation.Pearson(tcloseDatas, closeDatas);
                    if (c < p.ClosePriceCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //5日均线
                if (p.IsCalculateSMA)
                {
                    var ma = TechnicalIndicator.SMA(ptempdatas, p.SMACycleCount);
                    var maDatas = ma.Skip(ma.Count-count).Take(count).ToList();

                    var c = Correlation.Pearson(tmaSMADatas, maDatas);
                    if (c < p.SMACL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //成交量
                if (p.IsCalculateVol)
                {
                    var volDatas = ptempdatas.Skip(ptempdatas.Count - count).Take(count).Select(pt => pt.Volume).ToList();

                    var c = Correlation.Pearson(tvolDatas, volDatas);

                    if (c < p.VolCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }

                if (isMeet)
                {
                    r.Add(tsCode);
                }

            }

            return r;
        }



        /// <summary>
        /// 周线
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        private List<string> CalculateOnWeekData(CorrelationFactorExecutorParameter p, List<string> tsCodes)
        {
            List<string> r = new List<string>();

            //获取匹配目标数据 
            var stdr = _stdrFunc(p.TSCode, KCycle.Day);
           

            //开始日期周的开始与结束
            p.Start.GetWeekSpan(out DateTime sStart, out DateTime sEnd);
            p.End.GetWeekSpan(out DateTime eStart, out DateTime eEnd);

            //读取日线数据
            var tdailydatas = stdr.Entities.Where(pt => pt.TradeDateTime >= sStart && pt.TradeDateTime <= eEnd).OrderBy(pt => pt.TradeDateTime).ToList();
            //转换成周线数据
            var tweekdatas = ResampleBasedOnDailyData.ToWeekCycle(tdailydatas);
            //K线周期数目
            int wcount = tweekdatas.Count;

            int wdataCount = wcount;
            if(p.IsCalculateSMA)
            {
                wdataCount += p.SMACycleCount;
            }

            //需要读取的数据大小
            int actualDailyDataCount = wdataCount * 5;

            //获取数据
            var tempdatas = stdr.Entities.Where(pt => pt.TradeDateTime <= eEnd)
                .OrderByDescending(pt => pt.TradeDateTime).Take(actualDailyDataCount).ToList();
            tempdatas.Reverse();
            //前复权
            AdjustCalculator.CalculatePrePrice(tempdatas);

            ///周线数据
            var tcweekdatas = ResampleBasedOnDailyData.ToWeekCycle(tempdatas);
            List<double> tcloseDatas = null;
            List<double> tma5Datas = null;
            List<double> tvolDatas = null;

            //收盘价
            if (p.IsCalculateClosePrice)
            {
                tcloseDatas = tweekdatas.Select(pt => pt.Close)
                    .Skip(tweekdatas.Count-wcount).Take(wcount).ToList();
            }
            //均线
            if (p.IsCalculateSMA)
            {
                var ma = TechnicalIndicator.SMA(tcweekdatas, p.SMACycleCount);
                tma5Datas = ma.Skip(ma.Count - wcount).Take(wcount).ToList();
            }
            //成交量
            if (p.IsCalculateVol)
            {
                tvolDatas = tweekdatas.Select(pt => pt.Volume)
                    .Skip(tweekdatas.Count - wcount).Take(wcount).ToList();
            }

            foreach (string tsCode in tsCodes)
            {
                bool isMeet = true;
                var pstdr = _stdrFunc(tsCode, KCycle.Day);
                var ptempdatas = pstdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(actualDailyDataCount).ToList();
                ptempdatas.Reverse();

                AdjustCalculator.CalculatePrePrice(ptempdatas);

                var pweekdatas = ResampleBasedOnDailyData.ToWeekCycle(ptempdatas);

                //数据不够
                if (pweekdatas.Count < wcount)
                {
                    isMeet = false;
                    continue;
                }

                //收盘价
                if (p.IsCalculateClosePrice)
                {
                    var closeDatas = pweekdatas.Skip(pweekdatas.Count - wcount).Take(wcount).Select(pt => pt.Close).ToList();

                    var c = Correlation.Pearson(tcloseDatas, closeDatas);
                    if (c < p.ClosePriceCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //5周均线
                if (p.IsCalculateSMA)
                {
                    var ma = TechnicalIndicator.SMA(pweekdatas, p.SMACycleCount);
                    var maDatas = ma.Skip(ma.Count - wcount).Take(wcount).ToList();

                    var c = Correlation.Pearson(tma5Datas, maDatas);

                    if (c < p.ClosePriceCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //成交量
                if (p.IsCalculateVol)
                {
                    var volDatas = pweekdatas.Skip(pweekdatas.Count - wcount).Take(wcount).Select(pt => pt.Volume).ToList();

                    var c = Correlation.Pearson(tvolDatas, volDatas);

                    if (c < p.VolCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }

                if (isMeet)
                {
                    r.Add(tsCode);
                }

            }

            return r;
        }


        /// <summary>
        /// 月线
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        private List<string> CalculateOnMonthData(CorrelationFactorExecutorParameter p, List<string> tsCodes)
        {
            List<string> r = new List<string>();

            //获取匹配目标数据 
            var stdr = _stdrFunc(p.TSCode, KCycle.Day);

            //开始日期周的开始与结束
            p.Start.GetMonthSpan(out DateTime sStart, out DateTime sEnd);
            p.End.GetMonthSpan(out DateTime eStart, out DateTime eEnd);

            var tempdatas = stdr.Entities.Where(pt => pt.TradeDateTime >= sStart.AddMonths(-10) && pt.TradeDateTime <= eEnd).OrderBy(pt => pt.TradeDateTime).ToList();

            var trdatas = tempdatas.Where(pt => pt.TradeDateTime >= sStart && pt.TradeDateTime <= eEnd).OrderBy(pt => pt.TradeDateTime).ToList();

            //前复权
            AdjustCalculator.CalculatePrePrice(tempdatas);
            AdjustCalculator.CalculatePrePrice(trdatas);

            var tmonthdatas = ResampleBasedOnDailyData.ToMonthCycle(trdatas);
            int count = tmonthdatas.Count;

            List<double> tcloseDatas = null;
            List<double> tma5Datas = null;
            List<double> tvolDatas = null;

            //收盘价
            if (p.IsCalculateClosePrice)
            {
                tcloseDatas = tmonthdatas.Select(pt => pt.Close).ToList();
            }
            //5日均线
            if (p.IsCalculateSMA)
            {
                var ma5 = TechnicalIndicator.SMA(ResampleBasedOnDailyData.ToMonthCycle(tempdatas), 5);
                tma5Datas = ma5.Skip(ma5.Count - count).Take(count).ToList();
            }
            //成交量
            if (p.IsCalculateVol)
            {
                tvolDatas = tmonthdatas.Select(pt => pt.Volume).ToList();
            }

            foreach (string tsCode in tsCodes)
            {
                bool isMeet = true;
                int daycount = (count + 5) * 23;
                var pstdr = _stdrFunc(tsCode, KCycle.Day);
                var ptempdatas = pstdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(daycount).ToList();
                ptempdatas.Reverse();

                //前复权价格
                AdjustCalculator.CalculatePrePrice(ptempdatas);
                var pmonthdatas = ResampleBasedOnDailyData.ToMonthCycle(ptempdatas);


                //数据不够
                if (pmonthdatas.Count < count)
                {
                    isMeet = false;
                    continue;
                }

                //收盘价
                if (p.IsCalculateClosePrice)
                {
                    var closeDatas = pmonthdatas.Skip(pmonthdatas.Count - count).Take(count).Select(pt => pt.Close).ToList();

                    var c = Correlation.Pearson(tcloseDatas, closeDatas);
                    if (c < p.ClosePriceCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //5月均线
                if (p.IsCalculateSMA)
                {
                    var ma5 = TechnicalIndicator.SMA(pmonthdatas, 5);
                    var ma5Datas = ma5.Skip(ma5.Count - count).Take(count).ToList();

                    var c = Correlation.Pearson(tma5Datas, ma5Datas);

                    if (c < p.SMACL)
                    {
                        isMeet = false;
                        continue;
                    }
                }
                //成交量
                if (p.IsCalculateVol)
                {
                    var volDatas = pmonthdatas.Skip(pmonthdatas.Count - 5).Take(count).Select(pt => pt.Volume).ToList();

                    var c = Correlation.Pearson(tvolDatas, volDatas);

                    if (c < p.VolCL)
                    {
                        isMeet = false;
                        continue;
                    }
                }

                if (isMeet)
                {
                    r.Add(tsCode);
                }

            }

            return r;
        }



    }
}
