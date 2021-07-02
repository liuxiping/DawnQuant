using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{

    /// <summary>
    /// 自选分类
    /// </summary>
    public class SelfSelectStockCategoryExecutor : ISelectScopeExecutor
    {
        public object Parameter { get; set; }

        private readonly ISelfSelectStockRepository _selfSelectStockRepository;


        public SelfSelectStockCategoryExecutor(ISelfSelectStockRepository selfSelectStockRepository)
        {
            _selfSelectStockRepository = selfSelectStockRepository;
        }

        public List<string> Execute()
        {
            SelfSelectStockCategoryExecutorParameter p = (SelfSelectStockCategoryExecutorParameter)Parameter;

            List<string> tscodes = new List<string>();
            if (p != null && p.SupportedCategories.Count > 0)
            {
                foreach (var cid in p.SupportedCategories)
                {
                    var temp = _selfSelectStockRepository.Entities.Where(p=>p.CategoryId== cid).Select(p=>p.TSCode).ToList();
                    if (temp != null && temp.Count > 0)
                    {
                        tscodes.AddRange(temp);
                        tscodes = tscodes.Distinct().ToList();
                    }

                }
            }
            return tscodes;
        }

    }


    
}
