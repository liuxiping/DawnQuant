using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using System.Collections.ObjectModel;
using UserProfile = DawnQuant.App.Core.Models.AShare.UserProfile;


namespace DawnQuant.App.Core.ViewModels.AShare.StrategyScheduledTask
{
    public class StrategyScheduledTaskViewModel 
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyMetadataService  _strategyMetadataService;
        private readonly SelfSelService  _selfSelService;
        private readonly AShareDataMaintainService  _aShareDataMaintainService;
        private readonly StrategyScheduledTaskService _scheduledTaskService;
        private readonly StockStrategyService _stockStrategyService;

        public StrategyScheduledTaskViewModel(IPassportProvider passportProvider,
           StrategyMetadataService strategyMetadataService,
           SelfSelService selfSelService,
           AShareDataMaintainService aShareDataMaintainService,
           StrategyScheduledTaskService scheduledTaskService,
           StockStrategyService stockStrategyService)
        {
            _passportProvider = passportProvider;
            _strategyMetadataService = strategyMetadataService;
            _selfSelService = selfSelService;
            _aShareDataMaintainService = aShareDataMaintainService;
            _scheduledTaskService = scheduledTaskService;
            _stockStrategyService = stockStrategyService;


          

            CurSelStockStrategies = new ObservableCollection<UserProfile.StockStrategy>();
            //初始化数据
            StockStrategyCategories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId);
            SelfSelectStockCategorys = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);

            UpdateStrategyScheduledTaskStatus();
        }

     

        /// <summary>
        /// 所有策略分类
        /// </summary>
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories { get;  set; }
       
        /// <summary>
        /// 

        //所有自选分类
        public ObservableCollection<SelfSelectStockCategory> SelfSelectStockCategorys { get; set; }



        /// <summary>
        /// 计划任务名称
        /// </summary>
        public string ScheduledTaskName { get; set; }


        /// <summary>
        /// 当前的策略
        /// </summary>
        ObservableCollection<UserProfile.StockStrategy> _curSelStockStrategies;
        public ObservableCollection<UserProfile.StockStrategy> CurSelStockStrategies
        {
            set
            {
                _curSelStockStrategies=value;
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
        public int SortNum { get; set; } = 1;


        /// <summary>
        /// 策略名称
        /// </summary>
        public string StrategyName { get; set; }



        /// <summary>
        /// 当前选择的股票分类
        /// </summary>
        public SelfSelectStockCategory CurSelStockCategory { get; set; }



        /// <summary>
        /// 最近执行的时间
        /// </summary>
        public DateTime? LatestExecuteTime { get; set; }



        /// <summary>
        /// 计划任务描述
        /// </summary>
        public string Desc { get; set; }


        /// <summary>
        /// 是否加入服务计划任务选股
        /// </summary>
        public bool IsJoinServerScheduleTask { get; set; }


        /// <summary>
        /// 客户端定时选股
        /// </summary>
        public bool IsJoinClientScheduleTask { get; set; }


        /// <summary>
        /// 客户端定时时间
        /// </summary>
        public DateTime ClientScheduleTime { get; set; }


        /// <summary>
        /// 所有计划任务列表
        /// </summary>
        public ObservableCollection<UserProfile.StrategyScheduledTask> StrategyScheduledTasks { get; set; }


        /// <summary>
        /// 当前选择的计划任务
        /// </summary>
        private UserProfile.StrategyScheduledTask _curSelStrategyScheduledTask;
        public UserProfile.StrategyScheduledTask CurSelStrategyScheduledTask
        {
            get { return _curSelStrategyScheduledTask; }
            set
            {
                _curSelStrategyScheduledTask= value;
                OnCurSelStrategyScheduledTaskChange(value);
            }
        }
        /// <summary>
        /// 任务选择变更
        /// </summary>
        /// <param name="task"></param>
        private void OnCurSelStrategyScheduledTaskChange(UserProfile.StrategyScheduledTask task)
        {
            if (task == null)
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
        public bool CanExecuteTask { get; set; }


        private void Save()
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
            var task = _scheduledTaskService.SaveStrategyScheduledTask(sst);


            UpdateStrategyScheduledTaskStatus();
         
        }

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

        private  void Delete()
        {

            _scheduledTaskService.DelStrategyScheduledTasksById(CurSelStrategyScheduledTask.Id);
            CurSelStockStrategies.Clear();
            StrategyScheduledTasks.Remove(CurSelStrategyScheduledTask);

        }

        List<long> _executingTasks = new List<long>();

        private void ExecuteTask()
        {
            if (CurSelStockStrategies == null)
            {
                return;
            }
           
            _executingTasks.Add(CurSelStrategyScheduledTask.Id);
            //执行策略
            Task.Run(() =>
            {
               var task = CurSelStrategyScheduledTask;

               var tsCodes= _scheduledTaskService.ExecuteStrategyScheduledTask(task);
                _executingTasks.Remove(task.Id);


                UpdateStrategyScheduledTaskStatus();
            });

            UpdateStrategyScheduledTaskStatus();
      
        }


        private void Refresh()
        {
            StockStrategyCategories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId);
            SelfSelectStockCategorys = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            UpdateStrategyScheduledTaskStatus();
        }

        /// <summary>
        /// 更新执行状态
        /// </summary>
        private void UpdateStrategyScheduledTaskStatus()
        {
            var ssTasks = _scheduledTaskService.GetStrategyScheduledTasksByUserId(_passportProvider.UserId);

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
