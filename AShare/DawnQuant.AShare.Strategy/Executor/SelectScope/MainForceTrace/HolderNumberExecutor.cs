using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{

    /// <summary>
    /// 从股东人数追踪主力
    /// </summary>
    public class HolderNumberExecutor : HolderNumberExecutorBase, ISelectScopeExecutor
    {
     

        public HolderNumberExecutor(IHolderNumberRepository holderNumberRepository):base(holderNumberRepository)
        {
         
        }

      
    }
    
    
}
