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

                var t1 = CollectInDailyTradeData(serviceProvider);
                var t2 = CollectInStockDailyIndicator(serviceProvider);
                var t3 = CollectInTHSIndexDailyTradeData(serviceProvider);

                Task.WaitAll(t1, t2);

                InSyncTrunover(serviceProvider);

            });
        }


        /// <summary>
        /// 采集增量日线数据
        /// </summary>
        /// <param name="serviceProvider"></param>
        private  async Task CollectInDailyTradeData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInStockDailyTradeDataJobStarted();
                    await collector.CollectInDailyTradeDataFromTushare(DateTime.Now);
                    jobMessageUtility.OnInStockDailyTradeDataJobCompleted();
                }
            }

        }



        /// <summary>
        /// 采集增量同花顺指数数据
        /// </summary>
        /// <param name="serviceProvider"></param>
        private async Task CollectInTHSIndexDailyTradeData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInTHSIndexDailyTradeDataJobStarted();
                    await collector.CollectInTHSIndexDailyTradeDataFromTushareAsync(DateTime.Now);
                    jobMessageUtility.OnInTHSIndexDailyTradeDataJobCompleted();
                }
            }

        }


        /// <summary>
        /// 采集每日指标
        /// </summary>
        /// <param name="serviceProvider"></param>
        private async Task CollectInStockDailyIndicator(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                IncrementalDataCollector collector = scope.ServiceProvider.GetService<IncrementalDataCollector>();

                if (collector.IsOpen())
                {
                    var jobMessageUtility = scope.ServiceProvider.GetService<JobMessageUtil>();

                    jobMessageUtility.OnInStockDailyIndicatorJobStarted();
                    await collector.CollectInStockDailyIndicatorFromTushare(DateTime.Now);
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
