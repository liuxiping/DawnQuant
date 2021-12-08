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
    /// PE
    /// </summary>
    public class PEFactorExecutorParameter : ExecutorParameter
    {

        public PEFactorExecutorParameter()
        {
            PEMax = 100;
            PEMin = 0;
            PETTMMax = 100;
            PETTMMin = 0;
            TradeDate = null;
        }
        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }


        [DisplayName("最大市盈率")]
        public double PEMax { get; set; }

        [DisplayName("最小市盈率")]
        public double PEMin { get; set; }


        [DisplayName("最大市盈率(TTM)")]
        public double PETTMMax { get; set; }

        [DisplayName("最小市盈率(TTM)")]
        public double PETTMMin { get; set; }

        [DisplayName("交易日期，为空为最新日期")]
        public DateTime? TradeDate { get; set; }
    }
   
}
