using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    public class HammerFactorExecutorParameter
    {
        public HammerFactorExecutorParameter()
        {
           
            KCycle = KCycle.Day;
            TradeDateTime = null;
            LookBackCycleCount = 3;
        }


        [DisplayName("K线周期")]
        public KCycle KCycle { get; set; }


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

      

       
    }
}
