

namespace DawnQuant.App.Core.Models.AShare.EssentialData
{
    /// <summary>
    /// 当前选择的股票基本信息
    /// </summary>
    public class CurSelectStockInfo: StockPlotData
    {

        public KCycle KCycle { get; set; } 

        public double TurnOver { get; set; }

        public double TurnOverFree { get; set; }
    }
}
