
namespace DawnQuant.App.Core.Models.AShare.UserProfile
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
