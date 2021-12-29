using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{

    /// <summary>
    /// 从股东人数追踪主力
    /// </summary>
    public class HolderNumberExecutorParameterBase
    {

        public List<SingleHolderNumberExecutorParameter> 
            HolderNumberExecutorParameters { get; set; }


        public class SingleHolderNumberExecutorParameter
        {
            /// <summary>
            /// 最小次数
            /// </summary>
            public int MinCount { get; set; }

            /// <summary>
            /// 较上期最小变化率
            /// </summary>
            public double MinHolderNumberChangeRatio { get; set; }


            /// <summary>
            /// 较上期最大变化率
            /// </summary>
            public double MaxHolderNumberChangeRatio { get; set; }
        }
    }

   
}
