using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{
    /// <summary>
    /// 当前选择的股票基本信息
    /// </summary>
    public class CurSelectStockInfo: PlotData
    {

        public KCycle KCycle { get; set; } 

        public double TurnOver { get; set; }

        public double TurnOverFree { get; set; }
    }
}
