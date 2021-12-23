using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.ViewModels.AShare.Common;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Models.AShare.EssentialData;
using UserProfile = DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Utils;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class StockStrategyViewModel 
    {
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyExecutorService _strategyExecutorService;
        private readonly AShareDataMaintainService _aShareDataMaintainService;
        private readonly StockStrategyService _stockStrategyService;
        private readonly SelfSelService _selfSelService;

        public StockStrategyViewModel(StrategyMetadataService strategyMetadataService,
            IPassportProvider passportProvider,
            StrategyExecutorService strategyExecutorService,
            AShareDataMaintainService aShareDataMaintainService,
            StockStrategyService stockStrategyService,
            SelfSelService selfSelService)
        {
            _strategyMetadataService = strategyMetadataService;
            _passportProvider = passportProvider;
            _strategyExecutorService = strategyExecutorService;
            _aShareDataMaintainService = aShareDataMaintainService;
            _stockStrategyService = stockStrategyService;
            _selfSelService = selfSelService;

          
            LoadCategoriesIncludeStrategies();
            LoadSelfSelectStockCategories();
           
        }

        /// <summary>
        /// 加载自选股分类
        /// </summary>
        public void LoadSelfSelectStockCategories()
        {
            //加载策略
            SelfSelectStockCategories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId); ;

        }


        /// <summary>
        /// 加载策略分类与策略
        /// </summary>
        public  void LoadCategoriesIncludeStrategies()
        {
            StockStrategyCategories = _stockStrategyService.GetCategoriesIncludeStrategiesByUser(_passportProvider.UserId); ;
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
                _curSelStock=value;
                //更新数据交易数据
                UpdateChart(value);
            }
        }

        //股票列表
        public ObservableCollection<SelfSelectStock> Stocks { get; set; }
        


        //所有分类
        public ObservableCollection<SelfSelectStockCategory> SelfSelectStockCategories { get; set; }

        //当前选择的策略分类
        public StockStrategyCategory CurSelStockStrategyCategory { get; set; }
        


        /// <summary>
        /// 策略分类
        /// </summary>
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories { get; set; }
        
        /// <summary>
        /// 绘图ViewModel
        /// </summary>
        public StockChartViewModel StockChartViewModel { get; set; }
       

        /// <summary>
        /// 股票列表选择变更 更新绘图
        /// </summary>
        /// <param name="stockItem"></param>
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

                await model.InitPlotContext();

                this.StockChartViewModel = model;

            }

        }

        /// <summary>
        /// 执行策略
        /// </summary>
        private async Task ExecuteStrategy(UserProfile.StockStrategy stockStrategy)
        {
            ObservableCollection<SelfSelectStock> stocks = null;
            await Task.Run(() =>
              {
                  //执行策略
                  stocks = _strategyExecutorService.ExecuteStrategyByContent(stockStrategy.StockStragyContent);
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

        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0, 6));
            }
        }
    }
}
