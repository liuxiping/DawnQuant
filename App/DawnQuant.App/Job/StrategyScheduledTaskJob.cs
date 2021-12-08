using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Job
{
    /// <summary>
    /// /定时执行策略计划
    /// </summary>
    public class StrategyScheduledTaskJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //调用策略
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                return Task.Run(() =>
                {
                    StrategyScheduledTask task = (StrategyScheduledTask)context.JobDetail.JobDataMap.Get("StrategyScheduledTask");
                    var _scheduledTaskService = IOCUtil.Container.Resolve<StrategyScheduledTaskService>();
                    //执行策略
                    _scheduledTaskService.ExecuteStrategyScheduledTask(task);
                    //通知策略执行执行完成
                    var jobc = IOCUtil.Container.Resolve<JobMessageUtil>();
                    jobc.OnStrategyScheduledTaskCompleted(task);

                });
            }

        }
    }
}
