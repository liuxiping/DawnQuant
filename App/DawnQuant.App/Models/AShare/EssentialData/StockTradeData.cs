using DawnQuant.App.Models.AShare.EssentialData.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{
    /// <summary>
    /// 股票交易数据
    /// </summary>
    public class StockTradeData:TradeData
    {

        /// <summary>
        /// 自由换手率
        /// </summary>
        public double TurnoverFree { get; set; }


        /// <summary>
        /// 复权因子
        /// </summary>
        public double AdjustFactor { get; set; }


    }
}
