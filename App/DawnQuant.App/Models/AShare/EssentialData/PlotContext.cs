using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{
    /// <summary>
    /// 绘图相关数据
    /// </summary>
    public class PlotContext
    {
        int _showCount = 120;

        public string TSCode { get; set; }

        public string Name { get; set; }

        public string Id { get { return TSCode.Substring(0, 6); } }


        /// <summary>
        /// 周期
        /// </summary>
        public KCycle KCycle { get; set; }


        /// <summary>
        /// 绘图数据
        /// </summary>
        public ObservableCollection<PlotData> PlotDatas { get; set; }


        //显示的均线
        public bool ShowMA5 { get; set; }
        public bool ShowMA10 { get; set; }
        public bool ShowMA20 { get; set; }
        public bool ShowMA30 { get; set; }
        public bool ShowMA60 { get; set; }
        public bool ShowMA120 { get; set; }
        public bool ShowMA250 { get; set; }

        /// <summary>
        /// 数据显示开始日期
        /// </summary>
        public string MinVisibleDate
        {
            get
            {
                if (PlotDatas != null && PlotDatas.Count > 0)
                {
                    if (PlotDatas.Count >= _showCount)
                    {
                        return PlotDatas[PlotDatas.Count - _showCount].FormatedTradeDateTime;
                    }
                    else
                    {
                        return PlotDatas[0].FormatedTradeDateTime;

                    }
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 需要多加的数据 以便计算均线
        /// </summary>
        /// <returns></returns>
        public int GetExtraDataSize()
        {
            int extraDataSize = 250;

            if (ShowMA250) extraDataSize = 250;
            else if(ShowMA120) extraDataSize = 120;
            else if (ShowMA60) extraDataSize = 60;
            else if (ShowMA30) extraDataSize = 30;
            else if (ShowMA20) extraDataSize = 20;
            else if (ShowMA10) extraDataSize = 10;
            else extraDataSize = 5;

            return extraDataSize;
        }
    }
}
