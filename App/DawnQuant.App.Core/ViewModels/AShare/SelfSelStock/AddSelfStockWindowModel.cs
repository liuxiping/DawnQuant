using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace DawnQuant.App.Core.ViewModels.AShare.SelfSelStock
{
    public class AddSelfStockWindowModel 
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public AddSelfStockWindowModel(AShareDataMaintainService dataMaintainService,
           SelfSelService selfSelService,
           IPassportProvider passportProvider)
        {
            _dataMaintainService = dataMaintainService;
            _selfSelService = selfSelService;
            _passportProvider = passportProvider;

        }

        //自定义分类
        public SelfSelectContext SelfSelectContext { get; set; }

        public string InputText { get; set; }
        

        //股票列表
        public ObservableCollection<SelfSelectStock> Stocks { get; set; }
        

        private void PopulateSuggestStockItems(string input)
        {
            Task.Run(() =>
            {
                if (input != null && input.Length >= 1)
                {
                    //获取建议的数据，已经添加的股票要排除
                    var stocks = _selfSelService.GetSuggestStocks(input);

                    //排除已经加载的数据
                    ObservableCollection<SelfSelectStock> selStockItems = new ObservableCollection<SelfSelectStock>();
                    foreach (var s in stocks)
                    {
                        if (!SelfSelectContext.Stocks.Where(p => p.TSCode == s.TSCode).Any())
                        {
                            selStockItems.Add(s);
                        }
                    }
                     Stocks = selStockItems;

                }
            });
        }

        private void AddStockItem(SelfSelectStock item)
        {
            item.UserId = SelfSelectContext.Category.UserId;
            item.CategoryId = SelfSelectContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            SelfSelectStock ssItem = _selfSelService.SaveSelfSelectStock(item);

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
                      //_logger.LogError($"下载股票{item.Name}交易数据失败。错误信息：{e.Message}");
                  }

              });
           
        }

        //自选股票事件
        public event Action<SelfSelectStock> StockItemAdded;
        protected void OnStockItemAdded(SelfSelectStock selfSelectStock)
        {
            if(StockItemAdded!=null)
            {
                StockItemAdded(selfSelectStock);
            }
        }
    }

}
