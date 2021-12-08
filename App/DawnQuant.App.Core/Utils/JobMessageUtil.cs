using DawnQuant.App.Core.Models.AShare.UserProfile;

namespace DawnQuant.App.Core.Utils
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
