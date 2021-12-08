using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.Bellwether
{
    public class AddBellwetherStockWindowModel : ViewModelBase
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly BellwetherService _bellwetherService;
      //  private readonly IPassportProvider _passportProvider;
       // private readonly SelfSelService _selfSelService;
        private readonly ILogger<AddBellwetherStockWindowModel> _logger;

        public AddBellwetherStockWindowModel()
        {
            _dataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _bellwetherService = IOCUtil.Container.Resolve<BellwetherService>();
           // _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
          //  _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
            _logger = IOCUtil.Container.Resolve<ILogger<AddBellwetherStockWindowModel>>();

            PopulateSuggestStockItemsCommand = new DelegateCommand<string>(PopulateSuggestStockItems);
            AddStockItemCommand = new DelegateCommand<BellwetherStock>(AddStockItem);
        }

        //自定义分类
        public BellwetherContext BellwetherContext { get; set; }

        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set { SetProperty(ref _inputText, value, nameof(InputText)); }
        }

        //股票列表
        ObservableCollection<BellwetherStock> _stocks;
        public ObservableCollection<BellwetherStock> Stocks
        {
            get
            {
                return _stocks;
            }
            set
            {
                SetProperty<ObservableCollection<BellwetherStock>>(ref _stocks, value, nameof(Stocks));
            }
        }

        public DelegateCommand<string> PopulateSuggestStockItemsCommand { get; set; }
        private async void PopulateSuggestStockItems(string input)
        {
            ObservableCollection<BellwetherStock> selStockItems = new ObservableCollection<BellwetherStock>();

            if (input != null && input.Length >= 1)
            {
                //获取建议的数据，已经添加的股票要排除
                ObservableCollection<BellwetherStock> stocks = null;
                await Task.Run(() =>
                {
                    stocks = _bellwetherService.GetSuggestStocks(input);
                }).ConfigureAwait(true);

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
            Stocks = selStockItems;
        }

        public DelegateCommand<BellwetherStock> AddStockItemCommand { get; set; }
        private async void AddStockItem(BellwetherStock item)
        {
            item.UserId = BellwetherContext.Category.UserId;
            item.CategoryId = BellwetherContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            BellwetherStock ssItem = null;
            await Task.Run(() =>
            {
                ssItem = _bellwetherService.SaveBellwetherStock(item);
            }).ConfigureAwait(true);
               
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
                      _logger.LogError($"下载股票{item.Name}交易数据失败。错误信息：{e.Message}");
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
