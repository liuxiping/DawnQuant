using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DawnQuant.App.ViewModels.AShare.Common;
using DawnQuant.App.Models.AShare.EssentialData;
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.Bellwether
{
    class BellwetherViewModel : ViewModelBase
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly BellwetherService _bellwetherService;
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;


        public BellwetherViewModel()
        {

            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
            _stockPlotDataService = IOCUtil.Container.Resolve<StockPlotDataService>(); ;
            _bellwetherService = IOCUtil.Container.Resolve<BellwetherService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();


            DelStockItemCommand = new DelegateCommand(DelStockItem);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);
            CopyStockNameCommand = new DelegateCommand(CopyStockName);

            CopyRelateStockCodeCommand = new DelegateCommand(CopyRelateStockCode);
            CopyRelateStockNameCommand = new DelegateCommand(CopyRelateStockName);

            DeleteBellwetherStockCategoryCommand = new DelegateCommand(DeleteBellwetherStockCategory);
            DelStockItemCommand = new DelegateCommand(DelStockItem);

            //初始化 加载龙头股分类数据
            RefreshBellwetherStockCategories();
        }


        /// <summary>
        /// 刷新龙头股类目
        /// </summary>
        public void RefreshBellwetherStockCategories()
        {
            LoadCategories();
        }


        /// <summary>
        /// 加载龙头股分类分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<BellwetherStockCategory> categories = null;

            await Task.Run(() =>
            {
                categories = _bellwetherService.GetBellwetherStockCategories(_passportProvider.UserId);

            }).ConfigureAwait(true);

            Categories = categories;

            if (Categories == null || Categories.Count == 0)
            {
                CurSelCategory = null;
            }
            if (CurSelCategory == null && Categories != null && Categories.Count > 0)
            {
                CurSelCategory = Categories.First();
            }
            else
            {
                CurSelCategory = Categories.Where(p => p.Id == CurSelCategory.Id).FirstOrDefault();
            }
        }


        //所有龙头股分类
        ObservableCollection<BellwetherStockCategory> _categories;
        public ObservableCollection<BellwetherStockCategory> Categories
        {
            set
            { SetProperty(ref _categories, value, nameof(Categories)); }
            get
            { return _categories; }
        }

        /// <summary>
        /// 当前选择的龙头股分类
        /// </summary>
        BellwetherStockCategory _curSelCategory;
        public BellwetherStockCategory CurSelCategory
        {
            set
            {
                SetProperty(ref _curSelCategory, value, nameof(CurSelCategory));
                OnCurSelCategoryChange();
            }
            get
            { return _curSelCategory; }
        }

        /// <summary>
        /// 龙头股分类变更
        /// </summary>
        private async void OnCurSelCategoryChange()
        {
            if (CurSelCategory != null)
            {
                ObservableCollection<BellwetherStock> temps = null;
                //更新龙头股票
                await Task.Run(() =>
                 {
                     temps = _bellwetherService.GetBellwetherStocksByCategory(CurSelCategory.Id);

                 }).ConfigureAwait(true);
                Stocks = temps;
                if (Stocks != null && Stocks.Count > 0)
                {
                    CurSelStock = Stocks[0];
                }
                else
                {
                    Stocks.Clear();
                }
            }
        }

        //龙头股股票列表
        ObservableCollection<BellwetherStock> _stocks = new ObservableCollection<BellwetherStock>();
        public ObservableCollection<BellwetherStock> Stocks
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


        //当前选择的龙头股
        BellwetherStock _curSelStock;
        public BellwetherStock CurSelStock
        {
            get
            {
                return _curSelStock;
            }
            set
            {

                SetProperty(ref _curSelStock, value, nameof(CurSelStock));
                //更新数据交易数据
                OnSelStockChange(value);
            }
        }


        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }


        //相关股票列表
        ObservableCollection<BellwetherStock> _relateStocks;
        public ObservableCollection<BellwetherStock> RelateStocks
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


        //当前选择的相关股票
        BellwetherStock _curSelRelateStock;
        public BellwetherStock CurSelRelateStock
        {
            get
            {
                return _curSelRelateStock;
            }
            set
            {

                SetProperty(ref _curSelRelateStock, value, nameof(CurSelRelateStock));
                //更新数据交易数据
                OnSelRelateStockChange(value);
            }
        }

        private  void  OnSelRelateStockChange(BellwetherStock stockItem)
        {
            UpdateChart(stockItem);
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private  void OnSelStockChange(BellwetherStock stockItem)
        {
             UpdateChart(stockItem);
             UpdateRelateStocks(stockItem?.TSCode);
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private async void UpdateChart(BellwetherStock stockItem)
        {
            if (stockItem != null)
            {
                StockChartViewModel model = new StockChartViewModel()
                {
                    TSCode = stockItem.TSCode,
                    StockName = stockItem.Name,
                    KCycle = KCycle.Day,
                    VA = StockChartViewModel.VisibleArea.Chart,
                    AdjustedState = AdjustedState.None,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                    model.AdjustedState= StockChartViewModel.AdjustedState;
                }
                //获取交易数据
                await model.InitPlotContext().ConfigureAwait(true);
                this.StockChartViewModel = model;

            }

        }

        /// <summary>
        /// 查找同类股票
        /// </summary>
        /// <param name="tsCode"></param>
        private async void UpdateRelateStocks(string tsCode)
        {
            if (tsCode != null)
            {
                ObservableCollection<BellwetherStock> temprs = null;
                await Task.Run(() =>
                {
                    temprs = _bellwetherService.GetSameIndustryStocks(tsCode);

                }).ConfigureAwait(true);
                RelateStocks = temprs;
            }
        } 


        /// <summary>
        /// 删除股票
        /// </summary>
        public DelegateCommand DelStockItemCommand { set; get; }
        private async void DelStockItem()
        {
            if (CurSelStock != null)
            {
                if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
                {
                    await Task.Run(() =>
                     {
                         _bellwetherService.DelBellwetherStock(CurSelStock);
                     }).ConfigureAwait(true);

                    Stocks.Remove(CurSelStock);
                }
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        public DelegateCommand DeleteBellwetherStockCategoryCommand { set; get; }
        private async void DeleteBellwetherStockCategory()
        {
            if (CurSelCategory != null)
            {
                await Task.Run(() =>
                {
                    _bellwetherService.DelBellwetherStockCategory(CurSelCategory);
                });

                LoadCategories();

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


       
    }
}
