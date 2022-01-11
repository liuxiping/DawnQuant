using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DawnQuant.App.ViewModels.AShare.Common;
using DawnQuant.App.Models.AShare.EssentialData;
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    class SubjectAndHotViewModel : ViewModelBase
    {

        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly IPassportProvider _passportProvider;
        private readonly AShareDataMaintainService _dataMaintainService;


        public SubjectAndHotViewModel()
        {
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _dataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();


            DelStockItemCommand = new DelegateCommand(DelStockItem);
            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);
            CopyStockNameCommand = new DelegateCommand(CopyStockName);

            CopyFocusStockCodeCommand = new DelegateCommand(CopyFocusStockCode);
            CopyFocusStockNameCommand = new DelegateCommand(CopyFocusStockName);

            CopyPlotStockCodeCommand=new DelegateCommand(CopyPlotStockCode);
            CopyPlotStockNameCommand= new DelegateCommand(CopyPlotStockName);

            DeleteSubjectAndHotStockCategoryCommand = new DelegateCommand(DeleteSubjectAndHotStockCategory);
            DelStockItemCommand = new DelegateCommand(DelStockItem);

            AddToCurSubjectAndHotCommand=new DelegateCommand(AddToCurSubjectAndHot);
            SetFocusStockCommand=new DelegateCommand(SetFocusStock);
            CancelFocusStockCommand=new DelegateCommand(CancelFocusStock);

            MoveCategoryToTopCommand = new DelegateCommand(MoveCategoryToTop);
            MoveCategoryToBottomCommand = new DelegateCommand(MoveCategoryToBottom);

            RefreshCategoryCommand=new DelegateCommand(RefreshCategory);

            //初始化 加载题材热点分类数据
            Initialize();
        }

      
        public void Initialize()
        {
            RefreshSubjectAndHotStockCategories();
        }


       

        public void RefreshSubjectAndHotStockCategories()
        {
                LoadCategories();
        }

        /// <summary>
        /// 加载题材概念分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<SubjectAndHotStockCategory> categories = null;

            await Task.Run(() =>
            {
                categories = _subjectAndHotService.GetSubjectAndHotStockCategories();

            }).ConfigureAwait(true);

            var curSelCategory = CurSelCategory;

            Categories = categories;

            if (Categories == null || Categories.Count == 0)
            {
                CurSelCategory = null;
            }
            else
            {
                if (curSelCategory != null )
                {
                    CurSelCategory = Categories.Where(p => p.Id == curSelCategory.Id).FirstOrDefault();
                    //找不到 选择第一条
                    if(CurSelCategory == null)
                    {
                        CurSelCategory = Categories.First();
                    }
                }
                else
                {
                    CurSelCategory = Categories.First();
                }
                
            }

            
        }


        //所有题材概念分类
        ObservableCollection<SubjectAndHotStockCategory> _categories;
        public ObservableCollection<SubjectAndHotStockCategory> Categories
        {
            set
            { 
                SetProperty(ref _categories, value, nameof(Categories));
            }
            get
            { 
                return _categories;
            }
        }

        /// <summary>
        /// 当前选择的题材概念分类
        /// </summary>
        SubjectAndHotStockCategory _curSelCategory;
        public SubjectAndHotStockCategory CurSelCategory
        {
            set
            { 
                SetProperty(ref _curSelCategory, value, nameof(CurSelCategory));
                OnCurSelCategoryChange();
            }
            get
            { return _curSelCategory; }
        }

        /// <summary>
        /// 题材概念分类选择变更
        /// </summary>
        private async void OnCurSelCategoryChange()
        {
            if (CurSelCategory != null)
            {

                ObservableCollection<SubjectAndHotStock> stocks = null;

                await Task.Run(() => {
                    stocks = _subjectAndHotService.GetSubjectAndHotStocksByCategory(CurSelCategory.Id);
                }).ConfigureAwait(true);

                Stocks = stocks;

                if (Stocks != null && Stocks.Count > 0)
                {

                    if(FocusStocks==null)
                    {
                        FocusStocks = new ObservableCollection<SubjectAndHotStock>();
                    }
                    //过滤重点关注股票
                    FocusStocks.Clear();
                    Stocks.Where(p=>p.IsFocus).ToList().ForEach(p => FocusStocks.Add(p));   

                    if(FocusStocks.Count>0)
                    {
                        CurSelFocusStock=FocusStocks[0];
                    }
                    else
                    {
                        CurSelStock = Stocks[0];
                    }
                }
                else
                {
                    FocusStocks?.Clear();
                }
            }
        }

        //题材概念股票列表
        ObservableCollection<SubjectAndHotStock> _stocks = new ObservableCollection<SubjectAndHotStock>();
        public ObservableCollection<SubjectAndHotStock> Stocks
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


        //当前选择的题材概念
        SubjectAndHotStock _curSelStock;
        public SubjectAndHotStock CurSelStock
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

        
        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }


        //相关股票列表
        ObservableCollection<SubjectAndHotStock> _focusStocks;
        public ObservableCollection<SubjectAndHotStock> FocusStocks
        {
            get
            {
                return _focusStocks;
            }
            set
            {
                SetProperty(ref _focusStocks, value, nameof(FocusStocks));
            }
        }


        //当前选择的相关股票
        SubjectAndHotStock _curSelFocusStock;
        public SubjectAndHotStock CurSelFocusStock
        {
            get
            {
                return _curSelFocusStock;
            }
            set
            {

                SetProperty(ref _curSelFocusStock, value, nameof(CurSelFocusStock));
                //更新数据交易数据
                OnSelFocusStockChange(value);
            }
        }

        private void OnSelFocusStockChange(SubjectAndHotStock stockItem)
        {
            UpdateChart(stockItem);
        }

        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelStockChange(SubjectAndHotStock stockItem)
        {
            UpdateChart(stockItem);

        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private  async void  UpdateChart(SubjectAndHotStock stockItem)
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
                    AdjustedState = AdjustedState.None,
                    Industry= stockItem.Industry,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                    model.AdjustedState = StockChartViewModel.AdjustedState;
                }

                await model.InitPlotContext();

                this.StockChartViewModel = model;


            }
            
        }


        public  DelegateCommand RefreshCategoryCommand { get; set; }
        private void RefreshCategory()
        {
            LoadCategories();
        }


        /// <summary>
        /// 分类置顶
        /// </summary>
        public DelegateCommand MoveCategoryToTopCommand { set; get; }
        private void MoveCategoryToTop()
        {
            if (CurSelCategory != null)
            {
                var cur=CurSelCategory;
                Categories.Remove(cur);
                Categories.Insert(0, cur);
                int i = 1;
                foreach (var c in Categories)
                {
                    c.SortNum = i;
                    i++;
                }

                ObservableCollection<SubjectAndHotStockCategory> categories = new ObservableCollection<SubjectAndHotStockCategory>();
                Categories.OrderBy(c => c.SortNum).ToList().ForEach(c=>categories.Add(c));
                Categories = categories;
                CurSelCategory = cur;

                //保存
                Task.Run(() => 
                {
                    foreach (var c in Categories)
                    {
                        _subjectAndHotService.SaveSubjectAndHotCategory(c);
                    }
                });

            }
        }

        /// <summary>
        /// 分类置底
        /// </summary>
        public DelegateCommand MoveCategoryToBottomCommand { set; get; }
        private void MoveCategoryToBottom()
        {
            if (CurSelCategory != null)
            {
                var cur = CurSelCategory;
                Categories.Remove(cur);
                Categories.Add(cur);
                int i = 1;
                foreach (var c in Categories)
                {
                    c.SortNum = i;
                    i++;
                }

                ObservableCollection<SubjectAndHotStockCategory> categories = new ObservableCollection<SubjectAndHotStockCategory>();
                Categories.OrderBy(c => c.SortNum).ToList().ForEach(c => categories.Add(c));
                Categories = categories;
                CurSelCategory = cur;

                //保存
                Task.Run(() =>
                {
                    foreach (var c in Categories)
                    {
                        _subjectAndHotService.SaveSubjectAndHotCategory(c);
                    }
                });
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
                if (CurSelStock.TSCode == StockChartViewModel.PlotContext.TSCode)
                {
                    await Task.Run(() =>
                    {
                        _subjectAndHotService.DelSubjectAndHotStock(CurSelStock);
                    }).ConfigureAwait(true);
                    Stocks.Remove(CurSelStock);
                }
            }
        }


        /// <summary>
        /// 删除分类
        /// </summary>
        public DelegateCommand DeleteSubjectAndHotStockCategoryCommand { set; get; }
        private async void DeleteSubjectAndHotStockCategory()
        {
            if (CurSelCategory != null)
            {
                //删除之后设置自动选择下一个
                SubjectAndHotStockCategory next = null;
                var categories = Categories.ToList();

                var index= categories.FindIndex(0,p=>p.Id==CurSelCategory.Id);

                if(index>0)
                {
                    if(index== categories.Count-1)
                    {
                        next = categories[index-1];
                    }
                    else if(index<categories.Count-1)
                    {
                        next = categories[index + 1];
                    }
                }

                await Task.Run(() =>
                {
                    _subjectAndHotService.DelSubjectAndHotStockCategory(CurSelCategory);
                }).ConfigureAwait(true);

                CurSelCategory = next;

                LoadCategories();

            }
        }


        public DelegateCommand AddToCurSubjectAndHotCommand { set; get; }
        private async void AddToCurSubjectAndHot()
        {
            if (StockChartViewModel != null)
            {
                if (Stocks == null || !Stocks.Any(p => p.TSCode == StockChartViewModel.TSCode))
                {
                    SubjectAndHotStock item = new SubjectAndHotStock();

                    item.UserId = CurSelCategory.UserId;
                    item.CategoryId = CurSelCategory.Id;
                    item.TSCode = StockChartViewModel.TSCode;
                    item.Name = StockChartViewModel.Name;
                    item.Industry = StockChartViewModel.Industry;
                    item.CreateTime = DateTime.Now;

                    //保存数据
                    SubjectAndHotStock ssItem = null;
                    await Task.Run(() =>
                    {
                        ssItem = _subjectAndHotService.SaveSubjectAndHotStock(item);
                    }).ConfigureAwait(true);

                    Stocks.Insert(0, ssItem);

                    await Task.Run(() => {
                        _dataMaintainService.DownLoadStockData(ssItem.TSCode);
                    });
                }
            }

        }

        public DelegateCommand CancelFocusStockCommand { set; get; }
        private void CancelFocusStock()
        {
            if (CurSelFocusStock != null)
            {
                CurSelFocusStock.IsFocus = false;

                FocusStocks.Remove(CurSelFocusStock);

                //重新绑定数据
                ObservableCollection<SubjectAndHotStock> stocks = new ObservableCollection<SubjectAndHotStock>();
                FocusStocks.ToList().ForEach(s => stocks.Add(s));
                FocusStocks = stocks;

                Task.Run(() =>
                {
                    _subjectAndHotService.SaveSubjectAndHotStock(CurSelFocusStock);

                });
            }
            

            
        }
        public DelegateCommand SetFocusStockCommand { set; get; }
        private void SetFocusStock()
        {
            if(CurSelStock!=null)
            {
                if (FocusStocks != null &&
                    !FocusStocks.Where(p => p.TSCode == CurSelStock.TSCode).Any())
                {
                    CurSelStock.IsFocus = true;

                    FocusStocks.Insert(0, CurSelStock);

                    //重新绑定数据
                    ObservableCollection<SubjectAndHotStock> stocks = new ObservableCollection<SubjectAndHotStock>();
                    FocusStocks.ToList().ForEach(s => stocks.Add(s));
                    FocusStocks = stocks;

                    Task.Run(() => 
                    {
                        _subjectAndHotService.SaveSubjectAndHotStock(CurSelStock);

                    });


                   

                }
            }
           
        }

        public DelegateCommand CopyPlotStockCodeCommand { set; get; }


        private  void CopyPlotStockCode()
        {
            if(StockChartViewModel!=null)
            {
                TextCopy.ClipboardService.SetText(StockChartViewModel.TSCode.Substring(0, 6));

            }
        }

        public DelegateCommand CopyPlotStockNameCommand { set; get; }

        private  void CopyPlotStockName()
        {
            if (StockChartViewModel != null)
            {
                TextCopy.ClipboardService.SetText(StockChartViewModel.Name);

            }
        }
        public DelegateCommand CopyStockCodeCommand { set; get; }
        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.TSCode.Substring(0, 6));
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


        public DelegateCommand CopyFocusStockCodeCommand { set; get; }
        private void CopyFocusStockCode()
        {
            if (CurSelFocusStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelFocusStock.TSCode.Substring(0, 6));
            }
        }

        public DelegateCommand CopyFocusStockNameCommand { set; get; }
        private void CopyFocusStockName()
        {
            if (CurSelFocusStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelFocusStock.Name);
            }
        }


       
    }
}
