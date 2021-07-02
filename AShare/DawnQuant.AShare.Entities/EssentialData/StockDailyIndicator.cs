
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.EssentialData
{
    /// <summary>
    /// 股票每日指标
    /// </summary>
    public class StockDailyIndicator:BaseEntity<DateTime>
    {

        public override DateTime GetKeyValue()
        {
            return TradeDate;
        }

        /// <summary>
        /// 交易日期
        /// </summary>
        [Key]
        [DataType(DataType.Date)]
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// 换手率
        /// </summary>
        public double Turnover { get; set; }

        /// <summary>
        /// 换手率（自由流通股）
        /// </summary>
        public double TurnoverFree { get; set; }


        /// <summary>
        /// 量比
        /// </summary>
        public double VolumeRatio { get; set; }

        /// <summary>
        /// 市盈率（总市值/净利润， 亏损的PE为空）
        /// </summary>
        public double? PE { get; set; }

        /// <summary>
        /// 市盈率（TTM，亏损的PE为空）
        /// </summary>
        public double? PETTM { get; set; }

        /// <summary>
        /// 市净率（总市值/净资产）
        /// </summary>
        public double PB { get; set; }

        /// <summary>
        ///市销率 
        /// </summary>
        public double PS { get; set; }

        /// <summary>
        /// 市销率（TTM）
        /// </summary>
        public double PSTTM { get; set; }

        /// <summary>
        /// 股息率 （%）
        /// </summary>
        public double DV { get; set; }

        /// <summary>
        /// 股息率（TTM）（%）
        /// </summary>
        public double DVTTM { get; set; }

        /// <summary>
        /// 总股本 
        /// </summary>
        public double TotalShare { get; set; }

        /// <summary>
        /// 流通股本 （万股）
        /// </summary>
        public double FloatShare { get; set; }

        /// <summary>
        /// 自由流通股本 （万）
        /// </summary>
        public double FreeShare { get; set; }

        /// <summary>
        /// 总市值 （万元）
        /// </summary>
        public double TotalMarketValue { get; set; }

        /// <summary>
        /// 流通市值（万元）
        /// </summary>
        public double CirculateMarketValue { get; set; }

    }
}
