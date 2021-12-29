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
    /// 从10大流通股追踪人数
    /// </summary>
    public class Top10FloatHolderExecutor : Top10FloatHolderExecutorBase, ISelectScopeExecutor
    {
     
        public Top10FloatHolderExecutor(ITop10FloatHolderRepository top10FloatHolderRepository) : base(top10FloatHolderRepository)
        {
        }


    }
}
