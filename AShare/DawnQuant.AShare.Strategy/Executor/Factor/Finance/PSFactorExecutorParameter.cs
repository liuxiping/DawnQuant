using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    public class PSFactorExecutorParameter 
    {

        public PSFactorExecutorParameter()
        {
            PSMax = 100;
            PSMin = 0;

            PSTTMMax = 100;
            PSTTMMin = 0;

            TradeDate = null;
        }

        [DisplayName("最大市销率")]
        public double PSMax { get; set; }

        [DisplayName("最小市销率")]
        public double PSMin { get; set; }

        [DisplayName("最大市销率(TTM)")]
        public double PSTTMMax { get; set; }

        [DisplayName("最小市销率(TTM)")]
        public double PSTTMMin { get; set; }

        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }
    }
}
