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
    /// 均线之上因子参数
    /// </summary>
    public class AboveSMAFactorExecutorParameter
    {
        [DisplayName("均线设置")]
        public List<SingleAboveSMAFactorParameter> AboveSMAFactors { get; set; }

        public override string ToString()
        {
            return "均线设置,可以同时设置多根均线";
        }
        #region  内部参数
        public class SingleAboveSMAFactorParameter
        {
            public SingleAboveSMAFactorParameter()
            {
                KCycle = KCycle.Day;
                SMACycle = 60;
                SMACycleCount = 20;
            }

            /// <summary>
            /// K线周期
            /// </summary>
            [DisplayName("K线周期")]
            public KCycle KCycle { get; set; }


            /// <summary>
            /// 均线周期
            /// </summary>
            [DisplayName("均线周期")]
            public int SMACycle { get; set; }


            /// <summary>
            /// 循环数
            /// </summary>
            [DisplayName("均线数量")]
            public int SMACycleCount { get; set; }

            public override string ToString()
            {
                return "单条均线设置";
            }
        }
       
        #endregion
    }
}




   
   

