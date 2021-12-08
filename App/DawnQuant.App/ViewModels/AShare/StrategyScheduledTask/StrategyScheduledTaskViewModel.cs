using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;


namespace DawnQuant.App.ViewModels.AShare.StrategyScheduledTask
{
    public class StrategyScheduledTaskViewModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService  _selfSelService;
        private readonly StrategyScheduledTaskService _scheduledTaskService;
        private readonly StockStrategyService _stockStrategyService;

        public StrategyScheduledTaskViewModel()
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
            _scheduledTaskService = IOCUtil.Container.Resolve<StrategyScheduledTaskService>();
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();


            SaveCommand = new DelegateCommand(Save);
            NewCommand = new DelegateCommand(New);
            DeleteCommand = new DelegateCommand(Delete);
            RefreshCommand = new DelegateCommand(Refresh);
            ExecuteTaskCommand = new DelegateCommand(ExecuteTask);

            CurSelStockStrategies = new ObservableCollection<UserProfile.StockStrategy>();

            LoadStrategyCategoriesAndSelectStockCategories();

            UpdateStrategyScheduledTaskStatus();
        }


        /// <summary>
        /// 加载任务计划列表和自选股列表
        /// </summary>
        public async void LoadStrategyCategoriesAndSelectStockCategories()
        {

            ObservableCollection<StockStrategyCategory> stockStrategyCategories = null;
            ObservableCollection<SelfSelectStockCategory> selfSelectStockCategories = null;

            await Task.Run(() =>
            {
                //初始化数据
                stockStrategyCategories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId);
                selfSelectStockCategories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);

            }).ConfigureAwait(true);

            StockStrategyCategories = stockStrategyCategories;
            SelfSelectStockCategorys = selfSelectStockCategories;
        }

        /// <summary>
        /// 所有策略分类
        /// </summary>
        public ObservableCollection<StockStrategyCategory> _stockStrategyCategories;
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories
        {
            get
            {
                return _stockStrategyCategories;
            }
            set
            {
                SetProperty(ref _stockStrategyCategories, value, nameof(StockStrategyCategories));
            }
        }
        /// <summary>
        /// 

        //所有自选分类
        ObservableCollection<SelfSelectStockCategory> _selfSelectStockCategorys;
        public ObservableCollection<SelfSelectStockCategory> SelfSelectStockCategorys
        {
            set
            { SetProperty(ref _selfSelectStockCategorys, value, nameof(SelfSelectStockCategorys)); }
            get
            { return _selfSelectStockCategorys; }
        }


        /// <summary>
        /// 计划任务名称
        /// </summary>
        private string _scheduledTaskName;
        public string ScheduledTaskName
        {
            get { return _scheduledTaskName; }
            set { SetProperty(ref _scheduledTaskName, value, nameof(ScheduledTaskName)); }
        }

        /// <summary>
        /// 当前的策略
        /// </summary>
        ObservableCollection<UserProfile.StockStrategy> _curSelStockStrategies;
        public ObservableCollection<UserProfile.StockStrategy> CurSelStockStrategies
        {
            set
            {
                SetProperty(ref _curSelStockStrategies, value, nameof(CurSelStockStrategies));
                OnCurSelStockStrategyChange(value);
            }
            get { return _curSelStockStrategies; }
        }

        private void OnCurSelStockStrategyChange(ObservableCollection<UserProfile.StockStrategy> strategies)
        {
            if(strategies==null)
            {
                CanExecuteTask = false;
            }
            
        }

        

        /// <summary>
        /// 排序
        /// </summary>
        private int _sortNum=1;
        public int  SortNum
        {
            get { return _sortNum; }
            set { SetProperty(ref _sortNum, value, nameof(SortNum)); }
        }

        /// <summary>
        /// 策略名称
        /// </summary>
        private string _strategyName;
        public string StrategyName
        {
            get { return _strategyName; }
            set { SetProperty(ref _strategyName, value, nameof(StrategyName)); }
        }


        /// <summary>
        /// 当前选择的股票分类
        /// </summary>
        private SelfSelectStockCategory _curSelStockCategory;
        public SelfSelectStockCategory CurSelStockCategory
        {
            get { return _curSelStockCategory; }
            set { SetProperty(ref _curSelStockCategory, value, nameof(CurSelStockCategory)); }
        }
       

        /// <summary>
        /// 最近执行的时间
        /// </summary>
        private DateTime? _latestExecuteTime;
        public DateTime? LatestExecuteTime
        {
            get { return _latestExecuteTime; }
            set { SetProperty(ref _latestExecuteTime, value, nameof(LatestExecuteTime)); }
        }


        /// <summary>
        /// 计划任务描述
        /// </summary>
        private string _desc;
        public string Desc
        {
            get { return _desc; }
            set { SetProperty(ref _desc, value, nameof(Desc)); }
        }

        /// <summary>
        /// 是否加入服务计划任务选股
        /// </summary>
        private bool _isJoinServerScheduleTask;
        public bool IsJoinServerScheduleTask
        {
            get { return _isJoinServerScheduleTask; }
            set { SetProperty(ref _isJoinServerScheduleTask, value, nameof(IsJoinServerScheduleTask)); }
        }

        /// <summary>
        /// 客户端定时选股
        /// </summary>
        private bool _isJoinClientScheduleTask;
        public bool IsJoinClientScheduleTask
        {
            get { return _isJoinClientScheduleTask; }
            set { SetProperty(ref _isJoinClientScheduleTask, value, nameof(IsJoinClientScheduleTask)); }
        }

        /// <summary>
        /// 客户端定时时间
        /// </summary>
        private DateTime _clientScheduleTime;
        public DateTime ClientScheduleTime
        {
            get { return _clientScheduleTime; }
            set { SetProperty(ref _clientScheduleTime, value, nameof(ClientScheduleTime)); }
        }

        /// <summary>
        /// 所有计划任务列表
        /// </summary>
        private ObservableCollection<UserProfile.StrategyScheduledTask> _strategyScheduledTasks;
        public ObservableCollection<UserProfile.StrategyScheduledTask> StrategyScheduledTasks
        {
            get { return _strategyScheduledTasks; }
            set { SetProperty(ref _strategyScheduledTasks, value,nameof(StrategyScheduledTasks)); }
        }

        /// <summary>
        /// 当前选择的计划任务
        /// </summary>
        private UserProfile.StrategyScheduledTask _curSelStrategyScheduledTask;
        public UserProfile.StrategyScheduledTask CurSelStrategyScheduledTask
        {
            get { return _curSelStrategyScheduledTask; }
            set
            {
                SetProperty(ref _curSelStrategyScheduledTask, value, nameof(CurSelStrategyScheduledTask));
                OnCurSelStrategyScheduledTaskChange(value);
            }
        }
        /// <summary>
        /// 任务选择变更
        /// </summary>
        /// <param name="task"></param>
        private void OnCurSelStrategyScheduledTaskChange(UserProfile.StrategyScheduledTask task)
        {
            if (task == null )
            {
                ScheduledTaskName = null;
                CurSelStockCategory = null;
                StrategyName = null;
                LatestExecuteTime = null;
                Desc = null;
                IsJoinServerScheduleTask = false;
                CanExecuteTask = false;
                SortNum = 0;
                CurSelStockStrategies.Clear();
                IsJoinClientScheduleTask = false;

            }
            else
            {
                CanExecuteTask = !task.IsExecuting;

                ScheduledTaskName = task.Name;
                CurSelStockCategory = SelfSelectStockCategorys.Where(p => p.Id == task.OutputStockCategoryId).FirstOrDefault();

                LatestExecuteTime = task.LatestExecuteTime;
                IsJoinServerScheduleTask = task.IsJoinServerScheduleTask;
                IsJoinClientScheduleTask = task.IsJoinClientScheduleTask;

                if (IsJoinClientScheduleTask)
                {
                    if (task.ClientScheduleTime != null)
                    {
                        ClientScheduleTime = task.ClientScheduleTime.Value;
                    }
                }

                StrategyName = "";
                SortNum = task.SortNum;
                //解析策略ID
                if (!string.IsNullOrEmpty( task.StrategyIds))
                {
                    var ids = task.StrategyIds.Split(",");
                    if(ids.Length>0)
                    {
                        CurSelStockStrategies.Clear();
                        foreach (var id in ids)
                        {
                            foreach (var c in StockStrategyCategories)
                            {
                                var s = c.StockStrategies.Where(p => p.Id.ToString() == id).FirstOrDefault();
                                if (s != null)
                                {
                                    CurSelStockStrategies.Add(s);
                                    StrategyName += s.Name+";";
                                    break;
                                }

                            }

                        }
                    }
                }

                StrategyName = StrategyName.TrimEnd(';');

                Desc = task.Desc;
            }
        }
        /// <summary>
        /// 时候可以执行策略
        /// </summary>
        private bool  _canExecuteTask=false;
        public bool CanExecuteTask
        {
            get { return _canExecuteTask; }
            set { SetProperty(ref _canExecuteTask, value, nameof(CanExecuteTask)); }
        }

        public DelegateCommand SaveCommand { set; get; }
        private async void Save()
        {
            var sst = new UserProfile.StrategyScheduledTask();

            sst.UserId = _passportProvider.UserId;
            sst.Name = ScheduledTaskName;
            sst.OutputStockCategoryId = CurSelStockCategory.Id;
            sst.LatestExecuteTime = null;
            sst.IsJoinServerScheduleTask = IsJoinServerScheduleTask;
            sst.CreateTime = DateTime.Now;

            sst.IsJoinClientScheduleTask = IsJoinClientScheduleTask;

            if (IsJoinClientScheduleTask)
            {
                sst.ClientScheduleTime = ClientScheduleTime;
            }

            sst.StrategyIds = string.Join(',', CurSelStockStrategies.Select(p => p.Id));

            sst.Desc = Desc;

            sst.SortNum = SortNum;

            if (CurSelStrategyScheduledTask != null)//更新
            {
                sst.Id = CurSelStrategyScheduledTask.Id;
                sst.LatestExecuteTime = LatestExecuteTime;
            }

            //保存数据
            await Task.Run(() =>
            {
                var task = _scheduledTaskService.SaveStrategyScheduledTask(sst);
            }).ConfigureAwait(true);

            UpdateStrategyScheduledTaskStatus();

        }

        public DelegateCommand NewCommand { set; get; }
        private void New()
        {
            CurSelStrategyScheduledTask = null;
            SortNum = 1;
            if(StrategyScheduledTasks!=null && StrategyScheduledTasks.Count>0)
            {
                SortNum = StrategyScheduledTasks.Last().SortNum + 1;

                if (IsJoinClientScheduleTask)
                {
                    //默认下午2:35 执行计划任务
                    ClientScheduleTime = new DateTime(8888, 8, 8, 14, 35, 0);
                }
            }
        }

        public DelegateCommand DeleteCommand { set; get; }
        private  async void Delete()
        {
            await Task.Run(() => {
                _scheduledTaskService.DelStrategyScheduledTasksById(CurSelStrategyScheduledTask.Id);
            }).ConfigureAwait(true);

            CurSelStockStrategies.Clear();

            StrategyScheduledTasks.Remove(CurSelStrategyScheduledTask);

        }

        List<long> _executingTasks = new List<long>();

        public DelegateCommand ExecuteTaskCommand { set; get; }
        private async void ExecuteTask()
        {
            if (CurSelStockStrategies == null)
            {
                return;
            }

            _executingTasks.Add(CurSelStrategyScheduledTask.Id);

            UpdateStrategyScheduledTaskStatus();
            //执行策略
            await Task.Run(() =>
            {
                var task = CurSelStrategyScheduledTask;
                var tsCodes = _scheduledTaskService.ExecuteStrategyScheduledTask(task);
                _executingTasks.Remove(task.Id);

            }).ConfigureAwait(true);

            UpdateStrategyScheduledTaskStatus();

        }


        public DelegateCommand RefreshCommand { set; get; }
        private void  Refresh()
        {
            LoadStrategyCategoriesAndSelectStockCategories();

            UpdateStrategyScheduledTaskStatus();
        }

        /// <summary>
        /// 更新执行状态
        /// </summary>
        private async void UpdateStrategyScheduledTaskStatus()
        {
            ObservableCollection<UserProfile.StrategyScheduledTask> ssTasks = null;

            await Task.Run(() => {
                 ssTasks = _scheduledTaskService.GetStrategyScheduledTasksByUserId(_passportProvider.UserId);
            }).ConfigureAwait(true);


            foreach (var id in _executingTasks)
            {
                var cur = ssTasks.Where(p => p.Id == id).FirstOrDefault();
                cur.IsExecuting = true;
            }

            UserProfile.StrategyScheduledTask scheduledTaskExt = null;
            if (CurSelStrategyScheduledTask != null)
            {
                scheduledTaskExt = ssTasks.Where(p => p.Id == CurSelStrategyScheduledTask.Id).FirstOrDefault();
            }


            StrategyScheduledTasks = ssTasks;
            if (scheduledTaskExt != null)
            {
                CurSelStrategyScheduledTask = scheduledTaskExt;
            }
        }

    }
}
