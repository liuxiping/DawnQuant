using Quartz;
using DawnQuant.DataCollector.Core.Utils;
using DawnQuant.DataCollector.Core.Collectors.AShare;
using Microsoft.Extensions.DependencyInjection;

namespace DawnQuant.DataCollector.Core.Job
{
    /// <summary>
    /// 每日指标增量数据
    /// </summary>
    public class InStockDailyIndicatorJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);
                CollectInStockDailyIndicator(serviceProvider);
            });
        }

        private void CollectInStockDailyIndicator(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    //collector.CollectInDIProgressChanged += (msg) =>
                    //{
                    //    jobMessageUtility?.OnStockDailyIndicatorJobProgressChanged(msg);
                    //};

                    jobMessageUtility.OnInStockDailyIndicatorJobStarted();
                    collector.CollectIncrementStockDailyIndicator(DateTime.Now);
                    jobMessageUtility.OnInStockDailyIndicatorJobCompleted();
                }
            }
        }
    }
}
