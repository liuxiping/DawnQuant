using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Factor
{
   
    /// <summary>
    /// 均线多头排列
    /// </summary>
    public class SMABullFactorExecutorParameter : ExecutorParameter
    {
        public SMABullFactorExecutorParameter()
        {
            KCycle = SupportedKCycle.日线;
            FirstSMACycle = 60;
            SecondSMACycle = 120;
            ThreeSMACycle = 250;
            KCycleCount = 20;
        }
        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
        }

        [DisplayName("K线周期")]
        public SupportedKCycle KCycle { get; set; }

        [DisplayName("周期数目")]
        public int KCycleCount { get; set; }

        [DisplayName("第一条均线周期")]
        public int FirstSMACycle { get; set; }

       
        [DisplayName("第二条均线周期")]
        public int SecondSMACycle { get; set; }

      
        [DisplayName("第三条均线周期")]
        public int ThreeSMACycle { get; set; }


        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }

    }
}
