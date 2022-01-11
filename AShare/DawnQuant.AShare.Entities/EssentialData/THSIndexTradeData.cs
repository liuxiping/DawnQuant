
using DawnQuant.AShare.Entities.EssentialData.Common;
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
    public class THSIndexTradeData : TradeData
    {
       
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
        /// 总市值
        /// </summary>
        public double TotalMV { get; set; }

        /// <summary>
        /// 流通市值
        /// </summary>
        public double FloatMV { get; set; }


    }
}
