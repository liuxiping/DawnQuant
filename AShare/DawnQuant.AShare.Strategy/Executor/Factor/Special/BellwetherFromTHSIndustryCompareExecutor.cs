using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Executor.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    public class BellwetherFromTHSIndustryCompareExecutor : BellwetherFromTHSIndustryCompareExecutorBase,IFactorExecutor
    {
    
        public BellwetherFromTHSIndustryCompareExecutor(IBellwetherRepository bellwetherRepository):
            base(bellwetherRepository)
           
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
