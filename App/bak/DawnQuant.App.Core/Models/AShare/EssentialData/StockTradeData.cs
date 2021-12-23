using System.ComponentModel.DataAnnotations;

namespace DawnQuant.App.Core.Models.AShare.EssentialData
{
    /// <summary>
    /// 股票交易数据
    /// </summary>
    public class StockTradeData
    {

        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// 前收盘价
        /// </summary>
        public double PreClose { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }

        /// <summary>
        ///  最低价
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// 成交额
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        [Key]
        public DateTime TradeDateTime { get; set; }
        /// <summary>
        /// 复权因子
        /// </summary>
        public double AdjustFactor { get; set; }
        

    }
}
