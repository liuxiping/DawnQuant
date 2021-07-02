
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 排除自选股票
    /// </summary>
    public class ExcludeSelfSelectFactorExecutor : IFactorExecutor
    {

        private readonly ISelfSelectStockRepository _selfSelectStockRepository;


        public ExcludeSelfSelectFactorExecutor(ISelfSelectStockRepository selfSelectStockRepository)
        {
            _selfSelectStockRepository = selfSelectStockRepository;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            ExcludeSelfSelectFactorExecutorParameter p = (ExcludeSelfSelectFactorExecutorParameter)Parameter;

            List<string> extscodes = new List<string>();
            if (p != null && p.ExcludeCategories.Count > 0)
            {
                foreach (var cid in p.ExcludeCategories)
                {

                    var temp = _selfSelectStockRepository.Entities
                        .Where(p=>p.CategoryId==cid).Select(p=>p.TSCode)
                        .ToList();

                    if (temp != null && temp.Count > 0)
                    {
                        extscodes.AddRange(temp);
                        //去重
                        extscodes = extscodes.Distinct().ToList();
                    }

                }
            }
            return tsCodes.Except(extscodes).ToList();
        }
    }
}
