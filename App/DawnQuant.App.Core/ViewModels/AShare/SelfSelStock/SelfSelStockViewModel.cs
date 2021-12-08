using DawnQuant.App.Core.ViewModels.AShare.Common;
using DawnQuant.App.Core.Models.AShare.EssentialData;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.Utils;

namespace DawnQuant.App.Core.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockViewModel 
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public SelfSelStockViewModel(StockPlotDataService stockPlotDataService,
           SelfSelService selfSelService,
           IPassportProvider passportProvider)
        {
            _stockPlotDataService = stockPlotDataService;
            _selfSelService = selfSelService;
            _passportProvider = passportProvider;

           
            //初始化 加载自选分类数据
            RefreshSelfSelectStockCategories();

            //默认选择第一条
            if (Categories != null && Categories.Any())
            {
                CurSelCategory = Categories[2];
            }
        }

        /// <summary>
        /// 刷新自选股分类
        /// </summary>
        public void RefreshSelfSelectStockCategories()
        {
            //刷新自选股分类
            Categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
        }

        //所有分类
        public ObservableCollection<SelfSelectStockCategory> Categories { get; private set; }


        /// <summary>
        /// 当前选择的分类
        /// </sum
        SelfSelectStockCategory _curSelCategory;
        public SelfSelectStockCategory CurSelCategory
        {
            get { return _curSelCategory; }
            set
            {
                //加载股票列表
                _curSelCategory = value;
                OnCategorySelChanged(_curSelCategory);
            }
        }

        /// <summary>
        /// 自选分类切换
        /// </summary>
        private void OnCategorySelChanged(SelfSelectStockCategory category)
        {
            //类目发生变更 加载自选股数据
            //  CurSelCategory = category;

            ObservableCollection<SelfSelectStock> temps = null;
            temps = _selfSelService.GetSelfSelectStocksByCategory(category.Id);

            Stocks = temps;
            // IsGroupByIndustry = category.IsGroupByIndustry;
            if (Stocks != null && Stocks.Count > 0)
            {
                CurSelStock = Stocks[0];
            }

        }


        //股票列表
        public ObservableCollection<SelfSelectStock> Stocks { get;  set; }
       



        //相关股票列表
        public ObservableCollection<SelfSelectStock> RelateStocks { get; set; }
        

        //是否分组
        public bool IsGroupByIndustry { get; set; }
       

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

                _curSelStock = value;
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
                _curSelRelateStock = value;
                //更新数据交易数据
                OnSelRelateStockChange(value);
            }
        }


        private void OnSelRelateStockChange(SelfSelectStock stockItem)
        {
           // UpdateChart(stockItem);
        }


        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelStockChange(SelfSelectStock stockItem)
        {
           // UpdateChart(stockItem);
           // UpdateRelateStocks(stockItem?.TSCode);
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private async void UpdateChart(SelfSelectStock stockItem)
        {
            if (stockItem != null)
            {
                //获取交易数据

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

                await model.InitPlotContext().ConfigureAwait(true);

                this.StockChartViewModel = model;
            }
        }

        /// <summary>
        /// 查找同类股票
        /// </summary>
        /// <param name="tsCode"></param>
        private  async void UpdateRelateStocks(string tsCode)
        {
            if (tsCode != null)
            {
                ObservableCollection<SelfSelectStock> temprs = null;
                await Task.Run(() =>
                {
                     temprs = _selfSelService.GetSameIndustryStocks(tsCode);
                  
                });
                RelateStocks = temprs;
            }
           

        }

        //每日指标数据
        //StockDailyIndicator _dailyIndicator;
        //public StockDailyIndicator DailyIndicator
        //{
        //    get { return _dailyIndicator; }
        //    set { SetProperty(ref _dailyIndicator, value, nameof(DailyIndicator)); }
        //}

        public StockChartViewModel StockChartViewModel { get; set; }
       


        #region commond
        //导入自选股
        private void ImportSelfStocks(List<string> stocksId)
        {
            if (CurSelCategory != null)
            {
                var ss = _selfSelService.ImportSelfStocks(_passportProvider.UserId, stocksId, CurSelCategory.Id);

                //刷新自选股
                OnCategorySelChanged(CurSelCategory);
            }

        }

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

            var sstock = _selfSelService.SaveSelfSelectStock(stockItem);

            //删除股票
            DelStockItem();

            //更新当前
            if (CurSelCategory.Id == category.Id)
            {
                if (!Stocks.Any(p => p.Id == sstock.Id))
                {
                    Stocks.Insert(0, sstock);
                }

            }
        }

       
        /// <summary>
        /// 删除股票
        /// </summary>
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

        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
              TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0,6));
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

        #endregion




    }
}
