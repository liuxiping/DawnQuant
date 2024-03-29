﻿

namespace DawnQuant.DataCollector.Core.Config
{
    /// <summary>
    /// 采集器参数
    /// </summary>
    public class CollectorConfig
    {

        /// <summary>
        /// tushare Token
        /// </summary>
        public string TushareToken { get; set; }
        /// <summary>
        /// TushareUrl
        /// </summary>
        public string TushareUrl { get; set; }

        /// <summary>
        /// 日线数据采集线程数目
        /// </summary>
        public int DailyStockTradeDataCollectorThreadCount { get; set; }
        /// <summary>
        /// 每日指标数据采集线程数目
        /// </summary>
        public int  StockDailyIndicatorCollectorThreadCount { get; set; }

        /// <summary>
        /// 行业采集线程
        /// </summary>
        public int IndustryCollectorThreadCount { get; set; }

        /// <summary>
        /// A股市场采集数据上传API
        /// </summary>
        public string AShareApiUrl { get; set; }


        /// <summary>
        /// 资源认证服务器Url
        /// </summary>
        public string IdentityUrl { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 权限作用域
        /// </summary>
        public string Scope { get; set; }


        /// <summary>
        /// 每日增量交易数据指标定时时间
        /// </summary>
       // public string DailyTradeDataTaskCron { get; set; }
        public string InDTDFromSinaTaskCron { get; set; }

        public string InDataFromTushareTaskCron { get; set; }

        /// <summary>
        /// 每日增量m每日指标定时时间
        /// </summary>
        //  public string StockDailyIndicatorTaskCron { get; set; }


        /// <summary>
        /// 增量同步换手率定时时间
        /// </summary>
        //   public string InSyncTurnoverTaskCron { get; set; }



        //龙头股判定关键词
        public List<string> BellwetherKeyWords { get; set; }
    }
}
