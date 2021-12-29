using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 从股东人数追踪主力
    /// </summary>
    public class HolderNumberExecutor : HolderNumberExecutorBase, IFactorExecutor
    {
        public HolderNumberExecutor(IHolderNumberRepository holderNumberRepository):base(holderNumberRepository)
        {
        }

        public List<string> Execute(List<string> tsCodes)
        {
            if (tsCodes != null && tsCodes.Count > 0)
            {
                return base.Execute().Intersect(tsCodes).ToList();
            }
            else
            {
               return null;
            }

        }
      
    }
    
    
}
