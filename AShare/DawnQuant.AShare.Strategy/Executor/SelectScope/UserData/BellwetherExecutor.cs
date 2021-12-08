using DawnQuant.AShare.Repository.Abstract.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{
    /// <summary>
    /// 龙头股
    /// </summary>
    public class BellwetherExecutor : ISelectScopeExecutor
    {
        public object Parameter { get; set; }

        private readonly IBellwetherStockRepository _bellwetherStockRepository;


        public BellwetherExecutor(IBellwetherStockRepository bellwetherStockRepository)
        {
            _bellwetherStockRepository = bellwetherStockRepository;
        }

        public List<string> Execute()
        {
            BellwetherExecutorParameter pa = (BellwetherExecutorParameter)Parameter;
            var temp = _bellwetherStockRepository.Entities.Where(p => p.UserId == pa.UserId).Select(p => p.TSCode).ToList();
            return temp;
        }
    }
}
