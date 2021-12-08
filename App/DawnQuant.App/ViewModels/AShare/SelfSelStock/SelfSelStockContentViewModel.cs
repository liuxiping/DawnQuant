using DawnQuant.App.ViewModels.AShare.Common;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.App.Models.AShare.EssentialData;

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockContentViewModel : ViewModelBase
    {
       // private readonly StockDataService _stockDataService;
        private readonly SelfSelService _selfSelService;


        public SelfSelStockContentViewModel( SelfSelService selfSelService)
        {
          //  _stockDataService = stockDataService;
            _selfSelService = selfSelService;

            AddStockItemCommand = new DelegateCommand(AddStockItem);
          
        }

        //private void OnSelfSelStockItemMessage(SelfSelStockItemMessage message)
        //{

        //    if(Category.Id== message.Item.CategoryId)
        //    {
        //        //更新界面
        //        StockItems.Add(message.Item);
        //    }
        //}

        //自定义分类
        SelfSelectStockCategory _category;
        public SelfSelectStockCategory Category
        {
            set
            { 
                SetProperty<SelfSelectStockCategory>(ref _category, value,nameof(Category));
            }
            get
            { return _category; }
        }

        //股票列表
        ObservableCollection<SelfSelectStock> _stockItems;
        public ObservableCollection<SelfSelectStock> StockItems
        {
            get
            {
                return _stockItems;
            }
            set
            {
                SetProperty<ObservableCollection<SelfSelectStock>>(ref _stockItems, value, nameof(StockItems));
            }
        }

        //是否分组
        bool _isGroupByIndustry = false;
        public bool IsGroupByIndustry
        {
            get { return _isGroupByIndustry; }
            set { SetProperty(ref _isGroupByIndustry, value, nameof(IsGroupByIndustry)); }
        }

        ////当前选择的股票
        //SelfSelStockItem _selStockItem;
        //public SelfSelStockItem CurSelectedStockItem
        //{
        //    get
        //    {
        //        return _selStockItem;
        //    }
        //    set
        //    {

        //        SetProperty(ref _selStockItem, value);
        //        //更新数据交易数据
        //        OnSelfSelStockItemChange(value);
        //    }
        //}

        ////每日指标数据
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

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelfSelStockItemChange(SelfSelectStock stockItem)
        {
            //获取交易数据
           Task t= Task.Run(() =>
            {
                StockChartViewModel model = new StockChartViewModel { TSCode= stockItem.TSCode,
                    StockName= stockItem.Name,KCycle= KCycle.Day,VA= StockChartViewModel.VisibleArea.Chart};
                //保存前一个数据选择状态
                if(StockChartViewModel!=null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                }
                model.InitPlotContext();

                this.StockChartViewModel = model;

                //每日指标数据
               // DailyIndicator = _stockDataService.GetLatestStockDailyIndicator(stockItem.TSCode);
              
               
            });

            
        }

        #region commond
        public DelegateCommand AddStockItemCommand { set; get; }

        private void AddStockItem()
        {
           // _mvxNavigationService.Navigate<AddSelfStockViewModel, SelfSelStockCategory>(Category);
        }

        #endregion

        //public override void Prepare(SelfSelStockCategory parameter)
        //{
        //    Category = parameter;
        //    _isGroupByIndustry = Category.IsGroupByIndustry;
        //    StockItems = _selfSelService.GetSelfSelStockItemsByCategory(Category.Id);
        //}

        //public override void ViewAppeared()
        //{
        //    //默认选择第一行数据
        //    if (StockItems != null && StockItems.Count > 0)
        //    {
        //        if (CurSelectedStockItem == null)
        //        {
        //            CurSelectedStockItem = StockItems[0];
        //        }
        //    }
        //}
    }
}
