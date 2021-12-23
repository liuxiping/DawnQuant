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
    /// <summary>
    /// 从tushre 增量获取数据
    /// </summary>
    internal class InDataFromTushareJob : IJob
    {


        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                IServiceProvider serviceProvider = (IServiceProvider)context.MergedJobDataMap.Get(Constant.ServiceProvider);

                var t1 = Task.Run(() => { CollectInDailyTradeData(serviceProvider); });
                var t2 = Task.Run(() => { CollectInStockDailyIndicator(serviceProvider); });

                Task.WaitAll(t1, t2);

                InSyncTrunover(serviceProvider);

            });
        }


        /// <summary>
        /// 采集增量日线数据
        /// </summary>
        /// <param name="serviceProvider"></param>
        private  void CollectInDailyTradeData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInDailyTradeDataJobStarted();
                     collector.CollectInDailyTradeDataFromTushare(DateTime.Now);
                    jobMessageUtility.OnInDailyTradeDataJobCompleted();
                }
            }

        }


        /// <summary>
        /// 采集每日指标
        /// </summary>
        /// <param name="serviceProvider"></param>
        private void CollectInStockDailyIndicator(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInStockDailyIndicatorJobStarted();
                    collector.CollectInStockDailyIndicatorFromTushare(DateTime.Now);
                    jobMessageUtility.OnInStockDailyIndicatorJobCompleted();
                }
            }
        }


        /// <summary>
        /// 同步换手率
        /// </summary>
        /// <param name="serviceProvider"></param>
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
