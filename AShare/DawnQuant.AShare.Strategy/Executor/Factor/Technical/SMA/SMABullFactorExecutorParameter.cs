using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
   
    /// <summary>
    /// 均线多头排列
    /// </summary>
    public class SMABullFactorExecutorParameter
    {
        public SMABullFactorExecutorParameter()
        {
            KCycle = KCycle.Day;
            FirstSMACycle = 5;
            SecondSMACycle = 10;
            ThreeSMACycle = 20;
            KCycleCount = 10;
        }

        
        [DisplayName("K线周期")]
        public KCycle KCycle { get; set; }

        [DisplayName("周期数目")]
        public int KCycleCount { get; set; }

        [DisplayName("第一条均线周期")]
        public int FirstSMACycle { get; set; }

       
        [DisplayName("第二条均线周期")]
        public int SecondSMACycle { get; set; }

      
        [DisplayName("第三条均线周期")]
        public int ThreeSMACycle { get; set; }

       
        

    }
}
