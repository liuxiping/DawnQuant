using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;

using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.ViewModels.AShare.SubjectAndHot
{
    public class AddSubjectAndHotStockWindowModel 
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly SubjectAndHotService _SubjectAndHotService;
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;


        public AddSubjectAndHotStockWindowModel(AShareDataMaintainService dataMaintainService,
          SubjectAndHotService SubjectAndHotService,
          IPassportProvider passportProvider,
         SelfSelService selfSelService)
        {
            _dataMaintainService= dataMaintainService; 
            _SubjectAndHotService = SubjectAndHotService;
            _passportProvider = passportProvider;
            _selfSelService = selfSelService;

        }

        //自定义分类
        public SubjectAndHotContext SubjectAndHotContext { get; set; }


        public string InputText { get; set; }
       

        //股票列表
        public ObservableCollection<SubjectAndHotStock> Stocks { get; set; }


        private void PopulateSuggestStockItems(string input)
        {
            Task.Run(() =>
            {
                if (input != null && input.Length >= 1)
                {
                    //获取建议的数据，已经添加的股票要排除
                    var stocks = _SubjectAndHotService.GetSuggestStocks(input);

                    //排除已经加载的数据
                    ObservableCollection<SubjectAndHotStock> selStockItems = new ObservableCollection<SubjectAndHotStock>();


                    foreach (var s in stocks)
                    {
                        if (SubjectAndHotContext.Stocks==null || SubjectAndHotContext.Stocks.Count<=0 ||
                        !SubjectAndHotContext.Stocks.Where(p => p.TSCode == s.TSCode).Any())
                        {
                            selStockItems.Add(s);
                        }
                    }

                    Stocks = selStockItems;
                }
            });
            
        }

        private void AddStockItem(SubjectAndHotStock item)
        {
            item.UserId = SubjectAndHotContext.Category.UserId;
            item.CategoryId = SubjectAndHotContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            SubjectAndHotStock ssItem = _SubjectAndHotService.SaveSubjectAndHotStock(item);

            //更新完成通知主窗体更新数据
            OnStockItemAdded(ssItem);

            Task task = Task.Run(() =>
              {
                  //下载交易数据
                  _dataMaintainService.DownLoadStockData(item.TSCode);
                 
              });
           
        }

        //自选股票事件
        public event Action<SubjectAndHotStock> StockItemAdded;
        protected void OnStockItemAdded(SubjectAndHotStock selfSelectStock)
        {
            if(StockItemAdded!=null)
            {
                StockItemAdded(selfSelectStock);
            }
        }
    }

}
