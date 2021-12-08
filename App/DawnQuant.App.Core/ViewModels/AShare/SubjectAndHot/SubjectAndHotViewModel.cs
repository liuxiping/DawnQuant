using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.ViewModels.AShare.Common;
using DawnQuant.App.Core.Models.AShare.EssentialData;

namespace DawnQuant.App.Core.ViewModels.AShare.SubjectAndHot
{
    class SubjectAndHotViewModel 
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly SubjectAndHotService _SubjectAndHotService;
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;


        public SubjectAndHotViewModel(StockPlotDataService stockPlotDataService,
           SubjectAndHotService SubjectAndHotService,
           IPassportProvider passportProvider,
           SelfSelService selfSelService)
        {
            _selfSelService = selfSelService;
            _stockPlotDataService = stockPlotDataService;
            _SubjectAndHotService = SubjectAndHotService;
            _passportProvider = passportProvider;


          

            //初始化 加载题材热点分类数据
            Initialize();
        }


        public Task Initialize()
        {
           return RefreshSubjectAndHotStockCategories();
        }


        public Task RefreshSubjectAndHotStockCategories()
        {
            return Task.Run(() =>
            {
                LoadCategories();
            });
        }

        /// <summary>
        /// 加载题材概念分类
        /// </summary>
        private void LoadCategories()
        {
            Categories = _SubjectAndHotService.GetSubjectAndHotStockCategories(_passportProvider.UserId);
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


        //所有题材概念分类
        public ObservableCollection<SubjectAndHotStockCategory> Categories { set; get; }
      

        /// <summary>
        /// 当前选择的题材概念分类
        /// </summary>
        SubjectAndHotStockCategory _curSelCategory;
        public SubjectAndHotStockCategory CurSelCategory
        {
            set
            {
                _curSelCategory = value ;
                OnCurSelCategoryChange();
            }
            get
            { return _curSelCategory; }
        }

        /// <summary>
        /// 题材概念分类选择变更
        /// </summary>
        private void OnCurSelCategoryChange()
        {
            if (CurSelCategory != null)
            {
                //更新龙头股票
                Task.Run(() =>
                {
                    Stocks = _SubjectAndHotService.GetSubjectAndHotStocksByCategory(CurSelCategory.Id);
                    if (Stocks != null && Stocks.Count > 0)
                    {
                        CurSelStock = Stocks[0];
                    }

                });
            }
            else
            {
                Stocks.Clear();
            }
        }

        //题材概念股票列表
        public ObservableCollection<SubjectAndHotStock> Stocks { set; get; }



        //当前选择的题材概念
        SubjectAndHotStock _curSelStock;
        public SubjectAndHotStock CurSelStock
        {
            get
            {
                return _curSelStock;
            }
            set
            {

                _curSelStock=value;
                //更新数据交易数据
                OnSelStockChange(value);
            }
        }

        
        public StockChartViewModel StockChartViewModel { set; get; }



        //相关股票列表
        public ObservableCollection<SubjectAndHotStock> RelateStocks { set; get; }



        //当前选择的相关股票
        SubjectAndHotStock _curSelRelateStock;
        public SubjectAndHotStock CurSelRelateStock
        {
            get
            {
                return _curSelRelateStock;
            }
            set
            {

                _curSelRelateStock=value;
                //更新数据交易数据
                OnSelRelateStockChange(value);
            }
        }

        private void OnSelRelateStockChange(SubjectAndHotStock stockItem)
        {
            UpdateChart(stockItem);
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelStockChange(SubjectAndHotStock stockItem)
        {
            UpdateChart(stockItem);
            UpdateRelateStocks(stockItem?.TSCode);
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private  async void  UpdateChart(SubjectAndHotStock stockItem)
        {
            if (stockItem != null)
            {
                //获取交易数据

                StockChartViewModel model = new StockChartViewModel(null)
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

                await model.InitPlotContext();

                this.StockChartViewModel = model;


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
                    RelateStocks = _SubjectAndHotService.GetSameIndustryStocks(tsCode);
                });
            }
            else
            {
                return Task.CompletedTask;
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
                    _SubjectAndHotService.DelSubjectAndHotStock(CurSelStock);
                    Stocks.Remove(CurSelStock);
                }
            }
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        private void DeleteSubjectAndHotStockCategory()
        {
            if (CurSelCategory != null)
            {
                _SubjectAndHotService.DelSubjectAndHotStockCategory(CurSelCategory);
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
