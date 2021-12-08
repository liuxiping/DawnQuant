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

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    class SubjectAndHotViewModel : ViewModelBase
    {

        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly IPassportProvider _passportProvider;


        public SubjectAndHotViewModel()
        {
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();


            DelStockItemCommand = new DelegateCommand(DelStockItem);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);
            CopyStockNameCommand = new DelegateCommand(CopyStockName);

            CopyRelateStockCodeCommand = new DelegateCommand(CopyRelateStockCode);
            CopyRelateStockNameCommand = new DelegateCommand(CopyRelateStockName);

            DeleteSubjectAndHotStockCategoryCommand = new DelegateCommand(DeleteSubjectAndHotStockCategory);
            DelStockItemCommand = new DelegateCommand(DelStockItem);

            //初始化 加载题材热点分类数据
            Initialize();
        }

        public Dispatcher Dispatcher { get; set; }

        public void Initialize()
        {
            RefreshSubjectAndHotStockCategories();
        }


        public void RefreshSubjectAndHotStockCategories()
        {
           
                LoadCategories();
          
        }

        /// <summary>
        /// 加载题材概念分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<SubjectAndHotStockCategory> categories = null;

            await Task.Run(() =>
            {
                categories = _subjectAndHotService.GetSubjectAndHotStockCategories(_passportProvider.UserId);

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


        //所有题材概念分类
        ObservableCollection<SubjectAndHotStockCategory> _categories;
        public ObservableCollection<SubjectAndHotStockCategory> Categories
        {
            set
            { 
                SetProperty(ref _categories, value, nameof(Categories));
            }
            get
            { 
                return _categories;
            }
        }

        /// <summary>
        /// 当前选择的题材概念分类
        /// </summary>
        SubjectAndHotStockCategory _curSelCategory;
        public SubjectAndHotStockCategory CurSelCategory
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
        /// 题材概念分类选择变更
        /// </summary>
        private async void OnCurSelCategoryChange()
        {
            if (CurSelCategory != null)
            {

                ObservableCollection<SubjectAndHotStock> stocks = null;

                await Task.Run(() => {
                    stocks = _subjectAndHotService.GetSubjectAndHotStocksByCategory(CurSelCategory.Id);
                }).ConfigureAwait(true);
                Stocks = stocks;

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

        //题材概念股票列表
        ObservableCollection<SubjectAndHotStock> _stocks = new ObservableCollection<SubjectAndHotStock>();
        public ObservableCollection<SubjectAndHotStock> Stocks
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
        ObservableCollection<SubjectAndHotStock> _relateStocks;
        public ObservableCollection<SubjectAndHotStock> RelateStocks
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
        SubjectAndHotStock _curSelRelateStock;
        public SubjectAndHotStock CurSelRelateStock
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
                    model.AdjustedState = StockChartViewModel.AdjustedState;
                }

                await model.InitPlotContext();

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
                ObservableCollection<SubjectAndHotStock> stocks = null;

                await Task.Run(() =>
                {
                    stocks = _subjectAndHotService.GetSameIndustryStocks(tsCode);
                }).ConfigureAwait(true);

                RelateStocks = stocks;
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
                        _subjectAndHotService.DelSubjectAndHotStock(CurSelStock);
                    }).ConfigureAwait(true);
                    Stocks.Remove(CurSelStock);
                }
            }
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        public DelegateCommand DeleteSubjectAndHotStockCategoryCommand { set; get; }
        private async void DeleteSubjectAndHotStockCategory()
        {
            if (CurSelCategory != null)
            {
                await Task.Run(() =>
                {
                    _subjectAndHotService.DelSubjectAndHotStockCategory(CurSelCategory);
                }).ConfigureAwait(true);
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
