using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace DawnQuant.App.Core.ViewModels.AShare.Bellwether
{
    public class AddBellwetherStockWindowModel 
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly BellwetherService _bellwetherService;
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;

        public AddBellwetherStockWindowModel(AShareDataMaintainService dataMaintainService,
          BellwetherService bellwetherService,
          IPassportProvider passportProvider,
            SelfSelService selfSelService)
        {
            _dataMaintainService = dataMaintainService;
            _bellwetherService = bellwetherService;
            _passportProvider = passportProvider;
            _selfSelService = selfSelService;

        }

        //自定义分类
        public BellwetherContext BellwetherContext { get; set; }

        public string InputText { get; set; }
        

        //股票列表
        ObservableCollection<BellwetherStock> _stocks;
        public ObservableCollection<BellwetherStock> Stocks { get; set; }
        

        private async void PopulateSuggestStockItems(string input)
        {
            ObservableCollection<BellwetherStock> selStockItems = new ObservableCollection<BellwetherStock>();
            await Task.Run(() =>
             {
                 if (input != null && input.Length >= 1)
                 {
                    //获取建议的数据，已经添加的股票要排除
                    var stocks = _bellwetherService.GetSuggestStocks(input);

                    //排除已经加载的数据
                    foreach (var s in stocks)
                     {
                         if (BellwetherContext.Stocks == null || BellwetherContext.Stocks.Count <= 0 ||
                         !BellwetherContext.Stocks.Where(p => p.TSCode == s.TSCode).Any())
                         {
                             selStockItems.Add(s);
                         }
                     }
                 }
             }).ConfigureAwait(true);
            Stocks = selStockItems;
        }

        private void AddStockItem(BellwetherStock item)
        {
            item.UserId = BellwetherContext.Category.UserId;
            item.CategoryId = BellwetherContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            BellwetherStock ssItem = _bellwetherService.SaveBellwetherStock(item);

            //更新完成通知主窗体更新数据
            OnStockItemAdded(ssItem);

            Task task = Task.Run(() =>
              {
                  try
                  {
                      //下载交易数据
                      _dataMaintainService.DownLoadStockData(item.TSCode);
                  }
                  catch (Exception e)
                  {
                    //  _logger.LogError($"下载股票{item.Name}交易数据失败。错误信息：{e.Message}");
                  }

              });

        }

        //自选股票事件
        public event Action<BellwetherStock> StockItemAdded;
        protected void OnStockItemAdded(BellwetherStock selfSelectStock)
        {
            if(StockItemAdded!=null)
            {
                StockItemAdded(selfSelectStock);
            }
        }
    }

}
