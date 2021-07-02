using DawnQuant.App.Utils;
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
    /// 均线之上因子参数
    /// </summary>
    public class AboveSMAFactorExecutorParameter : ExecutorParameter
    {
        public AboveSMAFactorExecutorParameter()
        {
            SMAFactors = new AboveSMAFactors();
        }

        public override string Serialize()
        {
            List<SingleAboveSMAFactorParameter> smas = new List<SingleAboveSMAFactorParameter>();
            foreach (SingleAboveSMAFactorParameter p in SMAFactors)
            {
                smas.Add(p);
            }
            return JsonSerializer.Serialize(new InternalParameter { AboveSMAFactors = smas });
        }

        public override void Initialize(string json)
        {
            if(!string.IsNullOrEmpty(json))
            {
                InternalParameter p = JsonSerializer.Deserialize<InternalParameter>(json);
                SMAFactors.AddRange(p.AboveSMAFactors);
            }
        }

        [DisplayName("均线设置")]
        public AboveSMAFactors SMAFactors { get; set; }


        public override string ToString()
        {
            return "均线设置,可以同时设置多根均线";
        }

        #region  内部参数
        public class SingleAboveSMAFactorParameter
        {
            public SingleAboveSMAFactorParameter()
            {
                KCycle = SupportedKCycle.日线;
                SMACycle = 60;
                SMACycleCount = 20;
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

        public enum SupportedKCycle
        {
            日线 = 6,
            周线 = 7,
        }


        public class AboveSMAFactors : List<SingleAboveSMAFactorParameter>
        {
            public override string ToString()
            {
                return "均线设置,可以同时设置多根均线";
            }
        }

        /// <summary>
        /// 内部序列化参数
        /// </summary>
        public class InternalParameter
        {
            public List<SingleAboveSMAFactorParameter> AboveSMAFactors { get; set; }

        }
        #endregion
    }
}




   
   

