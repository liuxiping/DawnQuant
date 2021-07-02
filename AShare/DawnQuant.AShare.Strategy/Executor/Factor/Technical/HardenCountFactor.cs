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
    /// 涨停次数
    /// </summary>
    public class HardenCountFactor : IFactorExecutor
    {
        private readonly ITradingCalendarRepository _tradingCalendar;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;

        public HardenCountFactor(ITradingCalendarRepository tradingCalendar,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _tradingCalendar = tradingCalendar;
            _stdrFunc = stdrFunc;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            throw new NotImplementedException();
        }
    }
}
