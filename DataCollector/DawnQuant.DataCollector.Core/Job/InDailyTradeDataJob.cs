using DawnQuant.DataCollector.Core.Collectors.AShare;
using DawnQuant.DataCollector.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Quartz;


namespace DawnQuant.DataCollector.Core.Job
{
    /// <summary>
    /// 执行采集每日增量数据的工作
    /// </summary>
    public class InDailyTradeDataJob : IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);
                CollectInDailyTradeData( serviceProvider);

            });
        }


        private  void CollectInDailyTradeData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {

                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    //collector.CollectInDTDProgressChanged += (msg) =>
                    //{
                    //    jobMessageUtility.OnDailyTradeDataJobProgressChanged(msg);
                    //};

                    jobMessageUtility.OnInDailyTradeDataJobStarted();
                     collector.CollectInDailyTradeDataFromTushare(DateTime.Now);
                    jobMessageUtility.OnInDailyTradeDataJobCompleted();
                }
            }

        }
    }
}
