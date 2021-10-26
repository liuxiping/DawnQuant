using Autofac;
using AutoMapper.Configuration;
using DawnQuant.DataCollector.Config;
using DawnQuant.DataCollector.Job;
using DawnQuant.DataCollector.Utils;
using DevExpress.Mvvm;
using Quartz;


namespace DawnQuant.DataCollector.ViewModels.AShare
{
    public class IncrementDataViewModel : ViewModelBase
    {

        public IncrementDataViewModel()
        {
            using (var scope = IOCUtil.Container.BeginLifetimeScope())
            {
                var config = scope.Resolve<CollectorConfig>();
                _dailyTradeDataTaskCron = config.DailyTradeDataTaskCron;
                _stockDailyIndicatorTaskCron = config.StockDailyIndicatorTaskCron;
                _230dailyTradeDataTaskCron = config.DailyTradeDataTask230Cron;

                _jobMessageUtil = scope.Resolve<JobMessageUtil>();

                _jobMessageUtil.DailyTradeDataJobProgressChanged += _jobMessageUtility_DailyTradeDataJobProgressChanged;
                _jobMessageUtil.StockDailyIndicatorJobProgressChanged += _jobMessageUtility_StockDailyIndicatorJobProgressChanged;
               
            };

            StartIDTDTaskCommand = new DelegateCommand(StartIDTDTask, () =>
             {
                 return !isStartInDailyTradeData;
             });

            StartI230DTDTaskCommand = new DelegateCommand(StartI230DTDTask, () => 
            {
                return !isStartIn230DailyTradeData;
            });

            StartIDITaskCommand = new DelegateCommand(StartIDITask, () =>
               {
                   return !isStartInStockDailyIndicator;
               });

            ClearMessageCommand = new DelegateCommand(() =>
            {
                Message = "";
            });

            StartAllCommand = new DelegateCommand(StartAllTask);
        }

        private void _jobMessageUtility_StockDailyIndicatorJobProgressChanged(string msg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                StockDailyIndicatorProgress = msg;
            });
           
        }

        private void _jobMessageUtility_DailyTradeDataJobProgressChanged(string msg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                DailyTradeDataProgress = msg; 
            });
          
        }

        string _dailyTradeDataTaskCron;
        string _230dailyTradeDataTaskCron;
        string _stockDailyIndicatorTaskCron;

        bool isStartInDailyTradeData = false;
        bool isStartInStockDailyIndicator = false;
        bool isStartIn230DailyTradeData=false;

        JobMessageUtil _jobMessageUtil;


        /// <summary>
        /// 运行时消息
        /// </summary>
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value, nameof(Message)); }
        }

        /// <summary>
        /// 采集增量日线历史数据进度
        /// </summary>
        private string _dailyTradeDataProgress;
        public string DailyTradeDataProgress
        {
            get { return _dailyTradeDataProgress; }
            set { SetProperty(ref _dailyTradeDataProgress, value, nameof(DailyTradeDataProgress)); }
        }


        /// <summary>
        /// 采集增量每日指标数据进度
        /// </summary>
        private string _stockDailyIndicatorProgress;
        public string StockDailyIndicatorProgress
        {
            get { return _stockDailyIndicatorProgress; }
            set
            {
                SetProperty(ref _stockDailyIndicatorProgress, value,
              nameof(StockDailyIndicatorProgress));
            }
        }


        /// <summary>
        /// 增量更新每日日线交易数据
        /// </summary>
        public DelegateCommand StartIDTDTaskCommand { set; get; }
        private void StartIDTDTask()
        {
            isStartInDailyTradeData = true;
            StartIDTDTaskCommand.RaiseCanExecuteChanged();

            //任务
            IJobDetail job = JobBuilder.Create<InDailyTradeDataJob>()
                .WithIdentity("InDailyTradeDataJob").Build();

            ITrigger trigger = TriggerBuilder.Create().
              WithCronSchedule(_dailyTradeDataTaskCron).StartNow().Build();

           // ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(b =>
           // { b.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(0); }).StartNow().Build();

            TaskUtil.Scheduler.ScheduleJob(job, trigger);

        }



        /// <summary>
        /// 增量更新每日指标数据
        /// </summary>
        public DelegateCommand StartIDITaskCommand { set; get; }
        private void StartIDITask()
        {
            isStartInStockDailyIndicator = true;

            StartIDITaskCommand.RaiseCanExecuteChanged();
            IJobDetail job = JobBuilder.Create<InStockDailyIndicatorJob>()
               .WithIdentity("InStockDailyIndicatorJob").Build();

             ITrigger trigger = TriggerBuilder.Create().
             WithCronSchedule(_stockDailyIndicatorTaskCron).StartNow().Build();

           // ITrigger trigger = TriggerBuilder.Create().WithSimpleSchedule(b =>
           // { b.WithInterval(TimeSpan.FromSeconds(3)).WithRepeatCount(0); }).StartNow().Build();

            TaskUtil.Scheduler.ScheduleJob(job, trigger);
        }


        /// <summary>
        /// 交易日每日2:30 更新实时数据
        /// </summary>
        public DelegateCommand StartI230DTDTaskCommand { set; get; }
        private void StartI230DTDTask()
        {
            isStartIn230DailyTradeData = true;
            StartI230DTDTaskCommand.RaiseCanExecuteChanged();

            //任务
            IJobDetail job = JobBuilder.Create<In230DailyTradeDataJob>()
                .WithIdentity("In230DailyTradeDataJob").Build();

            ITrigger trigger = TriggerBuilder.Create().
              WithCronSchedule(_230dailyTradeDataTaskCron).StartNow().Build();
            TaskUtil.Scheduler.ScheduleJob(job, trigger);
        }

        public DelegateCommand ClearMessageCommand { set; get; }


        public DelegateCommand StartAllCommand { set; get; }
        private void StartAllTask()
        {

        }
    }
}
