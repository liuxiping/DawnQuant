
namespace DawnQuant.DataCollector.Core.Utils
{
    /// <summary>
    /// 任务消息
    /// </summary>
    public class JobMessageUtil
    {
        /// <summary>
        /// 从新浪同步数据
        /// </summary>
        //开始事件
        public event Action InStockDailyTradeDataFromSinaJobStarted;
        public void OnInStockDailyTradeDataFromSinaJobStarted()
        {
            InStockDailyTradeDataFromSinaJobStarted?.Invoke();
        }
        //结束事件
        public event Action InStockDailyTradeDataFromSinaJobCompleted;
        public void OnInStockDailyTradeDataFromSinaJobCompleted()
        {
            InStockDailyTradeDataFromSinaJobCompleted?.Invoke();
        }



        /// <summary>
        /// 每日交易数据
        /// </summary>
        public event Action InStockDailyTradeDataJobStarted;
        public void OnInStockDailyTradeDataJobStarted()
        {
            InStockDailyTradeDataJobStarted?.Invoke();
        }
        //结束事件
        public event Action InStockDailyTradeDataJobCompleted;
        public void OnInStockDailyTradeDataJobCompleted()
        {
            InStockDailyTradeDataJobCompleted?.Invoke();
        }



        /// <summary>
        /// 同花顺指数每日交易数据
        /// </summary>
        public event Action InTHSIndexDailyTradeDataJobStarted;
        public void OnInTHSIndexDailyTradeDataJobStarted()
        {
            InTHSIndexDailyTradeDataJobStarted?.Invoke();
        }
        //结束事件
        public event Action InTHSIndexDailyTradeDataJobCompleted;
        public void OnInTHSIndexDailyTradeDataJobCompleted()
        {
            InTHSIndexDailyTradeDataJobCompleted?.Invoke();
        }

        /// <summary>
        /// 每日指标
        /// </summary>
        public event Action InStockDailyIndicatorJobStarted;
        public void OnInStockDailyIndicatorJobStarted()
        {
            InStockDailyIndicatorJobStarted?.Invoke();
        }
        //结束事件
        public event Action InStockDailyIndicatorJobCompleted;
        public void OnInStockDailyIndicatorJobCompleted()
        {
            InStockDailyIndicatorJobCompleted?.Invoke();
        }


        /// <summary>
        /// 同步换手率
        /// </summary>
        public event Action InSyncTrunoverJobStarted;
        public void OnInSyncTrunoverJobStarted()
        {
            InSyncTrunoverJobStarted?.Invoke();
        }
        //结束事件
        public event Action InSyncTrunoverJobCompleted;
        public void OnInSyncTrunoverJobCompleted()
        {
            InSyncTrunoverJobCompleted?.Invoke();
        }


        /// <summary>
        /// 日线数据
        /// </summary>
        public event Action<string> StockDailyTradeDataJobProgressChanged;
        public void OnStockDailyTradeDataJobProgressChanged(string msg)
        {
            StockDailyTradeDataJobProgressChanged?.Invoke(msg);
        }

        /// <summary>
        /// 同花顺指数
        /// </summary>
        public event Action<string> THSIndexDailyTradeDataJobProgressChanged;
        public void OnTHSIndexDailyTradeDataJobProgressChanged(string msg)
        {
            THSIndexDailyTradeDataJobProgressChanged?.Invoke(msg);
        }

       /// <summary>
       /// 每日指标
       /// </summary>
        public event Action<string> StockDailyIndicatorJobProgressChanged;
        public void OnStockDailyIndicatorJobProgressChanged(string msg)
        {
            StockDailyIndicatorJobProgressChanged?.Invoke(msg);
        }

        /// <summary>
        /// 换手率
        /// </summary>
        public event Action<string> InSyncTrunoverJobProgressChanged;
        public void OnInSyncTrunoverJobProgressChanged(string msg)
        {
            InSyncTrunoverJobProgressChanged?.Invoke(msg);
        }
    }

}
