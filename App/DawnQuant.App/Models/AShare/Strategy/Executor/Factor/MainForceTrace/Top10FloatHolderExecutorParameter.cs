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
    /// 从10大流通股追踪人数
    /// </summary>
    public class Top10FloatHolderExecutorParameter : ExecutorParameter
    {

        [DisplayName("10大流通股主力追踪参数设置")]
        public Top10FloatHolderExecutorParameters Top10FloatHolderParameters
        { get; set; } = new Top10FloatHolderExecutorParameters();

        public override string Serialize()
        {
            if (Top10FloatHolderParameters != null)
            {
                InternalParameter p = new InternalParameter
                { Top10FloatHolderExecutorParameters = Top10FloatHolderParameters.ToList() };
                return JsonSerializer.Serialize(p);
            }
            else
            {
                return null;
            };
        }

        public override void Initialize(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var d = JsonSerializer.Deserialize<InternalParameter>(json);

                if (Top10FloatHolderParameters == null)
                {
                    Top10FloatHolderParameters = new  Top10FloatHolderExecutorParameters();

                }
                Top10FloatHolderParameters.AddRange(d.Top10FloatHolderExecutorParameters);
            }
        }



        public class InternalParameter
        {
            public List<SingleTop10FloatHolderExecutorParameter> Top10FloatHolderExecutorParameters { set; get; }

        }
        public class SingleTop10FloatHolderExecutorParameter
        {
            /// <summary>
            /// 最小次数
            /// </summary>
            [DisplayName("新进股东满足要求次数")]
            public int MinCount { get; set; } = 1;

            /// <summary>
            /// 新进股东最小持股比例
            /// </summary>
            [DisplayName("新进股东最小持股比例(百分比)")]
            public double MinNewHolderRatio { get; set; } = 0;

            /// <summary>
            /// 新进股东最大持股比例
            /// </summary>
            [DisplayName("新进股东最大持股比例(百分比)")]
            public double MaxNewHolderRatio { get; set; } = 100;


            /// <summary>
            /// 新进股东累计持股比例
            /// </summary>
            [DisplayName("新进股东累计持股比例")]
            public double NewHolderTotalRatio { get; set; }=0;

            public override string ToString()
            {
                return "10大流通股主力追踪";
            }
        }

        public class Top10FloatHolderExecutorParameters: List<SingleTop10FloatHolderExecutorParameter>
        {
            public override string ToString()
            {
                return "10大流通股主力追踪参数";
            }
        }
    }
}
