using DawnQuant.DataCollector.Core.Collectors.AShare;
using DawnQuant.DataCollector.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Job
{
    internal class InSyncTrunoverJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);

                InSyncTrunover(serviceProvider);

            });
        }


        private async void InSyncTrunover(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInSyncTrunoverJobStarted();

                    await collector.InSyncTurnoverAsync();

                    jobMessageUtility.OnInSyncTrunoverJobCompleted();
                }
            }

        }
    }
}
