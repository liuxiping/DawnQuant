using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
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
                    var datas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(3).ToList();
                    datas.Reverse();
                    if (datas!=null && datas.Count==3)
                    {
                        if( IsHammer(datas))
                        {
                            rtsCodes.Add(tsCode);
                        }
                    }

                }
            }
            //周线
            else if (p.KCycle == KCycle.Week)
            {
            }
            return rtsCodes;
        }

        private bool IsHammer(List<StockTradeData> datas)
        {
            int[] outInteger = null;
            if (TALib.Core.CdlHammer(datas.Select(p => p.Open).ToArray(),
                 datas.Select(p => p.High).ToArray(),
                 datas.Select(p => p.Low).ToArray(),
                 datas.Select(p => p.Close).ToArray(),
                 0, 0, outInteger, out int begidx, out int element) == 0)
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
