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

namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StockStrategyViewModel : ViewModelBase
    {
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyExecutorService _strategyExecutorService;
        private readonly AShareDataMaintainService _aShareDataMaintainService;
        private readonly StockStrategyService _stockStrategyService;

        private readonly SelfSelService _selfSelService;

        public StockStrategyViewModel()
        {
            _strategyMetadataService = IOCUtil.Container.Resolve<StrategyMetadataService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _strategyExecutorService = IOCUtil.Container.Resolve<StrategyExecutorService>();
            _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>(); ;
            ExecuteStrategyCommand = new AsyncCommand<UserProfile.StockStrategy>(ExecuteStrategy);

            DelStockItemCommand = new DelegateCommand(DelStockItem);
            MoveToOtherCategoryCommand = new DelegateCommand<SelfSelectStockCategory>(MoveToOtherCategory);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);

            LoadCategoriesIncludeStrategies();
            LoadSelfSelectStockCategories();
        }


        /// <summary>
        /// 加载自选分类
        /// </summary>
        public void LoadSelfSelectStockCategories()
        {
            //加载策略
            Task.Run(() =>
            {
                SelfSelectStockCategories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);

            });
        }


        /// <summary>
        /// 加载策略
        /// </summary>
        public void LoadCategoriesIncludeStrategies()
        {
            StockStrategyCategories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId);
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
        private void UpdateChart(SelfSelectStock stockItem)
        {
            if (stockItem != null)
            {
                //获取交易数据
                Task t = Task.Run(() =>
                {
                    StockChartViewModel model = new StockChartViewModel
                    {
                        TSCode = stockItem.TSCode,
                        StockName = stockItem.Name,
                        KCycle = KCycle.Day,
                        VA = StockChartViewModel.VisibleArea.Chart
                    };
                    //保存前一个数据选择状态
                    if (StockChartViewModel != null)
                    {
                        model.VA = StockChartViewModel.VA;
                        model.KCycle = StockChartViewModel.KCycle;
                    }

                    model.InitPlotContext();

                    this.StockChartViewModel = model;

                });
            }



        }

        /// <summary>
        /// 执行策略
        /// </summary>
        public AsyncCommand<UserProfile.StockStrategy> ExecuteStrategyCommand { get; private set; }
        private Task ExecuteStrategy(UserProfile.StockStrategy stockStrategy)
        {
            var t = Task.Run(() =>
              {

                  //执行策略
                  var stocks = _strategyExecutorService.ExecuteStrategyByContent(stockStrategy.StockStragyContent);
                  if (stocks != null && stocks.Count > 0)
                  {
                      var tsCodes = stocks.Select(p => p.TSCode).ToList();

                      //下载数据
                      //_aShareDataMaintainService.DownLoadStockData(tsCodes);

                      Stocks = stocks;
                  }
                  else
                  {
                      Stocks = null;
                  }
              });
            return t;

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
        private void MoveToOtherCategory(SelfSelectStockCategory category)
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

            var sstock = _selfSelService.SaveSelfSelectStock(stockItem);

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
