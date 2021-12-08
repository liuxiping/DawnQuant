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
    ///  涨停次数
    /// </summary>
    public class HardenFactorExecutorParameter : ExecutorParameter
    {

        public HardenFactorExecutorParameter()
        {
            LatestTradeDateCount = 60;
            HardenCount = 1;
            EndDate = null;
            StartDate = null;
        }
        public override string Serialize()
        {

            return JsonSerializer.Serialize(this);
        }

        [DisplayName("最近交易日数量")]
        [Description("此项参数具有最高优先级，此项为空的时候，开始日期和结束日期才会生效")]
       
        public int? LatestTradeDateCount { get; set; }


        /// <summary>
        /// 振幅（单位%）
        /// </summary>
        [DisplayName("开始日期(为空则为最近交易日期)")]
        public DateTime? StartDate { get; set; }


        [DisplayName("结束日期(为空则为最近交易日期)")]
        public DateTime? EndDate { get; set; }


        [DisplayName("涨停次数")]
        public int HardenCount { get; set; }

    }
}
