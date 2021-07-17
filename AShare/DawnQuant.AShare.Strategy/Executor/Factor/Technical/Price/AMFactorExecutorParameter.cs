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
    /// 振幅参数
    /// </summary>
   
    public class AMFactorExecutorParameter
    {

        public AMFactorExecutorParameter()
        {
            MaxAM = Double.MaxValue;
            MinAM = 0;
            LookBackCycleCount = 1;
            KCycle = KCycle.Day;
        }


        [DisplayName("K线周期")]
        public KCycle KCycle { get; set; }

        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        [DisplayName("最近交易周期数")]
        public int LookBackCycleCount { get; set; }

        /// <summary>
        /// 振幅（单位%）
        /// </summary>
        [DisplayName("振幅最大值(%)")]
        public double MaxAM { get; set; }

        /// <summary>
        /// 振幅（单位%）
        /// </summary>
        [DisplayName("振幅最小值(%)")]
        public double MinAM { get; set; }

        
    }
}
