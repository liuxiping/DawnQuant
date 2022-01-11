
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.EssentialData.Common
{
    /// <summary>
    /// 交易数据
    /// </summary>
    public class TradeData: BaseEntity<DateTime>
    {
        public override DateTime GetKeyValue()
        {
            return TradeDateTime;
        }

        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }

       
        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }

        /// <summary>
        ///  最低价
        /// </summary>
        public double Low { get; set; }


        /// <summary>
        /// 前收盘价
        /// </summary>
        public double PreClose { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public double Volume { get; set; }


        /// <summary>
        /// 成交额
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 换手率
        /// </summary>
        public double Turnover { get; set; }

        /// <summary>
        /// 交易时间
        /// </summary>
        [Key]
        public DateTime TradeDateTime { get; set; }


    }
}
