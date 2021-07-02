using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{
    /// <summary>
    /// 中小板
    /// </summary>
    public class SAMBoardMarketStocksExecutor : ISelectScopeExecutor
    {
        public object Parameter { get; set; }
        public SAMBoardMarketStocksExecutor(IBasicStockInfoRepository repository)
        {
            _repository = repository;
        }
        IBasicStockInfoRepository _repository;

        public List<string> Execute()
        {
            return _repository.Entities.Where(p => p.MarketType == StockEssentialDataConst.SAMBoard
            && p.ListedStatus == StockEssentialDataConst.Listing)
                .Select(p => p.TSCode).ToList();
        }
    }
}
