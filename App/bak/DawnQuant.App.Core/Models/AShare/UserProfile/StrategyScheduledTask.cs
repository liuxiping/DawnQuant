﻿

namespace DawnQuant.App.Core.Models.AShare.UserProfile
{
    /// <summary>
    /// 策略任务计划
    /// </summary>
    public class StrategyScheduledTask
    {
       
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 策略ID 中间以逗号分隔
        /// </summary>
        public string StrategyIds { get; set; }

        /// <summary>
        /// 输出自定义分类
        /// </summary>
        public long OutputStockCategoryId { get; set; }

        /// <summary>
        /// 策略计划任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 最近执行时间
        /// </summary>
        public DateTime? LatestExecuteTime { get; set; }


        /// <summary>
        /// 是否加入服务器服务器执行计划
        /// </summary>
        public bool IsJoinServerScheduleTask { get; set; }


        /// <summary>
        /// 是否加入客户端服务器执行计划
        /// </summary>
        public bool IsJoinClientScheduleTask { get; set; }

        /// <summary>
        /// 客户端定时执行时间
        /// </summary>
        public DateTime? ClientScheduleTime { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        private bool _isExecuting = false;
        public bool IsExecuting
        {
            get { return _isExecuting; }
            set { _isExecuting = value; }
        }
    }
}
