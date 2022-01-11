using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.App.ViewModels.AShare.Common;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THS=DawnQuant.App.Models.AShare.EssentialData;

namespace DawnQuant.App.ViewModels.AShare.THSDataAnalysis.THSIndex
{
    public class THSIndexViewModel:ViewModelBase
    {
        private readonly PlotDataService _stockPlotDataService;
        private readonly THSIndexService  _thsIndexService;
        private readonly IPassportProvider _passportProvider;
        
        public THSIndexViewModel()
        {
            _stockPlotDataService = IOCUtil.Container.Resolve<PlotDataService>(); ;
            _thsIndexService = IOCUtil.Container.Resolve<THSIndexService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();

            CopyStockCodeCommand = new DelegateCommand(CopyStockCode);
            CopyStockNameCommand = new DelegateCommand(CopyStockName);

            CopyTHSIndexCodeCommand = new DelegateCommand(CopyTHSIndexCode);
            CopyTHSIndexNameCommand = new DelegateCommand(CopyTHSIndexName);


            AddToSubjectAndHotCommand=new DelegateCommand(AddToSubjectAndHot);
            //初始化 加载自选分类数据

            CurSelTHSIndexType =THSIndexType.AllSupportType[0];

        }


        //所有指数类型
        public List<THSIndexType> THSIndexTypes
        {
            get
            {
                return THSIndexType.AllSupportType;
            }
        }

        /// <summary>
        /// 当前选择的指数类型
        /// </summary>
        THSIndexType _curSelTHSIndexType ;
        public THSIndexType CurSelTHSIndexType 
        {
            set
            {
                SetProperty(ref _curSelTHSIndexType, value, nameof(CurSelTHSIndexType));
                OnSelTHSIndexTypeChange(value);
            }
            get
            { return _curSelTHSIndexType; }
        }

        //所有同花顺指数
        ObservableCollection<THS.THSIndex> _allTHSIndexes=null;
        private async void OnSelTHSIndexTypeChange(THSIndexType type)
        {
            if (THSIndexes == null)
            {
                THSIndexes = new ObservableCollection<THS.THSIndex>();
            }
            else
            {
                THSIndexes.Clear();
            }

            if (_allTHSIndexes == null)
            {
                await Task.Run(() =>
                {
                    _allTHSIndexes = _thsIndexService.GetAllTHSIndex();
                })
                .ConfigureAwait(true);
            }

            switch (type.Code)
            {
                case "A":
                     _allTHSIndexes.Where(p=>p.ListDate!=null)
                        .DistinctBy(p=>p.TSCode).ToList().ForEach(p => THSIndexes.Add(p));
                    break;
                case "N":
                    _allTHSIndexes.Where(p => p.ListDate != null && p.Type=="N")
                       .DistinctBy(p => p.TSCode).ToList().ForEach(p => THSIndexes.Add(p));
                    break;
                case "I":
                    _allTHSIndexes.Where(p => p.ListDate != null && p.Type == "I")
                      .DistinctBy(p => p.TSCode).ToList().ForEach(p => THSIndexes.Add(p));
                    break;
                case "S":
                    _allTHSIndexes.Where(p => p.ListDate != null && p.Type == "S")
                      .DistinctBy(p => p.TSCode).ToList().ForEach(p => THSIndexes.Add(p));
                    break;
                default:
                    break;
            }
        }


        //同花顺指数
        ObservableCollection<THS.THSIndex> _thsIndexes;
        public ObservableCollection<THS.THSIndex> THSIndexes
        {
            set
            { 
                SetProperty(ref _thsIndexes, value, nameof(THSIndexes));
            }
            get
            { return _thsIndexes; }
        }


        /// <summary>
        /// 当前选择的指数
        /// </summary>
        THS.THSIndex _curSelTHSIndex;
        public THS.THSIndex CurSelTHSIndex
        {
            set
            {
                SetProperty(ref _curSelTHSIndex, value, nameof(CurSelTHSIndex));
                OnSelTHSIndexChange(value);
            }
            get
            { return _curSelTHSIndex; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void OnSelTHSIndexChange(THS.THSIndex index)
        {
            if (index != null)
            {
                UpdateTHSIndexMembers(index.TSCode);
                UpdateChart(index);
            }
        }

        //指数成分股票列表
        ObservableCollection<THSIndexMember> _stocks;
        public ObservableCollection<THSIndexMember> Stocks
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


        //当前选择的股票
        THSIndexMember  _curSelStock;
        public THSIndexMember CurSelStock
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


        /// <summary>
        /// 股票列表选择变更
        /// </summary>
        /// <param name="stockItem"></param>
        private void OnSelStockChange(THSIndexMember stockItem)
        {
            if (stockItem != null)
            {
                UpdateChart(stockItem);
            }
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private async void UpdateChart(THSIndexMember indexMember)
        {
            if (indexMember != null)
            {
                //获取交易数据

                StockChartViewModel model = new StockChartViewModel()
                {
                    TSCode = indexMember.Code,
                    Name = indexMember.Name,
                    KCycle = KCycle.Day,
                    VA = VisibleArea.Chart,
                    AdjustedState = AdjustedState.None,
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                    model.AdjustedState = StockChartViewModel.AdjustedState;
                }

                await model.InitPlotContext().ConfigureAwait(true);

                this.StockChartViewModel = model;
            }
        }


        /// <summary>
        /// 更新绘图数据
        /// </summary>
        private async void UpdateChart(THS.THSIndex index)
        {
            if (index != null)
            {
                //获取交易数据

                StockChartViewModel model = new StockChartViewModel()
                {
                    TSCode = index.TSCode,
                    Name = index.Name,
                    KCycle = KCycle.Day,
                    VA = VisibleArea.Chart,
                    TradeDataType= TradeDataType.THSIndex,
                    
                };
                //保存前一个数据选择状态
                if (StockChartViewModel != null)
                {
                    model.VA = StockChartViewModel.VA;
                    model.KCycle = StockChartViewModel.KCycle;
                   
                }

                await model.InitPlotContext().ConfigureAwait(true);

                this.StockChartViewModel = model;
            }
        }
        /// <summary>
        /// 更新成分股
        /// </summary>
        /// <param name="tsCode"></param>
        private async void UpdateTHSIndexMembers(string tsCode)
        {
            if (tsCode != null)
            {
                ObservableCollection<THSIndexMember> temprs = null;
                await Task.Run(() =>
                {
                    temprs = _thsIndexService.GetTHSIndexMembersByTSCode(tsCode);

                }).ConfigureAwait(true);

                Stocks = temprs;
            }


        }

        StockChartViewModel _stockChartViewModel;
        public StockChartViewModel StockChartViewModel
        {
            get { return _stockChartViewModel; }
            set { SetProperty(ref _stockChartViewModel, value, nameof(StockChartViewModel)); }
        }


        #region commond

        public DelegateCommand AddToSubjectAndHotCommand { set; get; }

        private void AddToSubjectAndHot()
        {
            if(CurSelTHSIndex!=null)
            {
                Task.Run(() => 
                {
                    _thsIndexService.AddToSubjectAndHot(CurSelTHSIndex.TSCode);
                });
                
            }
        }


        public DelegateCommand CopyTHSIndexCodeCommand { set; get; }
        private void CopyTHSIndexCode()
        {
            if (CurSelTHSIndex != null)
            {
                TextCopy.ClipboardService.SetText(CurSelTHSIndex.TSCode.Substring(0, 6));
            }
        }

        public DelegateCommand CopyTHSIndexNameCommand { set; get; }
        private void CopyTHSIndexName()
        {
            if (CurSelTHSIndex != null)
            {
                TextCopy.ClipboardService.SetText(CurSelTHSIndex.Name);
            }
        }


        public DelegateCommand CopyStockCodeCommand { set; get; }
        private void CopyStockCode()
        {
            if (CurSelStock != null)
            {
                TextCopy.ClipboardService.SetText(CurSelStock.Code.Substring(0, 6));
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

        #endregion




    }

}
