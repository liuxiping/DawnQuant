using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 股息率
    /// </summary>
    
    public class DVFactorExecutorParameter : ExecutorParameter
    {

        public DVFactorExecutorParameter()
        {
            DVMax = 100;
            DVMin = 0;
            DVTTMMax = 100;
            DVTTMMin = 0;
            TradeDate = null;
        }

        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
        }

        [DisplayName("最大股息率(%)")]
        public double DVMax { get; set; }

        
        [DisplayName("最小股息率(%)")]
        public double DVMin { get; set; }


        
        [DisplayName("最大股息率(TTM)(%)")]
        public double DVTTMMax { get; set; }

        
        [DisplayName("最小股息率(TTM)(%)")]
        public double DVTTMMin { get; set; }

       
        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }

       
    }

}
