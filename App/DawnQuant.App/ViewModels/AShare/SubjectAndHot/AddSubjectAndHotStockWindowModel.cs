using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    public class AddSubjectAndHotStockWindowModel : ViewModelBase
    {
        private readonly AShareDataMaintainService _dataMaintainService;
        private readonly SubjectAndHotService _subjectAndHotService;

        public AddSubjectAndHotStockWindowModel()
        {
            _dataMaintainService= IOCUtil.Container.Resolve<AShareDataMaintainService>(); 
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
        
            PopulateSuggestStockItemsCommand = new DelegateCommand<string>(PopulateSuggestStockItems);
            AddStockItemCommand = new DelegateCommand<SubjectAndHotStock>(AddStockItem);
        }

        //自定义分类
        public SubjectAndHotContext SubjectAndHotContext { get; set; }


        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set { SetProperty(ref _inputText, value, nameof(InputText)); }
        }

        //股票列表
        ObservableCollection<SubjectAndHotStock> _stocks;
        public ObservableCollection<SubjectAndHotStock> Stocks
        {
            get
            {
                return _stocks;
            }
            set
            {
                SetProperty<ObservableCollection<SubjectAndHotStock>>(ref _stocks, value, nameof(Stocks));
            }
        }

        public DelegateCommand<string> PopulateSuggestStockItemsCommand { get; set; }
        private async void PopulateSuggestStockItems(string input)
        {

            if (input != null && input.Length >= 1)
            {
                //获取建议的数据，已经添加的股票要排除
                ObservableCollection<SubjectAndHotStock> stocks = null;

                await Task.Run(() =>
                {
                    stocks= _subjectAndHotService.GetSuggestStocks(input);
                }).ConfigureAwait(true);

                //排除已经加载的数据
                ObservableCollection<SubjectAndHotStock> selStockItems = new ObservableCollection<SubjectAndHotStock>();

                foreach (var s in stocks)
                {
                    if (SubjectAndHotContext.Stocks == null || SubjectAndHotContext.Stocks.Count <= 0 ||
                    !SubjectAndHotContext.Stocks.Where(p => p.TSCode == s.TSCode).Any())
                    {
                        selStockItems.Add(s);
                    }
                }
                Stocks = selStockItems;
            }
        }

        public DelegateCommand<SubjectAndHotStock> AddStockItemCommand { get; set; }
        private async void AddStockItem(SubjectAndHotStock item)
        {
            item.UserId = SubjectAndHotContext.Category.UserId;
            item.CategoryId = SubjectAndHotContext.Category.Id;
            item.CreateTime = DateTime.Now;

            //保存数据
            SubjectAndHotStock ssItem = null;
            await Task.Run(() => {
                ssItem= _subjectAndHotService.SaveSubjectAndHotStock(item);
            }).ConfigureAwait(true);
             

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
