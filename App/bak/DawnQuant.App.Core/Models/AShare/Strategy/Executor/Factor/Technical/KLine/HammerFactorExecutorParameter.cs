using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{
    public class HammerFactorExecutorParameter: ExecutorParameter
    {
        public HammerFactorExecutorParameter()
        {
           
            KCycle = SupportedKCycle.日线;
            TradeDateTime = null;
            LookBackCycleCount = 3;
        }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        [DisplayName("K线周期")]
        public SupportedKCycle KCycle { get; set; }


        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        [DisplayName("最近交易周期数")]
        public int LookBackCycleCount { get; set; }

        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        [DisplayName("交易时间,交易时间为空取最近交易日期")]
        public DateTime? TradeDateTime { get; set; }


        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }
    }
}
