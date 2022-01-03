using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    public class HammerFactorExecutorParameter
    {
     
        /// <summary>
        /// K线周期
        /// </summary>
        public KCycle KCycle { get; set; }

       
        /// <summary>
        /// 最近交易周期数据
        /// </summary>
        public int LookBackCycleCount { get; set; }

        /// <summary>
        /// 满足条件的次数
        /// </summary>
        public int MeetCount { get; set; }


        /// <summary>
        /// 最近一个交易日期是否满足
        /// </summary>
        public bool IsLatestTradeDateTimeMeet { get; set; }




    }
}
