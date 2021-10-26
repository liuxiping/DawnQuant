using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.DataCollector.Collectors.AShare;
using DawnQuant.DataCollector.Utils;
using Autofac;


namespace DawnQuant.DataCollector.Job
{
    /// <summary>
    /// 交易日2:30 从新浪实时采集交易日线数据
    /// </summary>
    public class In230DailyTradeDataJob: IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                CollectIn230DailyTradeData();

            });
        }


        private void CollectIn230DailyTradeData()
        {
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {

                IncrementalDataCollector collector = scope.Resolve<IncrementalDataCollector>();

                var jobMessageUtility = scope.Resolve<JobMessageUtil>();

                collector.CollectIn230DTDProgressChanged += (msg) =>
                {
                    jobMessageUtility.OnDailyTradeDataJobProgressChanged(msg);
                };

                collector.CollectIncrement230DailyTradeData(new DateTime (DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day)) ;

            }

        }
    }
}
