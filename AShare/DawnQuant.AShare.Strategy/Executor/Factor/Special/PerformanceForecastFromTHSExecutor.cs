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
    /// 业绩预测 
    /// </summary>
    public class PerformanceForecastFromTHSExecutor : PerformanceForecastFromTHSExecutorBase, IFactorExecutor
    {
    
        public PerformanceForecastFromTHSExecutor(IPerformanceForecastRepository forecastRepository):
            base(forecastRepository)
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
