using Autofac;
using DawnQuant.App.Job;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;
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
        /// 客户端定时执行策略任务
        /// </summary>
        public static void StartStrategyScheduledTask()
        {

            //获取策略任务
            long userId = IOCUtil.Container.Resolve<IPassportProvider>().UserId;
            var logger = IOCUtil.Container.Resolve<ILogger>();

            var strategyScheduledTaskService = IOCUtil.Container.Resolve<StrategyScheduledTaskService>();
            var tasks = strategyScheduledTaskService.GetStrategyScheduledTasksByUserId(userId);

            if (tasks != null && tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    if (task.IsJoinClientScheduleTask)
                    {
                        try
                        {
                            JobDataMap jobDataMap = new JobDataMap();
                            jobDataMap.Add(new KeyValuePair<string, object>("StrategyScheduledTask", task));

                            //任务
                            IJobDetail job = JobBuilder.Create<StrategyScheduledTaskJob>()
                                .WithIdentity("StrategyScheduledTaskJob").SetJobData(jobDataMap)
                                .Build();

                            string cron = task.ClientScheduleCron;
                            ITrigger trigger = TriggerBuilder.Create().
                              WithCronSchedule(cron).StartNow().Build();

                            Scheduler.ScheduleJob(job, trigger);
                        }
                        catch (Exception ex)
                        {
                            string message = $"启动{task.Name}定时任务失败。\r\n 错误信息：{ex.Message}\r\n{ex.StackTrace}";
                            logger.LogError(message);
                        }
                    }
                }

            }

        }


        /// <summary>
        /// 定时自动更新交易数据
        /// </summary>
        public static void StartDataUpdateScheduledTask()
        {
            if (App.AShareSetting != null && App.AShareSetting.DataUpdateSetting != null)
            {
                var u = App.AShareSetting.DataUpdateSetting;

                if (string.IsNullOrEmpty(u.TaskCron))
                {
                    var logger = IOCUtil.Container.Resolve<ILogger>();
                    try
                    {
                        //任务
                        IJobDetail job = JobBuilder.Create<DataUpdateScheduledTaskJob>()
                            .WithIdentity("DataUpdateScheduledTaskJob").Build();

                        ITrigger trigger = TriggerBuilder.Create().
                          WithCronSchedule(u.TaskCron).StartNow().Build();
                        Scheduler.ScheduleJob(job, trigger);
                    }
                    catch (Exception ex)
                    {
                        string message = $"启动自动更新定时任务失败。\r\n 错误信息：{ex.Message}\r\n{ex.StackTrace}";
                        logger.LogError(message);
                    }
                }
            }

        }
    }
}
