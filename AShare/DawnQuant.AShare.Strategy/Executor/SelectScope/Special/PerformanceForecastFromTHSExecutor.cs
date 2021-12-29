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
    /// 业绩预测 
    /// </summary>
    public class PerformanceForecastFromTHSExecutor : PerformanceForecastFromTHSExecutorBase, ISelectScopeExecutor
    {
   
        public PerformanceForecastFromTHSExecutor(IPerformanceForecastRepository forecastRepository):
            base(forecastRepository)
        {
        }

        
    
    }
}
