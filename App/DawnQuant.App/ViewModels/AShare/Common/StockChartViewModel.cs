using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.Common
{
    public class StockChartViewModel : ViewModelBase
    {

        private readonly PlotDataService _plotDataService;


        public TradeDataType TradeDataType { get; set; } = TradeDataType.Stock;

        /// <summary>
        /// 是否显示复权菜单
        /// </summary>
        public bool ShowAdjustMenu
        {
            get
            {
                if (TradeDataType == TradeDataType.Stock)
                {
                    return true;
                }
                else if (TradeDataType == TradeDataType.THSIndex)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// F10 按钮名称
        /// </summary>
        public string F10Caption
        {
            get
            {
                if (TradeDataType == TradeDataType.Stock)
                {
                    return "个股资料";
                }
                else if (TradeDataType == TradeDataType.THSIndex)
                {
                    return "指数资料";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 显示实际换手率
        /// </summary>
        public bool ShowTurnoverFree
        {
            get
            {
                if (TradeDataType == TradeDataType.Stock)
                {
                    return true;
                }
                else if (TradeDataType == TradeDataType.THSIndex)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    
        public string TSCode { get; set; }

        public string Name { get; set; }

        public string Industry { get; set; }

        public KCycle KCycle { get; set; }

        //页面数据大小
        int _tradeDataPageSize = 500;

        public StockChartViewModel()
        {

            _plotDataService = IOCUtil.Container.Resolve<PlotDataService>();

            #region Command Init
            ShowDayCycleCommand = new DelegateCommand(ShowDayCycle);
            ShowWeekCycleCommand = new DelegateCommand(ShowWeekCycle);
            ShowMonthCycleCommand = new DelegateCommand(ShowMonthCycle);
            ShowM30CycleCommand = new DelegateCommand(ShowM30Cycle);
            ShowM60CycleCommand = new DelegateCommand(ShowM60Cycle);
            ShowM120CycleCommand = new DelegateCommand(ShowM120Cycle);
            ShowStockInfoCommand = new DelegateCommand(ShowStockInfo);
            #endregion

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public async Task InitPlotContext()
        {
            if (!string.IsNullOrEmpty(TSCode))
            {

                if (KCycle == KCycle.Day || KCycle == KCycle.Week ||
                 KCycle == KCycle.Month || KCycle == KCycle.Minute30 ||
                 KCycle == KCycle.Minute60 || KCycle == KCycle.Minute120)
                {
                    PlotContext pc = null;
                    await Task.Run(() =>
                     {

                         if (TradeDataType == TradeDataType.Stock)
                         {
                             pc = _plotDataService.GetStockPlotContext(TSCode, KCycle, _tradeDataPageSize, AdjustedState);
                         }
                         else if(TradeDataType== TradeDataType.THSIndex)
                         {
                             pc = _plotDataService.GetTHSIndexPlotContext(TSCode, KCycle, _tradeDataPageSize);

                         }

                     }).ConfigureAwait(true);
                    pc.Name = Name;
                    PlotContext = pc;
                }
                else
                {
                    throw new NotSupportedException("不支持的K线周期");
                }
            }

        }


        #region   属性
        //绘图相关数据
        PlotContext _plotContext;
        public PlotContext PlotContext
        {
            get { return _plotContext; }
            set { SetProperty(ref _plotContext, value, nameof(PlotContext)); }
        }

        VisibleArea _va = VisibleArea.Chart;
        public VisibleArea VA//显示区域
        {
            get { return _va; }
            set { SetProperty(ref _va, value, nameof(VA)); }
        }

        public string F10Url
        {
            get { return "http://basic.10jqka.com.cn/" + TSCode.Substring(0, 6) + "/"; }

        }


        IList<PlotData> _selectedStockItems;
        public IList<PlotData> SelectedStockItems
        {
            set
            {
                if (SetProperty(ref _selectedStockItems, value, nameof(SelectedStockItems)))
                {
                    OnSelectedStockItemsChange();
                }
              
            }
            get {
                return _selectedStockItems;
            }
        }

        private void OnSelectedStockItemsChange()
        {

        }

        //当前节点交易信息
        CurSelectStockInfo _curSelStockInfo;
        public CurSelectStockInfo CurSelStockInfo
        {
            set { SetProperty(ref _curSelStockInfo, value, nameof(CurSelStockInfo)); }
            get { return _curSelStockInfo; }
        }



        //复权信息
        AdjustedState   _adjustedState= AdjustedState.None;
        public AdjustedState AdjustedState
        {
            set
            {
                if (SetProperty(ref _adjustedState, value, nameof(AdjustedState)))
                {
                    OnAdjustedStateChange();
                }
            }
            get { return _adjustedState; }
        }

        private async void OnAdjustedStateChange()
        {
            //重新绑定数据
           await InitPlotContext();
        }
        #endregion


        #region Command
        public DelegateCommand ShowDayCycleCommand { set; get; }
        private async void ShowDayCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Day)
            {
                KCycle = KCycle.Day;
              await  InitPlotContext();
            }

        }

        public DelegateCommand ShowWeekCycleCommand { set; get; }
        private async void ShowWeekCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Week)
            {
                KCycle = KCycle.Week;

              await  InitPlotContext();
            }

        }

        public DelegateCommand ShowMonthCycleCommand { set; get; }
        private async void ShowMonthCycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Month)
            {
                KCycle = KCycle.Month;
              await  InitPlotContext();
            }

        }

        public DelegateCommand ShowM30CycleCommand { set; get; }
        private async void ShowM30Cycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Minute30)
            {
                KCycle = KCycle.Minute30;
               await InitPlotContext();
            }

        }

        public DelegateCommand ShowM60CycleCommand { set; get; }

        private async void ShowM60Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute60)
            {
             
                KCycle = KCycle.Minute60;
               await InitPlotContext();
            }

        }
        public DelegateCommand ShowM120CycleCommand { set; get; }

        private async void ShowM120Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute120)
            {
             
                KCycle = KCycle.Minute120;
              await  InitPlotContext();
            }

        }

        public DelegateCommand ShowStockInfoCommand { set; get; }
        private void ShowStockInfo()
        {
            VA = VisibleArea.F10;
        }

        #endregion
    }


    /// <summary>
    /// 显示区域
    /// </summary>
    public enum VisibleArea
    {
        Chart,
        F10,
    }

    /// <summary>
    /// 交易数据类型
    /// </summary>
    public enum TradeDataType
    {
        Stock,//股票交易数据
        THSIndex,//同花顺指数
    }
}
