using Quartz;
using Quartz.Impl;

namespace DawnQuant.DataCollector.Core.sUtils
{
    /// <summary>
    /// 定时任务调度
    /// </summary>
    public static class TaskUtil
    {
        public static IScheduler Scheduler;

        static TaskUtil()
        {
            //初始化调度器
            var task = StdSchedulerFactory.GetDefaultScheduler();
            task.Wait();
            Scheduler = task.Result;
            Scheduler.Start();
            

        }
    }
}
