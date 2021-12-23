using DawnQuant.DataCollector.Core.Config;
using DawnQuant.DataCollector.Core.Job;
using DawnQuant.DataCollector.Core.sUtils;
using DawnQuant.DataCollector.Core.Utils;
using Quartz;


namespace DawnQuant.DataCollector.Core.ViewModels.AShare
{
    public class IncrementDataViewModel
    {


        public IncrementDataViewModel(IServiceProvider serviceProvider,
            CollectorConfig collectorConfig, JobMessageUtil jobMessageUtil)
        {
            _serviceProvider = serviceProvider;
            _collectorConfig = collectorConfig;
            _jobMessageUtil = jobMessageUtil;


            _jobMessageUtil.DailyTradeDataJobProgressChanged += _jobMessageUtility_DailyTradeDataJobProgressChanged;
            _jobMessageUtil.StockDailyIndicatorJobProgressChanged += _jobMessageUtility_StockDailyIndicatorJobProgressChanged;
            _jobMessageUtil.InSyncTrunoverJobProgressChanged += _jobMessageUtil_InSyncTrunoverJobProgressChanged;

            _jobMessageUtil.InDailyTradeDataFromSinaJobStarted += _jobMessageUtil_InDailyTradeDataFromSinaJobStarted;
            _jobMessageUtil.InDailyTradeDataFromSinaJobCompleted += _jobMessageUtil_InDailyTradeDataFromSinaJobCompleted;

            _jobMessageUtil.InDailyTradeDataJobStarted += _jobMessageUtil_InDailyTradeDataJobStarted;
            _jobMessageUtil.InDailyTradeDataJobCompleted += JobMessageUtil_InDailyTradeDataJobCompleted;

            _jobMessageUtil.InStockDailyIndicatorJobStarted += _jobMessageUtil_InStockDailyIndicatorJobStarted;
            _jobMessageUtil.InStockDailyIndicatorJobCompleted += _jobMessageUtil_InStockDailyIndicatorJobCompleted;


            _jobMessageUtil.InSyncTrunoverJobStarted += _jobMessageUtil_InSyncTrunoverJobStarted;
            _jobMessageUtil.InSyncTrunoverJobCompleted += _jobMessageUtil_InSyncTrunoverJobCompleted;
        }

        private void _jobMessageUtil_InSyncTrunoverJobProgressChanged(string msg)
        {
            InSyncTurnoverProgress = msg;
            OnViewNeedUpdate();

        }

        private void _jobMessageUtil_InSyncTrunoverJobCompleted()
        {
            lock (this)
            {
                Message += $"同步换手率已成功完成，{DateTime.Now}\r\n";
                OnViewNeedUpdate();
            }
        }

        private void _jobMessageUtil_InSyncTrunoverJobStarted()
        {
            lock (this)
            {
                Message += $"开始同步换手率数据，{DateTime.Now}\r\n";
                OnViewNeedUpdate();
            }
        }

        private void _jobMessageUtil_InStockDailyIndicatorJobCompleted()
        {
            lock (this)
            {
                Message += $"增量每日指标数据已成功采集，{DateTime.Now}\r\n";
                OnViewNeedUpdate();
            }
        }

        private void _jobMessageUtil_InStockDailyIndicatorJobStarted()
        {
            lock (this)
            {
                Message += $"开始采集增量每日指标数据，{DateTime.Now}\r\n";
                OnViewNeedUpdate();
            }
        }

        private void JobMessageUtil_InDailyTradeDataJobCompleted()
        {
            lock (this)
            {
                Message += $"增量每日日线数据已成功采集，{DateTime.Now} \r\n";
                OnViewNeedUpdate();
            }
        }

        private void _jobMessageUtil_InDailyTradeDataJobStarted()
        {
            lock (this)
            {
                Message += $"开始采集增量每日日线数据，{DateTime.Now} \r\n";
                OnViewNeedUpdate();
            }

        }

        private void _jobMessageUtil_InDailyTradeDataFromSinaJobCompleted()
        {
            lock (this)
            {
                Message += $"从新浪更新增量每日日线数据成功，{DateTime.Now} \r\n";
                OnViewNeedUpdate();
            }
        }

        private void _jobMessageUtil_InDailyTradeDataFromSinaJobStarted()
        {
            lock (this)
            {
                Message += $"从新浪开始更新增量每日日线数据，{DateTime.Now} \r\n";
                OnViewNeedUpdate();
            }
        }

        IServiceProvider _serviceProvider;
        CollectorConfig _collectorConfig;
        JobMessageUtil _jobMessageUtil;

        #region
        //通知更新视图
        public event Action ViewNeedUpdate;

        protected  void OnViewNeedUpdate()
        {
            ViewNeedUpdate?.Invoke();
        }

       
        #endregion

        private void _jobMessageUtility_StockDailyIndicatorJobProgressChanged(string msg)
        {
            StockDailyIndicatorProgress = msg;
            OnViewNeedUpdate();
        }

        private void _jobMessageUtility_DailyTradeDataJobProgressChanged(string msg)
        {
            DailyTradeDataProgress = msg;
            OnViewNeedUpdate();
        }



        public bool IsInDataFromTushare { set; get; } = false;
        public bool IsStartInDailyTradeDataFromSina { set; get; } = false;
        public bool IsStartAllTask { set; get; } = false;

      


        /// <summary>
        /// 运行时消息
        /// </summary>
        public string Message { set; get; }
       


        /// <summary>
        /// 采集增量日线历史数据进度
        /// </summary>
        public string DailyTradeDataProgress { set; get; }


        /// <summary>
        /// 同步换手率
        /// </summary>
        public string InSyncTurnoverProgress { set; get; }


        /// <summary>
        /// 采集增量每日指标数据进度
        /// </summary>
        public string StockDailyIndicatorProgress { set; get; }



        /// <summary>
        /// 从Tushare 增量更新数据
        /// </summary>
        public void StartInDataFromTushareTask()
        {
            IsInDataFromTushare = true;

            //任务
            IJobDetail job = JobBuilder.Create<InDataFromTushareJob>()
                .WithIdentity("InDataFromTushareJob").Build();

            job.JobDataMap.Add(Constant.ServiceProvider, _serviceProvider);

            ITrigger trigger = TriggerBuilder.Create().
              WithCronSchedule(_collectorConfig.InDataFromTushareTaskCron).StartNow().Build();

           
            TaskUtil.Scheduler.ScheduleJob(job, trigger);

            Message += $"从Tushare增量更新数据任务计划已经启动，{DateTime.Now.ToString()}\r\n";

            SetIsStartAllTask();
        }


        /// <summary>
        /// 交易日从新浪更新数据
        /// </summary>
        public void StartIDTDFromSinaTask()
        {
            IsStartInDailyTradeDataFromSina = true;

            //任务
            IJobDetail job = JobBuilder.Create<InDailyTradeDataFromSinaJob>()
                .WithIdentity("InDailyTradeDataFromSinaJob").Build();
            job.JobDataMap.Add(Constant.ServiceProvider, _serviceProvider);

            ITrigger trigger = TriggerBuilder.Create().
              WithCronSchedule(_collectorConfig.InDTDFromSinaTaskCron).StartNow().Build();

            TaskUtil.Scheduler.ScheduleJob(job, trigger);
            Message += $"从新浪增量更新日线交易数据任务计划已经启动，{DateTime.Now.ToString()}\r\n";

            SetIsStartAllTask();
        }



        public void StartAllTask()
        {
            IsStartAllTask = true;

            if (!IsStartInDailyTradeDataFromSina)
            {
                StartIDTDFromSinaTask();
            }
           

            if (!IsInDataFromTushare)
            {
                StartInDataFromTushareTask();
            }

            IsStartInDailyTradeDataFromSina = true;
            IsInDataFromTushare = true;
           
        }

       

        private void SetIsStartAllTask()
        {
            if (IsStartInDailyTradeDataFromSina && 
                IsInDataFromTushare )
            {
                IsStartAllTask = true;
            }

        }
    }
}
