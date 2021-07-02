using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Utils
{
    /// <summary>
    /// 任务消息
    /// </summary>
    public class JobMessageUtil
    {

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
