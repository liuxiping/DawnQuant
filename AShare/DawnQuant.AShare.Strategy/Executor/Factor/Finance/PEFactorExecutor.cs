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
    /// 市盈率
    /// </summary>
    public class PEFactorExecutor : IFactorExecutor
    {


        private readonly Func<string, IStockDailyIndicatorRepository> _sdirFunc;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;
        private readonly IBasicStockInfoRepository _bsinfoRepository;
        private readonly ITradingCalendarRepository _tcRepository;

        public PEFactorExecutor(Func<string, IStockDailyIndicatorRepository> sdirFunc,
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
            PEFactorExecutorParameter p = (PEFactorExecutorParameter)Parameter;
            List<string> meet = new List<string>();
            if (tsCodes != null && tsCodes.Count > 0)
            {
                foreach (string tscode in tsCodes)
                {
                    IStockDailyIndicatorRepository repository = _sdirFunc(tscode);

                    var indicator = repository.Entities.OrderByDescending(p => p.TradeDate)
                      .FirstOrDefault();

                    if (indicator != null)
                    {
                        if ((indicator.PE >= p.PEMin && indicator.PE <= p.PEMax) &&
                            (indicator.PETTM >= p.PETTMMin && indicator.PETTM <= p.PETTMMax))
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
