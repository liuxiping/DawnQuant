using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{
    /// <summary>
    /// 从10大流通股追踪人数
    /// </summary>
    public class Top10FloatHolderExecutorParameterBase
    {
        
        public List<SingleTop10FloatHolderExecutorParameter> 
            Top10FloatHolderExecutorParameters { get; set; } 

        public class SingleTop10FloatHolderExecutorParameter
        {
            /// <summary>
            /// 最小次数
            /// </summary>
            public int MinCount { get; set; }

            /// <summary>
            /// 新进股东最小持股比例
            /// </summary>
            public double MinNewHolderRatio { get; set; }

            /// <summary>
            /// 新进股东最大持股比例
            /// </summary>
            public double MaxNewHolderRatio { get; set; }


            /// <summary>
            /// 新进股东累计持股比例
            /// </summary>
            public double NewHolderTotalRatio { get; set; }
        }
    }
}
