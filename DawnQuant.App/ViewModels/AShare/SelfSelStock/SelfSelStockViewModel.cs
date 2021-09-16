using DawnQuant.App.ViewModels.AShare.Common;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.App.Utils;
using Autofac;
using System.Windows;

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockViewModel:ViewModelBase
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public SelfSelStockViewModel()
        {

            _stockPlotDataService = IOCUtil.Container.Resolve<StockPlotDataService>(); ;
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();

            ImportSelfStocksCommand = new DelegateCommand<List<string>>(ImportSelfStocks);
            CategorySelChangedCommand = new DelegateCommand<SelfSelectStockCategory>(CategorySelChanged);
            DelStockItemCommand = new DelegateCommand(DelStockItem);
            MoveToOtherCategoryCommand = new DelegateCommand<SelfSelectStockCategory>(MoveToOtherCategory);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);
            CopyStockNameCommand = new DelegateCommand(CopyStockName);

            CopyRelateStockCodeCommand = new DelegateCommand(CopyRelateStockCode);
            CopyRelateStockNameCommand = new DelegateCommand(CopyRelateStockName);

            //初始化 加载自选分类数据
            Initialize();
        }

        public Task Initialize()
        {
            RefreshSelfSelectStockCategories();
            return Task.CompletedTask;
        }


        /// <summary>
        /// 刷新自选股分类
        /// </summary>
        public void RefreshSelfSelectStockCategories()
        {
            //刷新自选股分类
            Task.Run(() =>
            {
                Categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            });

        }

        //所有分类
        ObservableCollection<SelfSelectStockCategory> _categories;
        public ObservableCollection<SelfSelectStockCategory> Categories
        {
            set
            { SetProperty(ref _categories, value,nameof(Categories)); }
            get
            { return _categories; }
        }

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        SelfSelectStockCategory _curSelCategory;
        public SelfSelectStockCategory CurSelCategory
        {
            set
            { SetProperty(ref _curSelCategory, value, nameof(CurSelCategory)); }
            get
            { return _curSelCategory; }
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
                SetProperty(ref _stocks, value, nameof(Stocks));
            }
        }


        //相关股票列表
        ObservableCollection<SelfSelectStock> _relateStocks;
        public ObservableCollection<SelfSelectStock> RelateStocks
        {
            get
            {
                return _relateStocks;
            }
            set
            {
                SetProperty(ref _relateStocks, value, nameof(RelateStocks));
            }
        }

        //是否分组
        bool _isGroupByIndustry = false;
        public bool IsGroupByIndustry
        {
            get { return _isGroupByIndustry; }
            set { SetProperty(ref _isGroupByIndustry, value, nameof(IsGroupByIndustry)); }
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

                SetProperty(ref _curSelStock, value,nameof(CurSelStock));
                //更新数据交易数据
                OnSelStockChange(value);
            }
        }


        //当前选择的相关股票
        SelfSelectStock _curSelRelateStock;
        public SelfSelectStock CurSelRelateStock
        {
            get
            {
                return _curSelRelateStock;
            }
            set
            {

                SetProperty(ref _curSelRelateStock, value,nameof(CurSelRelateStock));
                //更新数据交易数据
                OnSelRelateStockChange(value);
            }
        }


        private void OnSelRelateStockChange(SelfSelectStock stockItem)
        {
            UpdateChart(stockItem);
        }


        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelStockChange(SelfSelectStock stockItem)
        {
                UpdateChart(stockItem);
                UpdateRelateStocks(stockItem?.TSCode);
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private Task UpdateChart(SelfSelectStock stockItem)
        {
            if (stockItem != null)
            {
                //获取交易数据
                return Task.Run(() =>
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
            else
            {
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 查找同类股票
        /// </summary>
        /// <param name="tsCode"></param>
        private Task UpdateRelateStocks(string tsCode)
        {
            if (tsCode != null)
            {
                return Task.Run(() =>
            {
                RelateStocks = _selfSelService.GetSameIndustryStocks(tsCode);
            });
            }
            else
            {
                return Task.CompletedTask;
            }

        }

        //每日指标数据
        //StockDailyIndicator _dailyIndicator;
        //public StockDailyIndicator DailyIndicator
        //{
        //    get { return _dailyIndicator; }
        //    set { SetProperty(ref _dailyIndicator, value, nameof(DailyIndicator)); }
        //}

        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }


        #region commond
        //导入自选股
        public DelegateCommand<List<string>> ImportSelfStocksCommand { set; get; }
        private void ImportSelfStocks(List<string> stocksId)
        {
            if (CurSelCategory != null)
            {
                var ss = _selfSelService.ImportSelfStocks(_passportProvider.UserId, stocksId, CurSelCategory.Id);

                //刷新自选股
                CategorySelChanged(CurSelCategory);
            }

        }

        public DelegateCommand<SelfSelectStockCategory> MoveToOtherCategoryCommand { set; get; }
        private void MoveToOtherCategory(SelfSelectStockCategory category)
        {
            SelfSelectStock curPlotStock = null;

            if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
            {
                curPlotStock = CurSelStock;
            }
            else if (CurSelRelateStock.TSCode == StockChartViewModel.PlotContext.TSCode)
            {
                curPlotStock = CurSelRelateStock;
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

            var sstock=_selfSelService.SaveSelfSelectStock(stockItem);

            //删除股票
            DelStockItem();

            //更新当前
            if (CurSelCategory.Id == category.Id)
            {
                if (!Stocks.Any(p => p.Id == sstock.Id))
                {
                    Stocks.Insert(0,sstock);
                }
                
            }
        }

        /// <summary>
        /// 自选分类切换
        /// </summary>
        public DelegateCommand<SelfSelectStockCategory> CategorySelChangedCommand { set; get; }
        private void CategorySelChanged(SelfSelectStockCategory category)
        {
            //类目发生变更 加载自选股数据
            CurSelCategory = category;

            Task.Run(() =>
            {
                Stocks = _selfSelService.GetSelfSelectStocksByCategory(category.Id);
                IsGroupByIndustry = category.IsGroupByIndustry;
                if(Stocks!=null && Stocks.Count>0)
                {
                    CurSelStock = Stocks[0];
                }
               
            });
          
        }

        /// <summary>
        /// 删除股票
        /// </summary>
        public DelegateCommand DelStockItemCommand { set; get; }
        private void DelStockItem()
        {
            if(CurSelStock!=null)
            {
                if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
                {
                    _selfSelService.DelSelfSelectStock(CurSelStock);
                    Stocks.Remove(CurSelStock);
                }
            }
        }

        public DelegateCommand CopyStockCodeCommand { set; get; }
        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
              TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0,6));
            }
        }

        public DelegateCommand CopyStockNameCommand { set; get; }
        private void CopyStockName()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.Name);
            }
        }
        

        public DelegateCommand CopyRelateStockCodeCommand { set; get; }
        private void CopyRelateStockCode()
        {
            if (CurSelRelateStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelRelateStock.TSCode.Substring(0, 6));
            }
        }


        public DelegateCommand CopyRelateStockNameCommand { set; get; }
        private void CopyRelateStockName()
        {
            if (CurSelRelateStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelRelateStock.Name);
            }
        }

        #endregion




    }
}
