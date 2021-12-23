using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{
   

    /// <summary>
    /// 市净率
    /// </summary>
    public class PBFactorExecutorParameter : ExecutorParameter
    {

        public PBFactorExecutorParameter()
        {
            PBMax = 100;
            PBMin = 0;
            TradeDate = null;
        }
        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
        }


        [DisplayName("最大PB")]
        public double PBMax { get; set; }

        [DisplayName("最小PB")]
        public double PBMin { get; set; }


        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }
    }
}
