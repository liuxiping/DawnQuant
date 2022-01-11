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
    public class PlotData
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
        public double Gain { 
            get 
            {
                if (PreClose > 0)
                {
                    return (Close - PreClose) / (PreClose * 100);
                }
                else
                {
                    return 0;
                }
            } 
        }
        /// <summary>
        /// 振幅
        /// </summary>
        public double AM
        {
            get
            {
                if (PreClose > 0)
                {
                    return (High - Low) / (PreClose * 100);
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool IsRise { get { return Close>= Open; } }

        /// <summary>
        /// 换手率
        /// </summary>
        public double Turnover { get; set; }

        /// <summary>
        /// 缩小 绘图使用
        /// </summary>
        public double ScaleDownTurnoverFree
        {
            get
            {
                return TurnoverFree / 10000;
            }
        }

        /// <summary>
        /// 换手率（自由流通股）
        /// </summary>
        public double TurnoverFree { get; set; }

        //均线
        public double MA5 { get; set; }
        public double MA10 { get; set; }
        public double MA20 { get; set; }
        public double MA30 { get; set; }
        public double MA60 { get; set; }
        public double MA120 { get; set; }
        public double MA250 { get; set; }

        //MACD
        /// <summary>
        /// 快线
        /// </summary>
        public double MACD { get; set; }

        /// <summary>
        /// 慢线
        /// </summary>
        public double MacdSignal { get; set; }


        /// <summary>
        /// 柱子
        /// </summary>
        public double MacdHist { get; set; }
      

    }
}
