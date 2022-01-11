using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DawnQuant.App.ViewModels.AShare.Common;
using DevExpress.Mvvm;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Models.AShare.EssentialData;
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using Autofac;
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StockStrategyViewModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyExecutorService _strategyExecutorService;
        private readonly StockStrategyService _stockStrategyService;

        private readonly SelfSelService _selfSelService;

        public StockStrategyViewModel()
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _strategyExecutorService = IOCUtil.Container.Resolve<StrategyExecutorService>();
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>(); ;
            ExecuteStrategyCommand = new AsyncCommand<UserProfile.StockStrategy>(ExecuteStrategy);

            DelStockItemCommand = new DelegateCommand(DelStockItem);
            MoveToOtherCategoryCommand = new DelegateCommand<SelfSelectStockCategory>(MoveToOtherCategory);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);

            Initialize();

        }

        private async void Initialize()
        {
            await LoadCategoriesIncludeStrategies();

            await LoadSelfSelectStockCategories();
        }

        /// <summary>
        /// 加载自选股分类
        /// </summary>
        public async Task LoadSelfSelectStockCategories()
        {
            ObservableCollection<SelfSelectStockCategory> category=null;

            await Task.Run(() => 
            {
                 category = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            }).ConfigureAwait(true);

            //加载策略
            SelfSelectStockCategories = category;

            OnSelfSelectStockCategoriesLoad();

        }


        /// <summary>
        /// 自选分类加载完毕
        /// </summary>
        public event Action SelfSelectStockCategoriesLoad;
        private void OnSelfSelectStockCategoriesLoad()
        {
            SelfSelectStockCategoriesLoad?.Invoke();
        }

        /// <summary>
        /// 加载策略分类与策略
        /// </summary>
        public async Task LoadCategoriesIncludeStrategies()
        {
            ObservableCollection<StockStrategyCategory> categories = null;
            await Task.Run(() =>
            {
                categories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId); ;
            }).ConfigureAwait(true);

            StockStrategyCategories = categories;
        }

        //当前选择的股票
        SelfSelectStock _curSelStock;
        public SelfSelectStock CurSelStock
        {
            get
            {
                return _curSelStock;
            }
            set
            {
                SetProperty(ref _curSelStock, value, nameof(CurSelStock));
                //更新数据交易数据
                UpdateChart(value);
            }
        }

        //股票列表
        ObservableCollection<SelfSelectStock> _stocks;
        public ObservableCollection<SelfSelectStock> Stocks
        {
            get
            {
                return _stocks;
            }
            set
            {
                SetProperty<ObservableCollection<SelfSelectStock>>(ref _stocks, value, nameof(Stocks));
            }
        }


        //所有分类
        ObservableCollection<SelfSelectStockCategory> _selfSelectStockCategories;
        public ObservableCollection<SelfSelectStockCategory> SelfSelectStockCategories
        {
            set
            { SetProperty(ref _selfSelectStockCategories, value, nameof(SelfSelectStockCategories)); }
            get
            { return _selfSelectStockCategories; }
        }



        //当前选择的策略分类
        StockStrategyCategory _curSelStockStrategyCategory;
        public StockStrategyCategory CurSelStockStrategyCategory
        {
            get
            {
                return _curSelStockStrategyCategory;
            }
            set
            {
                SetProperty<StockStrategyCategory>(ref _curSelStockStrategyCategory, value, nameof(CurSelStockStrategyCategory));
            }
        }


        /// <summary>
        /// 策略分类
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
        /// 绘图ViewModel
        /// </summary>
        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }

        /// <summary>
        /// 股票列表选择变更 更新绘图
        /// </summary>
        /// <param name="stockItem"></param>
        private async void UpdateChart(SelfSelectStock stockItem)
        {
            if (stockItem != null)
            {
                //获取交易数据
                StockChartViewModel model = new StockChartViewModel()
                {
                    TSCode = stockItem.TSCode,
                    Name = stockItem.Name,
                    KCycle = KCycle.Day,
                    VA = VisibleArea.Chart,
                    AdjustedState = AdjustedState.None,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                    model.AdjustedState= StockChartViewModel.AdjustedState;
                }

                await model.InitPlotContext();

                this.StockChartViewModel = model;

            }

        }

        /// <summary>
        /// 执行策略
        /// </summary>
        public AsyncCommand<UserProfile.StockStrategy> ExecuteStrategyCommand { get; private set; }
        private async Task ExecuteStrategy(UserProfile.StockStrategy stockStrategy)
        {
            ObservableCollection<SelfSelectStock> stocks = null;
            await Task.Run(() =>
              {
                  //执行策略
                 stocks = _strategyExecutorService.ExecuteStrategyByContent(stockStrategy.StockStragyContent);

                  if (stocks != null && stocks.Any())
                  {
                      stocks = new ObservableCollection<SelfSelectStock>(stocks.OrderBy(p => p.Industry));
                  }

              }).ConfigureAwait(true);

            if (stocks != null && stocks.Count > 0)
            {
                var tsCodes = stocks.Select(p => p.TSCode).ToList();
                Stocks = stocks;
            }
            else
            {
                Stocks = null;
            }
        }

        public DelegateCommand DelStockItemCommand { get; set; }
        private void DelStockItem()
        {
            if(CurSelStock!=null)
            {
                Stocks.Remove(CurSelStock);
            }
        }

        /// <summary>
        /// 添加到其他分类
        /// </summary>
        public DelegateCommand<SelfSelectStockCategory> MoveToOtherCategoryCommand { set; get; }
        private async void MoveToOtherCategory(SelfSelectStockCategory category)
        {
            SelfSelectStock curPlotStock = null;

            if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
            {
                curPlotStock = CurSelStock;
            }
            else
            {
                return;
            }

            SelfSelectStock stockItem = new SelfSelectStock
            {
                CategoryId = category.Id,
                Industry = curPlotStock.Industry,
                Name = curPlotStock.Name,
                SortNum = curPlotStock.SortNum,
                TSCode = curPlotStock.TSCode,
                UserId = curPlotStock.UserId,
                CreateTime = DateTime.Now,

            };

            SelfSelectStock sstock = null;
            await Task.Run(() => 
            {
                sstock = _selfSelService.SaveSelfSelectStock(stockItem);
            }).ConfigureAwait(true);
           
            //更新当前
            if (!Stocks.Any(p => p.Id == sstock.Id))
            {
                Stocks.Remove(curPlotStock);
            }
        }

        public DelegateCommand CopyStockCodeCommand { set; get; }
        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0, 6));
            }
        }
    }
}
