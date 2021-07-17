using DawnQuant.App.Models.AShare.EssentialData;
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
    /// 缩量因子
    /// </summary>
    public class VolReductionFactorExecutorParameter : ExecutorParameter
    {

        public VolReductionFactorExecutorParameter()
        {
            KCycle =  SupportedKCycle.日线;
            ReductionRatio = 40;
        }
        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
        }
        [DisplayName("K线周期")]
        public SupportedKCycle KCycle { get; set; }


        [DisplayName("缩量比率(%)")]
        public double ReductionRatio { get; set; }

        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }
    }
}
