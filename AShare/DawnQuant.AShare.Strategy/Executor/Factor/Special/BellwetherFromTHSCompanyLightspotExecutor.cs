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
    /// 从同花顺公司亮点中分析提取的龙头股
    /// </summary>
    public class BellwetherFromTHSCompanyLightspotExecutor : BellwetherFromTHSCompanyLightspotExecutorBase, IFactorExecutor
    {
      
        public BellwetherFromTHSCompanyLightspotExecutor(IBellwetherRepository bellwetherRepository)
            : base(bellwetherRepository)
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
