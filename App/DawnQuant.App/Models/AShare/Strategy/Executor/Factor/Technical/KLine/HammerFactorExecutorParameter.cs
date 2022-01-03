using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Factor
{
    public class HammerFactorExecutorParameter: ExecutorParameter
    {

        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        [DisplayName("K线周期")]
        public SupportedKCycle KCycle { get; set; } = SupportedKCycle.日线;


        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        [DisplayName("交易周期数(最小为6)")]
        public int LookBackCycleCount { get; set; } = 6;


        /// <summary>
        /// 满足条件的次数
        /// </summary>
        [DisplayName("出现锤子线次数")]
        public int MeetCount { get; set; } = 1;


        /// <summary>
        /// 最近一个交易日期是否满足
        /// </summary>
        [DisplayName("最近一个周期是否是锤子线")]
        public bool IsLatestTradeDateTimeMeet { get; set; } = false;

        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }
    }
}
