using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{
    /// <summary>
    /// 绘图数据
    /// </summary>
    public class StockPlotData
    {
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeDateTime { get; set; }


        public string FormatedTradeDateTime
        {
            get
            {
                return TradeDateTime.ToShortDateString();
            }
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
        /// 昨日收盘价
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
        /// 涨幅
        /// </summary>
        public double Gain { get { return (Close - PreClose) / PreClose; } }
        /// <summary>
        /// 振幅
        /// </summary>
        public double AM { get { return (High - Low) / PreClose; } }


        public bool IsRise { get { return Close>= Open; } }

        //均线
        public double MA5 { get; set; }
        public double MA10 { get; set; }
        public double MA20 { get; set; }
        public double MA30 { get; set; }
        public double MA60 { get; set; }
        public double MA120 { get; set; }
        public double MA250 { get; set; }

    }
}
