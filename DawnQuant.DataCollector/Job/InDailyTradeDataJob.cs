using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.DataCollector.Collectors.AShare;
using DawnQuant.DataCollector.Utils;
using Quartz;


namespace DawnQuant.DataCollector.Job
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
                CollectInDailyTradeData();

            });
        }


        private void CollectInDailyTradeData()
        {
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {

               IncrementalDataCollector collector = scope.Resolve<IncrementalDataCollector>();

                var jobMessageUtility = scope.Resolve<JobMessageUtil>();

                collector.CollectInDTDProgressChanged += (msg)=>
                {
                    jobMessageUtility.OnDailyTradeDataJobProgressChanged(msg);
                };

                collector.CollectIncrementDailyTradeData(DateTime.Now);

            }

        }
    }
}
