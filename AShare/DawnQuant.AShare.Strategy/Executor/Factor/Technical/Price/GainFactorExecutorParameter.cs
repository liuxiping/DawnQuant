using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 涨幅选择
    /// </summary>
    public class GainFactorExecutorParameter
    {
        public GainFactorExecutorParameter()
        {
            MaxGain = Double.MaxValue;
            MinGain = 0;
            LookBackCycleCount = 1;
            KCycle = KCycle.Day;
        }

        /// <summary>
        /// 振幅（单位%）
        /// </summary>
        [DisplayName("涨幅最大值(%)")]
        public double MaxGain { get; set; }


        /// <summary>
        /// 振幅（单位%）
        /// </summary>
        [DisplayName("涨幅最小值(%)")]
        public double MinGain { get; set; }

        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        [DisplayName("最近交易周期数")]
        public int LookBackCycleCount { get; set; }

        [DisplayName("K线周期")]
        public KCycle KCycle { get; set; }

        


    }
}
