using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.DataCollector.Utils;
using DawnQuant.DataCollector.Collectors.AShare;

namespace DawnQuant.DataCollector.Job
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
                CollectInStockDailyIndicator();
            });
        }

        private void CollectInStockDailyIndicator()
        {
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                IncrementalDataCollector collector = scope.Resolve<IncrementalDataCollector>();


                var jobMessageUtility = scope.Resolve<JobMessageUtil>();

                collector.CollectInDIProgressChanged += (msg) =>
                {
                    jobMessageUtility.OnStockDailyIndicatorJobProgressChanged(msg);
                };

                collector.CollectIncrementStockDailyIndicator(DateTime.Now);
              //  collector.CollectIncrementStockDailyIndicator(DateTime.Now.AddDays(-1));


            }
        }
    }
}
