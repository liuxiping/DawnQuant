using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;

using DawnQuant.AShare.Strategy.Utils;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 市值因子
    /// </summary>
    public class MarketValueFactorExecutor : IFactorExecutor
    {


        private readonly Func<string, IStockDailyIndicatorRepository>  _sdirFunc;
        private readonly Func<string,KCycle , IStockTradeDataRepository> _stdrFunc;
        private readonly IBasicStockInfoRepository _bsinfoRepository;
        private readonly ITradingCalendarRepository _tcRepository;

        public MarketValueFactorExecutor(Func<string, IStockDailyIndicatorRepository> sdirFunc,
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
            MarketValueFactorExecutorParameter p = (MarketValueFactorExecutorParameter)Parameter;
            List<string> meet = new List<string>();

            if (tsCodes != null && tsCodes.Count > 0)
            {
                foreach (string tscode in tsCodes)
                {
                    IStockDailyIndicatorRepository repository = _sdirFunc(tscode);

                    //最新市值
                    var indicator = repository.Entities.OrderByDescending(p => p.TradeDate)
                      .FirstOrDefault();

                    if (indicator != null)
                    {

                        if (indicator.TotalMarketValue / 10000 <= p.MaxMarketValue
                            && indicator.TotalMarketValue / 10000 >= p.MinMarketValue)
                        {
                            meet.Add(tscode);
                        }
                    }
                }
            }
            return meet;

        }
    }
}
