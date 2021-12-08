using Autofac;
using DawnQuant.App.Job;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    /// <summary>
    /// 定时任务调度
    /// </summary>
    public class TaskUtil
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


        /// <summary>
        /// 开启定时任务
        /// </summary>
        public static void StartStrategyScheduledTask()
        {

            //获取策略任务
            long userId = IOCUtil.Container.Resolve<IPassportProvider>().UserId;
            var _strategyScheduledTaskService = IOCUtil.Container.Resolve<StrategyScheduledTaskService>();
            var tasks = _strategyScheduledTaskService.GetStrategyScheduledTasksByUserId(userId);

            if (tasks != null && tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    if (task.IsJoinClientScheduleTask)
                    {
                        JobDataMap jobDataMap = new JobDataMap();
                        jobDataMap.Add(new KeyValuePair<string, object>("StrategyScheduledTask", task));

                        //任务
                        IJobDetail job = JobBuilder.Create<StrategyScheduledTaskJob>()
                            .WithIdentity("StrategyScheduledTaskJob").SetJobData(jobDataMap)
                            .Build();

                        string cron = task.ClientScheduleTime.Value.Second + " " +
                            task.ClientScheduleTime.Value.Minute + " " +
                            task.ClientScheduleTime.Value.Hour + " " + @"? * MON-FRI";
                        ITrigger trigger = TriggerBuilder.Create().
                          WithCronSchedule(cron).StartNow().Build();

                        Scheduler.ScheduleJob(job, trigger);
                    }
                }

            }

        }
    }
}
