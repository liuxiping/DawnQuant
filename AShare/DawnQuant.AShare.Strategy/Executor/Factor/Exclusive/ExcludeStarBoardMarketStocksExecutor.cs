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
    /// 排除科创板
    /// </summary>
    public class ExcludeStarBoardMarketStocksExecutor : IFactorExecutor
    {
        public object Parameter { get; set; }

      

        public ExcludeStarBoardMarketStocksExecutor(IBasicStockInfoRepository repository)
        {
            _repository = repository;
        }
        IBasicStockInfoRepository _repository;

        public List<string> Execute(List<string> tsCodes)
        {
            if (tsCodes != null && tsCodes.Count > 0)
            {
                var r = _repository.Entities.Where(p => p.MarketType == StockEssentialDataConst.StarBoard)
                  .Select(p => p.TSCode).ToList();
                return tsCodes.Except(r).ToList();
            }
            else
            {
                return null;
            }

        }
    }
}
