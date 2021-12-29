using DawnQuant.App.Models.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Common
{

    /// <summary>
    /// 业绩预测 
    /// </summary>
    public class PerformanceForecastFromTHSExecutorParameter : ExecutorParameter
    {
        /// <summary>
        /// 机构预测数目
        /// </summary>
        /// 
        [DisplayName("最小机构预测数目")]
        public int MinForecastOrgCount { get; set; } = 1;


        [DisplayName("最大机构预测数目")]
        public int MaxForecastOrgCount { get; set; }=int.MaxValue;

        /// <summary>
        /// 每股收益变动
        /// </summary>
        [DisplayName("最小每股收益变动比率(百分数)")]
        public double MinEarningsPerShareChangeRatio { get; set; } = 0;

        [DisplayName("最大每股收益变动比率(百分数)")]
        public double MaxEarningsPerShareChangeRatio { get; set; } = double.MaxValue;


        /// <summary>
        /// 净利润变动
        /// </summary>
        [DisplayName("最小净利润变动比率(百分数)")]
        public double MinRetainedProfitsChangeRatio { get; set; }=0;

        [DisplayName("最大净利润变动比率(百分数)")]
        public double MaxRetainedProfitsChangeRatio { get; set; } = double.MaxValue;

        public override string Serialize()
        {
             return JsonSerializer.Serialize(this);
        }
    }
}
