using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 市值参数
    /// </summary>
    public class MarketValueFactorExecutorParameter
    {

        public MarketValueFactorExecutorParameter()
        {
            MinMarketValue = 0;
            MaxMarketValue = double.MaxValue;
            TradeDate = null;
        }

        [DisplayName("最小市值(单位亿元)")]
        public double MinMarketValue { get; set; }

        [DisplayName("最大市值(单位亿元)")]
        public double MaxMarketValue { get; set; }

        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }
    }
}
