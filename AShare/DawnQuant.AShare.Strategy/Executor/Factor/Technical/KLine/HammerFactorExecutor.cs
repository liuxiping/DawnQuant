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
    /// 锤子线  
    /// </summary>
    public class HammerFactorExecutor : IFactorExecutor
    {

        private readonly ITradingCalendarRepository _tradingCalendar;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;

        public HammerFactorExecutor(ITradingCalendarRepository tradingCalendar,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _tradingCalendar = tradingCalendar;
            _stdrFunc = stdrFunc;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            List<string> rtsCodes = new List<string>();
            HammerFactorExecutorParameter p = (HammerFactorExecutorParameter)Parameter;

            //日线
            if (p.KCycle == KCycle.Day)
            {
                foreach (var tsCode in tsCodes)
                {
                    var tdr = _stdrFunc(tsCode, KCycle.Day);
                    var datas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(p.LookBackCycleCount).ToList();
                    datas.Reverse();
                    if (datas!=null && datas.Count>0)
                    {
                        if( IsHammer(datas,p))
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
                    var tdr = _stdrFunc(tsCode, KCycle.Day);
                    var ddatas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take((p.LookBackCycleCount+1)*5).ToList();
                   
                   var wdatas= ResampleBasedOnDailyData.ToWeekCycle(ddatas).Take(p.LookBackCycleCount).ToList(); 
                    if (wdatas != null && wdatas.Count > 0)
                    {
                        if (IsHammer(wdatas, p))
                        {
                            rtsCodes.Add(tsCode);
                        }
                    }

                }
            }
            return rtsCodes;
        }

        private bool IsHammer(List<StockTradeData> datas, HammerFactorExecutorParameter parameter)
        {
            bool r = false;

            int[] outInteger = new int[datas.Count];
            if (TALib.Core.CdlHammer(datas.Select(p => p.Open).ToArray(),
                 datas.Select(p => p.High).ToArray(),
                 datas.Select(p => p.Low).ToArray(),
                 datas.Select(p => p.Close).ToArray(),
                 0, datas.Count - 1, outInteger, out int begidx, out int element) == TALib.Core.RetCode.Success)
            {

                if (element> 0)
                {
                    var outres = new List<int>(datas.Count);

                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (i < begidx)
                        {
                            outres.Add(0);
                        }
                        else
                        {
                            outres.Add(outInteger[i - begidx]);
                        }

                    }

                    if (outres.Where(p => p == 100).Count() >= parameter.MeetCount)
                    {
                        if (parameter.IsLatestTradeDateTimeMeet)
                        {
                            r = outres.Last() == 100;
                        }
                        else
                        {
                            r = true;
                        }

                    }
                }
            }
            return r;
        }
    }
}
