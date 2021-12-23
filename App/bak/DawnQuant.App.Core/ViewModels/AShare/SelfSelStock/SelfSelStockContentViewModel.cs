using DawnQuant.App.Core.ViewModels.AShare.Common;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.Models.AShare.EssentialData;

namespace DawnQuant.App.Core.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockContentViewModel 
    {
        private readonly SelfSelService _selfSelService;


        public SelfSelStockContentViewModel( SelfSelService selfSelService)
        {
          //  _stockDataService = stockDataService;
            _selfSelService = selfSelService;

          
        }

       

        //自定义分类
        public SelfSelectStockCategory Category { get; set; }
        

        //股票列表
        public ObservableCollection<SelfSelectStock> StockItems { get; set; }
        

        //是否分组
        public bool IsGroupByIndustry { get; set; }
       

        

        public StockChartViewModel StockChartViewModel { get; set; }
        

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelfSelStockItemChange(SelfSelectStock stockItem)
        {
            //获取交易数据
           Task t= Task.Run(() =>
            {
                StockChartViewModel model = new StockChartViewModel (null){ TSCode= stockItem.TSCode,
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
