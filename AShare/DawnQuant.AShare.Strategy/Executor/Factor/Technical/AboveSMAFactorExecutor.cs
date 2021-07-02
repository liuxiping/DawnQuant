using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Indicators;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DawnQuant.AShare.Strategy.Executor.Factor.AboveSMAFactorExecutorParameter;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 运行在均线之上
    /// </summary>
    public class AboveSMAFactorExecutor : IFactorExecutor
    {
        private readonly ITradingCalendarRepository _tradingCalendar;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;

        public AboveSMAFactorExecutor(ITradingCalendarRepository tradingCalendar,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _tradingCalendar = tradingCalendar;
            _stdrFunc = stdrFunc;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            List<string> rtsCodes = new List<string>();
          
            AboveSMAFactorExecutorParameter p = (AboveSMAFactorExecutorParameter)Parameter;

            foreach(var tsCode in tsCodes)
            {

              if(  IsMeet(p,tsCode))
                {
                    rtsCodes.Add(tsCode);
                }
            }

            return rtsCodes;
            
        }

        private bool IsMeet(AboveSMAFactorExecutorParameter p ,string tsCode)
        {
            bool r = true;

            foreach(var sp in p.AboveSMAFactors)
            {
                if(!SingleAboveSMAMeet(sp,tsCode))
                {
                    r = false;
                    break;
                }
            }
            return r;
        }


        /// <summary>
        /// 单条均线
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="tsCode"></param>
        /// <returns></returns>
        private  bool SingleAboveSMAMeet(SingleAboveSMAFactorParameter sp, string tsCode)
        {
            bool r = true;
            List<StockTradeData> datas = new List<StockTradeData>();
            if (sp.KCycle==  KCycle.Day)
            {
                int size = sp.SMACycle + sp.SMACycleCount;
                var tdr = _stdrFunc(tsCode, KCycle.Day);
                datas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(size).ToList();
                //正序排列
                datas.Reverse();
                AdjustCalculator.CalculatePrePrice(datas);
            }
            else if (sp.KCycle == KCycle.Week)
            {
                int size = (sp.SMACycle + sp.SMACycleCount+2) * 5;
                var tdr = _stdrFunc(tsCode, KCycle.Day);
                var tempdatas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(size).ToList();

                AdjustCalculator.CalculatePrePrice(tempdatas);

                var temp = ResampleBasedOnDailyData.ToWeekCycle(tempdatas);
            }
            else
            {
                throw new NotSupportedException("只支持 日线 周线 ");
            }

            if (datas.Count < sp.SMACycleCount)
            {
                //数据不够
                r = false;
            }
            else
            {
                //计算均线
                var sma = TechnicalIndicator.SMA(datas, sp.SMACycle);
                var tempdata = datas.Skip(datas.Count - sp.SMACycleCount).Take(sp.SMACycleCount).ToList();
                var tempsma=sma.Skip(sma.Count - sp.SMACycleCount).Take(sp.SMACycleCount).ToList();

                //判断股价时候在均线之上运行
               for(int i=0;i<tempdata.Count(); i++)
                {
                    if((tempsma[i]> tempdata[i].Close) || tempsma[i]==0)
                    {
                        r = false;
                        break;
                    }
                    
                }
            }
            return r;
        }
    }

}
