using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 排除ST股票
    /// </summary>
    public class ExcludeSTFactorExecutor : IFactorExecutor
    {
        
        private readonly IBasicStockInfoRepository _bsinfoRepository;

        public ExcludeSTFactorExecutor(IBasicStockInfoRepository bsinfoRepository)
        {
            _bsinfoRepository = bsinfoRepository;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            var sts = _bsinfoRepository.Entities.Where(p => p.StockName.ToUpper().Contains("ST")).Select(p => p.TSCode).ToList();
            return tsCodes.Except(sts).ToList();
        }
    
    }
 
}
