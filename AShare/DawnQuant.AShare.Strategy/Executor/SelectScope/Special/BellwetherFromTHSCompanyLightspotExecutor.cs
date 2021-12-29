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
    /// 从同花顺公司亮点中分析提取的龙头股
    /// </summary>
    public class BellwetherFromTHSCompanyLightspotExecutor : BellwetherFromTHSCompanyLightspotExecutorBase, ISelectScopeExecutor
    {
     
        public BellwetherFromTHSCompanyLightspotExecutor(IBellwetherRepository bellwetherRepository)
            : base(bellwetherRepository)
        {
        }

    }

   
}
