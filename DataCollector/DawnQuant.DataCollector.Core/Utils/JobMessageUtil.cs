
namespace DawnQuant.DataCollector.Core.Utils
{
    /// <summary>
    /// 任务消息
    /// </summary>
    public class JobMessageUtil
    {

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
    }

}
