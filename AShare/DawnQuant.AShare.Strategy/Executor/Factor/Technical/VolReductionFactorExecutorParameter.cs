
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
    /// 缩量因子
    /// </summary>
    public class VolReductionFactorExecutorParameter 
    {

        public VolReductionFactorExecutorParameter()
        {
            KCycle =  KCycle.Day;
            ReductionRatio = 40;
        }

        [DisplayName("K线周期")]
        public KCycle KCycle { get; set; }


        [DisplayName("缩量比率(%)")]
        public double ReductionRatio { get; set; }

    }
}
