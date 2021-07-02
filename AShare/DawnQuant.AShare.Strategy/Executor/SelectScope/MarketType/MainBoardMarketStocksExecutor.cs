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
    /// 主板
    /// </summary>
    public class MainBoardMarketStocksExecutor : ISelectScopeExecutor
    {
        public object Parameter { get; set; }


        public MainBoardMarketStocksExecutor(IBasicStockInfoRepository repository)
        {
            _repository = repository;
        }

        IBasicStockInfoRepository _repository;

        public List<string> Execute()
        {
            return _repository.Entities.Where(p => p.MarketType == StockEssentialDataConst.MainBoard
            &&p.ListedStatus== StockEssentialDataConst.Listing)
                .Select(p => p.TSCode).ToList();
        }
    }
}
