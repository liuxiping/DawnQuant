using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 均线附近因子,多条均线设置或关系
    /// </summary>
    public class NearTheSMAFactorExecutorParameter : ExecutorParameter
    {
        public NearTheSMAFactorExecutorParameter()
        {
            SMAFactors = new NearTheSMAFactors();
        }

        [DisplayName("均线附近，多条是或的关系")]
        public NearTheSMAFactors SMAFactors { set; get; }

        public override string Serialize()
        {
            if(SMAFactors!=null)
            {
                InternalParameter p = new InternalParameter
                { NearTheSMAFactors = SMAFactors.ToList() };

                return JsonSerializer.Serialize(p);
            }
            else
            {
                return null;
            }
        }



        public override void Initialize(string json, IServiceProvider serviceProvider)
        {
            if(!string.IsNullOrEmpty(json))
            {
                InternalParameter p = JsonSerializer.Deserialize<InternalParameter>(json);
                SMAFactors.AddRange(p.NearTheSMAFactors);
            }
           
        }


        public class SingleNearTheSMAFactorParameter
        {
            public SingleNearTheSMAFactorParameter()
            {
                Precision = 2;
                KCycle = SupportedKCycle.日线;
                SMACycle = 20;
            }

            [DisplayName("精度(%)")]
            public double Precision { get; set; }

            [DisplayName("K线周期")]
            public SupportedKCycle KCycle { get; set; }

            [DisplayName("均线周期")]
            public int SMACycle { get; set; }

            public enum SupportedKCycle
            {
                日线 = 6,
                周线 = 7,
            }


            public override string ToString()
            {
                return "单条均线设置";
            }
        }


        /// <summary>
        /// 内部存储参数数据
        /// </summary>
        public class InternalParameter
        {
            public List<SingleNearTheSMAFactorParameter> NearTheSMAFactors { set; get; }

        }

        public class  NearTheSMAFactors : List<SingleNearTheSMAFactorParameter>
        {
            public override string ToString()
            {
                return "均线附近，多条是或的关系";
            }
        }

    }
}
