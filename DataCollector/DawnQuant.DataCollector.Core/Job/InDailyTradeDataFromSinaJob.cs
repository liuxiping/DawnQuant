using Quartz;
using DawnQuant.DataCollector.Core.Collectors.AShare;
using DawnQuant.DataCollector.Core.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DawnQuant.DataCollector.Core.Job
{
    /// <summary>
    /// 从新浪每隔15分钟采集数据
    /// </summary>
    public class InDailyTradeDataFromSinaJob : IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {

                IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);



               await CollectInDailyTradeDataFromSina(serviceProvider);

            });
        }


        private  async Task CollectInDailyTradeDataFromSina(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var now = DateTime.Now;

                    var s1 = new DateTime(now.Year, now.Month, now.Day, 9, 30, 0);
                    var e1 = new DateTime(now.Year, now.Month, now.Day, 11, 40, 0);

                    var s2 = new DateTime(now.Year, now.Month, now.Day, 13, 0, 0);
                    var e2 = new DateTime(now.Year, now.Month, now.Day, 15, 10, 0);

                    if ((now > s1 && now < e1) || (now > s2 && now < e2))
                    {
                        var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                        jobMessageUtility.OnInDailyTradeDataFromSinaJobStarted();
                        await collector.CollectInDailyTradeDataFromSina(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                        jobMessageUtility.OnInDailyTradeDataFromSinaJobCompleted();

                    }
                }
            }
        }
    }
}
