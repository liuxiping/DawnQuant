using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 相关分析 形态选股参数
    /// </summary>

    public class CorrelationFactorExecutorParameter : ExecutorParameter
    {
        public CorrelationFactorExecutorParameter()
        {
            KCycle = SupportedKCycle.日线;
            IsCalculateClosePrice = false;
            ClosePriceCL = 0.85;

            IsCalculateSMA = true;
            SMACycleCount = 5;
            SMACL = 0.85;

            IsCalculateVol = false;
            VolCL = 0.85;

            Start = DateTime.Now.Date;
            End = DateTime.Now.Date;
        }

        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// 股票代码
        /// </summary>
        [DisplayName("股票代码")]
        public string TSCode { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        [DisplayName("开始时间")]
        public DateTime Start { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        [DisplayName("结束时间")]
        public DateTime End { get; set; }

        /// <summary>
        /// K线周期
        /// </summary>
        [DisplayName("K线周期")]
        public SupportedKCycle KCycle { get; set; }


        /// <summary>
        /// 收盘价
        /// </summary>
        [DisplayName("匹配收盘价格")]
        public bool IsCalculateClosePrice { get; set; }

        /// <summary>
        /// 收盘价格相关系数，输出结果大于等于此系数
        /// </summary>
        [DisplayName("收盘价最小相似度")]
        public double ClosePriceCL { get; set; }

        /// <summary>
        /// 匹配均线走势
        /// </summary>
        [DisplayName("匹配均线走势")]
        public bool IsCalculateSMA { get; set; }

        /// <summary>
        /// 均线数量
        /// </summary>
        [DisplayName("均线数量")]
        public int SMACycleCount { get; set; }

        /// <summary>
        /// 均线相关系数，输出结果大于等于此系数
        /// </summary>
        [DisplayName("均线最小相似度")]
        public double SMACL { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        [DisplayName("匹配成交量")]
        public bool IsCalculateVol { get; set; }


        /// <summary>
        /// 成交量系数，输出结果大于等于此系数
        /// </summary>
        [DisplayName("成交量最小相似度")]
        public double VolCL { get; set; }


        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }

    }

    
}
