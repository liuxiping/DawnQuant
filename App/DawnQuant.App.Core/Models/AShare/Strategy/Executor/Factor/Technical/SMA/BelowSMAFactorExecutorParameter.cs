using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 均线之下因子参数
    /// </summary>
    public class BelowSMAFactorExecutorParameter: ExecutorParameter
    {

        public BelowSMAFactorExecutorParameter()
        {
            SMAFactors = new  BelowSMAFactors();
        }

        [DisplayName("均线设置")]
        public BelowSMAFactors SMAFactors { get; set; }



        public override string Serialize()
        {
            List<SingleBelowSMAFactorParameter> smas = new List<SingleBelowSMAFactorParameter>();
            foreach (SingleBelowSMAFactorParameter p in SMAFactors)
            {
                smas.Add(p);
            }
            return JsonSerializer.Serialize(new InternalParameter { BelowSMAFactors = smas });
        }

        public override void Initialize(string json, IServiceProvider serviceProvider)
        {
            if (!string.IsNullOrEmpty(json))
            {
                InternalParameter p = JsonSerializer.Deserialize<InternalParameter>(json);
                SMAFactors.AddRange(p.BelowSMAFactors);
            }
        }

        public override string ToString()
        {
            return "均线设置,可以同时设置多根均线";
        }

        #region  内部参数
        public class SingleBelowSMAFactorParameter
        {
            public SingleBelowSMAFactorParameter()
            {
                KCycle = SupportedKCycle.日线;
                SMACycle = 10;
                SMACycleCount = 1;
            }

            /// <summary>
            /// K线周期
            /// </summary>
            [DisplayName("K线周期")]
            public SupportedKCycle KCycle { get; set; }


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


        public class BelowSMAFactors : List<SingleBelowSMAFactorParameter>
        {
            public override string ToString()
            {
                return "均线设置,可以同时设置多根均线";
            }
        }


        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }
        /// <summary>
        /// 内部序列化参数
        /// </summary>
        public class InternalParameter
        {
            public List<SingleBelowSMAFactorParameter> BelowSMAFactors { get; set; }

        }
        #endregion
    }
}




   
   

