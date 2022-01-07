
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
    /// 同花顺概念和行业指数交易数据
    /// </summary>
    public class THSIndexTradeData : BaseEntity<DateTime>
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
        /// 平均价
        /// </summary>
        public double AvgClose { get; set; }


        /// <summary>
        /// 涨跌点位
        /// </summary>
        public double Change { get; set; }


        /// <summary>
        /// 涨跌幅
        /// </summary>
        public double PctChange { get; set; }
        

        /// <summary>
        /// 成交量
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// 总市值
        /// </summary>
        public double Total_MV { get; set; }

        /// <summary>
        /// 流通市值
        /// </summary>
        public double Flaot_MV { get; set; }


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
