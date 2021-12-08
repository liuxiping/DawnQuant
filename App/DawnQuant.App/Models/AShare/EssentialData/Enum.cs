using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{

    /// <summary>
    /// K线周期
    /// </summary>
    public enum KCycle
    {
        Minute1 = 0,

        Minute5 = 1,

        Minute15 = 2,

        Minute30 = 3,

        Minute60 = 4,

        Minute120 = 5,
       
        Day = 6,

        Week = 7,

        Month = 8,

        Year = 9,

        Other = 10

    }

    /// <summary>
    /// 市场类型 （主板/中小板/创业板/科创板/CDR）
    /// </summary>
    public enum MarketType
    {
        MainBoard=0,
        SmallAndMediumSizedBoard=1,
        GEMBoard=2,
        STARBoard=3,
        CDRBoard=4
    }


    /// <summary>
    /// 上市状态
    /// </summary>
    public enum ListedState
    {
        Listed=0,
        Delisted=1,
        Suspend=2

    }

    /// <summary>
    /// 数据状态 维护全(原始数据) 前复权 后复权
    /// </summary>
   public enum AdjustedState
    {
        /// <summary>
        /// 未复权
        /// </summary>
        None=0,

        /// <summary>
        /// 前复权
        /// </summary>
        Pre=1,

        /// <summary>
        /// 后复权
        /// </summary>
        After=2
    }
}
