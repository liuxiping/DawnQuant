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
    /// 市销率
    /// </summary>
    public class PSFactorExecutor : IFactorExecutor
    {

        private readonly Func<string, IStockDailyIndicatorRepository> _sdirFunc;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;
        private readonly IBasicStockInfoRepository _bsinfoRepository;
        private readonly ITradingCalendarRepository _tcRepository;

        public PSFactorExecutor(Func<string, IStockDailyIndicatorRepository> sdirFunc,
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
            PSFactorExecutorParameter p = (PSFactorExecutorParameter)Parameter;
            List<string> meet = new List<string>();
            DateTime adt = DateTime.Now.Date;
       
            if (tsCodes != null && tsCodes.Count > 0)
            {
                foreach (string tscode in tsCodes)
                {
                    IStockDailyIndicatorRepository repository = _sdirFunc(tscode);

                    var indicator = repository.Entities.OrderByDescending(p => p.TradeDate)
                      .FirstOrDefault();

                    if (indicator != null)
                    {
                        if ((indicator.PS >= p.PSMin && indicator.PS <= p.PSMax) &&
                            (indicator.PSTTM >= p.PSTTMMin && indicator.PSTTM <= p.PSTTMMax))
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
