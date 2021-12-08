using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.DataCollector.Core;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace DawnQuant.App.Core.Job
{
    /// <summary>
    /// /定时执行策略计划
    /// </summary>
    public class StrategyScheduledTaskJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {

            IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);

            //调用策略

            return Task.Run(() =>
            {
                StrategyScheduledTask task = (StrategyScheduledTask)context.JobDetail.JobDataMap.Get("StrategyScheduledTask");
                var _scheduledTaskService = serviceProvider.GetService<StrategyScheduledTaskService>();
                    //执行策略
                    _scheduledTaskService.ExecuteStrategyScheduledTask(task);
                    //通知策略执行执行完成
                    var jobc = serviceProvider.GetService<JobMessageUtil>();
                jobc.OnStrategyScheduledTaskCompleted(task);

            });
        }

    }
}

