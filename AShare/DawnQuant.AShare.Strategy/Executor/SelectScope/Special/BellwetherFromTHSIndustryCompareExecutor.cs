using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{
    public class BellwetherFromTHSIndustryCompareExecutor : BellwetherFromTHSIndustryCompareExecutorBase, ISelectScopeExecutor
    {
     
        public BellwetherFromTHSIndustryCompareExecutor(IBellwetherRepository bellwetherRepository):
            base(bellwetherRepository)
           
        {
        }

       

    }
}
