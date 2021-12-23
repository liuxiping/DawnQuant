using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 涨幅选择
    /// </summary>
    public class GainFactorExecutorParameter : ExecutorParameter
    {
        public GainFactorExecutorParameter()
        {
            MaxGain = Double.MaxValue;
            MinGain = 0;
            LookBackCycleCount = 1;
            KCycle = SupportedKCycle.日线;
        }
        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
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
        public SupportedKCycle KCycle { get; set; }


        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }

    }
}
