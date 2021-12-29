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
    /// 从股东人数追踪主力
    /// </summary>
    public class HolderNumberExecutorParameter:ExecutorParameter
    {

        [DisplayName("股东人数主力追踪参数设置")]
        public HolderNumberExecutorParameters  HolderNumberParameters { get; set; }=new HolderNumberExecutorParameters();
        public override string Serialize()
        {
            if (HolderNumberParameters != null)
            {
                InternalParameter p = new InternalParameter
                { HolderNumberExecutorParameters = HolderNumberParameters.ToList() };
                return JsonSerializer.Serialize(p);
            }
            else
            {
                return null;
            }
           
        }

        public override void Initialize(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var d = JsonSerializer.Deserialize<InternalParameter>(json);

                if (HolderNumberParameters == null)
                {
                    HolderNumberParameters = new HolderNumberExecutorParameters();

                }
                HolderNumberParameters.AddRange(d.HolderNumberExecutorParameters);
            }
        }

        public class InternalParameter
        {
            public List<SingleHolderNumberExecutorParameter> HolderNumberExecutorParameters { set; get; }

        }

        public class SingleHolderNumberExecutorParameter
        {
            /// <summary>
            /// 最小次数
            /// </summary>
            [DisplayName("股东人数变化满足要求次数")]
            public int MinCount { get; set; } = 1;

            /// <summary>
            /// 较上期最小变化率
            /// </summary>
            [DisplayName("股东人数变化最小变动比例(百分比)")]
            public double MinHolderNumberChangeRatio { get; set; } = -100;


            /// <summary>
            /// 较上期最大变化率
            /// </summary>
            [DisplayName("股东人数变化最大变动比例(百分比)")]
            public double MaxHolderNumberChangeRatio { get; set; } = -20;


            public override string ToString()
            {
                return "股东人数主力追踪参数设置";
            }
        }


        public class HolderNumberExecutorParameters: List<SingleHolderNumberExecutorParameter>
        {
            public override string ToString()
            {
                return "股东人数主力追踪参数";
            }
        }
    }

   
}
