using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Utils
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
