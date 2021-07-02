using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.UserProfile
{
    public class StockStrategyContext
    {
        /// <summary>
        /// 分类
        /// </summary>
        public StockStrategyCategory  StrategyCategory { get; set; }

        /// <summary>
        /// 策略
        /// </summary>
        public List<StockStrategy>  StockStrategies { get; set; }
    }
}
