using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{

    /// <summary>
    /// 业绩预测 
    /// </summary>
    public class PerformanceForecastFromTHSExecutorParameterBase 
    {
        /// <summary>
        /// 机构预测数目
        /// </summary>
        public int MinForecastOrgCount { get; set; }

        public int MaxForecastOrgCount { get; set; }

        /// <summary>
        /// 每股收益变动
        /// </summary>
        public double MinEarningsPerShareChangeRatio { get; set; }

        public double MaxEarningsPerShareChangeRatio { get; set; }


        /// <summary>
        /// 净利润变动
        /// </summary>
        public double  MinRetainedProfitsChangeRatio { get; set; }

        public double  MaxRetainedProfitsChangeRatio { get; set; }
    }
}
