
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
        public event Action InDailyTradeDataFromSinaJobStarted;
        public void OnInDailyTradeDataFromSinaJobStarted()
        {
            InDailyTradeDataFromSinaJobStarted?.Invoke();
        }
        //结束事件
        public event Action InDailyTradeDataFromSinaJobCompleted;
        public void OnInDailyTradeDataFromSinaJobCompleted()
        {
            InDailyTradeDataFromSinaJobCompleted?.Invoke();
        }



        /// <summary>
        /// 每日交易数据
        /// </summary>
        public event Action InDailyTradeDataJobStarted;
        public void OnInDailyTradeDataJobStarted()
        {
            InDailyTradeDataJobStarted?.Invoke();
        }
        //结束事件
        public event Action InDailyTradeDataJobCompleted;
        public void OnInDailyTradeDataJobCompleted()
        {
            InDailyTradeDataJobCompleted?.Invoke();
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


        public event Action<string> DailyTradeDataJobProgressChanged;
        public void OnDailyTradeDataJobProgressChanged(string msg)
        {
            DailyTradeDataJobProgressChanged?.Invoke(msg);
        }


        public event Action<string> StockDailyIndicatorJobProgressChanged;
        public void OnStockDailyIndicatorJobProgressChanged(string msg)
        {
            StockDailyIndicatorJobProgressChanged?.Invoke(msg);
        }

        public event Action<string> InSyncTrunoverJobProgressChanged;
        public void OnInSyncTrunoverJobProgressChanged(string msg)
        {
            InSyncTrunoverJobProgressChanged?.Invoke(msg);
        }
    }

}
