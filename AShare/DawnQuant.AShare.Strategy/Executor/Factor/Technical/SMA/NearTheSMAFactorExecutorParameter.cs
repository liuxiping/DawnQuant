
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
    /// 均线附近因子,多条均线设置或关系
    /// </summary>
    public class NearTheSMAFactorExecutorParameter
    {
        [DisplayName("均线附近，多条是或的关系")]
        public List<SingleNearTheSMAFactorParameter> NearTheSMAFactors { set; get; }

        public class SingleNearTheSMAFactorParameter
        {
            public SingleNearTheSMAFactorParameter()
            {
                Precision = 2;
                KCycle = KCycle.Day;
                SMACycle = 20;
            }

            [DisplayName("精度(%)")]
            public double Precision { get; set; }

            [DisplayName("K线周期")]
            public KCycle KCycle { get; set; }

            [DisplayName("均线周期")]
            public int SMACycle { get; set; }
        }
    }
}
