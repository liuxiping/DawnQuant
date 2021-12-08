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

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class AddSelfStockWindowModel : ViewModelBase
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly SelfSelService _selfSelService;
     //   private readonly IPassportProvider _passportProvider;
        private readonly ILogger<AddSelfStockWindowModel> _logger;


        public AddSelfStockWindowModel()
        {
            _dataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>(); 
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
          //  _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _logger = IOCUtil.Container.Resolve<ILogger<AddSelfStockWindowModel>>();

            PopulateSuggestStockItemsCommand = new DelegateCommand<string>(PopulateSuggestStockItems);
            AddStockItemCommand = new DelegateCommand<SelfSelectStock>(AddStockItem);
        }

        //自定义分类
        public SelfSelectContext SelfSelectContext { get; set; }
        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set { SetProperty(ref _inputText, value, nameof(InputText)); }
        }

        //股票列表
        ObservableCollection<SelfSelectStock> _stocks ;
        public ObservableCollection<SelfSelectStock> Stocks
        {
            get
            {
                return _stocks;
            }
            set
            {
                SetProperty<ObservableCollection<SelfSelectStock>>(ref _stocks, value, nameof(Stocks));
            }
        }

        public DelegateCommand<string> PopulateSuggestStockItemsCommand { get; set; }
        private async void PopulateSuggestStockItems(string input)
        {

            if (input != null && input.Length >= 1)
            {
                ObservableCollection<SelfSelectStock> stocks = null;

                //获取建议的数据，已经添加的股票要排除

                await Task.Run(() => {
                    stocks = _selfSelService.GetSuggestStocks(input);
                }).ConfigureAwait(true);

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
        }

        public DelegateCommand<SelfSelectStock> AddStockItemCommand { get; set; }
        private async void AddStockItem(SelfSelectStock item)
        {
            item.UserId = SelfSelectContext.Category.UserId;
            item.CategoryId = SelfSelectContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            SelfSelectStock ssItem = null;
            await Task.Run( () => { 
                ssItem = _selfSelService.SaveSelfSelectStock(item); 
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
