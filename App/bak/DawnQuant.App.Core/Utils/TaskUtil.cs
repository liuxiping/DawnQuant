using DawnQuant.App.Core.Job;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;


namespace DawnQuant.App.Core.Utils
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
        public static void StartStrategyScheduledTask(IServiceProvider serviceProvider)
        {
                //获取策略任务
                long userId = serviceProvider.GetService<IPassportProvider>().UserId;
                var strategyScheduledTaskService = serviceProvider.GetService<StrategyScheduledTaskService>();
                var tasks = strategyScheduledTaskService.GetStrategyScheduledTasksByUserId(userId);

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
