﻿using DawnQuant.App.ViewModels.AShare.Common;
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
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockViewModel : ViewModelBase
    {

        private readonly PlotDataService _stockPlotDataService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public SelfSelStockViewModel()
        {
            _stockPlotDataService = IOCUtil.Container.Resolve<PlotDataService>(); ;
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

            EmptySelfSelectCategoryCommand=new DelegateCommand(EmptySelfSelectCategory);

            //初始化 加载自选分类数据
            RefreshSelfSelectStockCategories();
        }



        /// <summary>
        /// 刷新自选股分类
        /// </summary>
        public async void RefreshSelfSelectStockCategories()
        {
            //刷新自选股分类
            ObservableCollection<SelfSelectStockCategory> categories = null;
            await Task.Run(() =>
            {
                categories= _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            }).ConfigureAwait(true);
            Categories = categories;
        }

        //所有分类
        ObservableCollection<SelfSelectStockCategory> _categories;
        public ObservableCollection<SelfSelectStockCategory> Categories
        {
            set
            { SetProperty(ref _categories, value, nameof(Categories)); }
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

                SetProperty(ref _curSelStock, value, nameof(CurSelStock));
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

                SetProperty(ref _curSelRelateStock, value, nameof(CurSelRelateStock));
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
                    AdjustedState= AdjustedState.None,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                    model.AdjustedState = StockChartViewModel.AdjustedState;
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
                  
                }).ConfigureAwait(true);

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

        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }


        #region commond
        //导入自选股
        public DelegateCommand<List<string>> ImportSelfStocksCommand { set; get; }
        private async void ImportSelfStocks(List<string> stocksId)
        {
            if (CurSelCategory != null)
            {
               await Task.Run(() =>
               {
                   var ss = _selfSelService.ImportSelfStocks(_passportProvider.UserId, stocksId, CurSelCategory.Id);
               }).ConfigureAwait(true);

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

            if (curPlotStock.CategoryId== category.Id)
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

            Task.Run(() => 
            {
                 sstock = _selfSelService.SaveSelfSelectStock(stockItem);
            }).ConfigureAwait(true);


            //当前移动的股票为自选股分类中的股票 则 删除股票
            if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
            {
                DelStockItem();
            }


            //更新当前
            if (CurSelCategory.Id == category.Id && sstock!=null)
            {
                if (!Stocks.Any(p => p.Id == sstock.Id))
                {
                    Stocks.Insert(0, sstock);
                }

            }
        }

        /// <summary>
        /// 自选分类切换
        /// </summary>
        public DelegateCommand<SelfSelectStockCategory> CategorySelChangedCommand { set; get; }
        private async void CategorySelChanged(SelfSelectStockCategory category)
        {
            //类目发生变更 加载自选股数据
            CurSelCategory = category;

            ObservableCollection<SelfSelectStock> temps = null;

            await Task.Run(() =>
              {
                  temps = _selfSelService.GetSelfSelectStocksByCategory(category.Id);

              }).ConfigureAwait(true);
            Stocks = temps;
            IsGroupByIndustry = category.IsGroupByIndustry;
            if (Stocks != null && Stocks.Count > 0)
            {
                CurSelStock = Stocks[0];
            }

        }

        /// <summary>
        /// 清空分类下的所有股票
        /// </summary>
        public DelegateCommand EmptySelfSelectCategoryCommand { set; get; }
        private async void EmptySelfSelectCategory()
        {
            if(CurSelCategory != null)
            {
                await Task.Run(() =>
                {
                    _selfSelService.EmptySelfSelectCategory(CurSelCategory.Id);
                }).ConfigureAwait(true);

                Stocks.Clear();
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

                await Task.Run(() =>
                {
                    _selfSelService.DelSelfSelectStock(CurSelStock);

                }).ConfigureAwait(true);

                Stocks.Remove(CurSelStock);

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
