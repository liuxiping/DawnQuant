using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
   

    /// <summary>
    /// 市净率
    /// </summary>
    public class PBFactorExecutorParameter 
    {

        public PBFactorExecutorParameter()
        {
            PBMax = 100;
            PBMin = 0;
            TradeDate = null;
        }

        [DisplayName("最大PB")]
        public double PBMax { get; set; }

        [DisplayName("最小PB")]
        public double PBMin { get; set; }


        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }
    }
}
