using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.ViewModels.AShare.Common;
using DawnQuant.App.Core.Models.AShare.EssentialData;

namespace DawnQuant.App.Core.ViewModels.AShare.Bellwether
{
    class BellwetherViewModel 
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly BellwetherService _bellwetherService;
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;


        public BellwetherViewModel(StockPlotDataService stockPlotDataService,
           BellwetherService bellwetherService,
           IPassportProvider passportProvider,
           SelfSelService selfSelService)
        {

            _selfSelService = selfSelService;
            _stockPlotDataService = stockPlotDataService;
            _bellwetherService = bellwetherService;
            _passportProvider = passportProvider;

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
        private void LoadCategories()
        {

            Categories = _bellwetherService.GetBellwetherStockCategories(_passportProvider.UserId);
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
        public ObservableCollection<BellwetherStockCategory> Categories { get; set; }   
        

        /// <summary>
        /// 当前选择的龙头股分类
        /// </summary>
        BellwetherStockCategory _curSelCategory;
        public BellwetherStockCategory CurSelCategory
        {
            set
            {
                _curSelCategory = value;
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
        public ObservableCollection<BellwetherStock> Stocks { get; set; }
        


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

                _curSelStock = value;
                //更新数据交易数据
                OnSelStockChange(value);
            }
        }


        public StockChartViewModel StockChartViewModel { get; set; }
        


        //相关股票列表
        public ObservableCollection<BellwetherStock> RelateStocks { get; set; }
       


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
                _curSelRelateStock = value;
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
                StockChartViewModel model = new StockChartViewModel(null)
                {
                    TSCode = stockItem.TSCode,
                    StockName = stockItem.Name,
                    KCycle = KCycle.Day,
                    VA = StockChartViewModel.VisibleArea.Chart,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
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
        private void DelStockItem()
        {
            if (CurSelStock != null)
            {
                if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
                {
                    _bellwetherService.DelBellwetherStock(CurSelStock);
                    Stocks.Remove(CurSelStock);
                }
            }
        }

        /// <summary>
        /// 删除分类
        /// </summary>
        private void DeleteBellwetherStockCategory()
        {
            if (CurSelCategory != null)
            {
                _bellwetherService.DelBellwetherStockCategory(CurSelCategory);
                LoadCategories();

            }
        }

        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0, 6));
            }
        }

        private void CopyStockName()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.Name);
            }
        }


        private void CopyRelateStockCode()
        {
            if (CurSelRelateStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelRelateStock.TSCode.Substring(0, 6));
            }
        }


        private void CopyRelateStockName()
        {
            if (CurSelRelateStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelRelateStock.Name);
            }
        }


       
    }
}
