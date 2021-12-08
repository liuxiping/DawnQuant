using DawnQuant.App.Models.AShare.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    /// <summary>
    /// 任务消息
    /// </summary>
    public class JobMessageUtil
    {

        /// <summary>
        /// 客户端定时执行策略计划完成
        /// </summary>
        public event Action<StrategyScheduledTask> StrategyScheduledTaskCompleted;
        public void OnStrategyScheduledTaskCompleted(StrategyScheduledTask task)
        {
            StrategyScheduledTaskCompleted?.Invoke(task);
        }


       
    }

}
