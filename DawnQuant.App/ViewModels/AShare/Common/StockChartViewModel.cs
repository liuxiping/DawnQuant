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

namespace DawnQuant.App.ViewModels.AShare.Common
{
    public class StockChartViewModel: ViewModelBase
    {
        /// <summary>
        /// 显示区域
        /// </summary>
        public enum VisibleArea
        {
            Chart,
            F10,
        }

        private readonly StockPlotDataService _stockPlotDataService;

        public string TSCode { get; set; }
        public string StockName { get; set; }
        public KCycle KCycle { get; set; }

        //页面数据大小
        int _tradeDataPageSize = 250;

        public StockChartViewModel()
        {
            _stockPlotDataService = IOCUtil.Container.Resolve<StockPlotDataService>();

            _adjustedState = AdjustedState.Pre;

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
        public void InitPlotContext()
        {
            if (KCycle == KCycle.Day || KCycle == KCycle.Week ||
             KCycle == KCycle.Month || KCycle == KCycle.Minute30 ||
             KCycle == KCycle.Minute60 || KCycle == KCycle.Minute120)
            {
                var pc = _stockPlotDataService.GetStockPlotContext(TSCode, KCycle, _tradeDataPageSize, AdjustedState);

                pc.Name = StockName;

                PlotContext = pc;
            }
            else
            {
                throw new NotSupportedException("不支持的K线周期");
            }

        }


        #region   属性
        //绘图相关数据
        StockPlotContext _plotContext;
        public StockPlotContext PlotContext
        {
            get { return _plotContext; }
            set { SetProperty(ref _plotContext, value, nameof(PlotContext)); }
        }

        VisibleArea  _va =  VisibleArea.Chart;
        public VisibleArea VA//显示区域
        {
            get { return _va; }
            set { SetProperty(ref _va, value, nameof(VA)); }
        }
       
        public string F10Url
        {
            get { return "http://basic.10jqka.com.cn/" +TSCode.Substring(0,6) + "/"; }
          
        }

        //当前节点交易信息
        CurSelectStockInfo _curSelStockInfo;
        public CurSelectStockInfo CurSelStockInfo
        {
            set { SetProperty(ref _curSelStockInfo, value, nameof(CurSelStockInfo)); }
            get { return _curSelStockInfo; }
        }

        //复权信息
        AdjustedState   _adjustedState;
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

        private void OnAdjustedStateChange()
        {
            //重新绑定数据
            InitPlotContext();
        }
        #endregion


        #region Command
        public DelegateCommand ShowDayCycleCommand { set; get; }
        private void ShowDayCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Day)
            {
                KCycle = KCycle.Day;
                InitPlotContext();
            }

        }

        public DelegateCommand ShowWeekCycleCommand { set; get; }
        private void ShowWeekCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Week)
            {
                KCycle = KCycle.Week;

                InitPlotContext();
            }

        }

        public DelegateCommand ShowMonthCycleCommand { set; get; }
        private void ShowMonthCycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Month)
            {
                KCycle = KCycle.Month;
                InitPlotContext();
            }

        }

        public DelegateCommand ShowM30CycleCommand { set; get; }
        private void ShowM30Cycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Minute30)
            {
                KCycle = KCycle.Minute30;
                InitPlotContext();
            }

        }

        public DelegateCommand ShowM60CycleCommand { set; get; }

        private void ShowM60Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute60)
            {
             
                KCycle = KCycle.Minute60;
                InitPlotContext();
            }

        }
        public DelegateCommand ShowM120CycleCommand { set; get; }

        private void ShowM120Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute120)
            {
             
                KCycle = KCycle.Minute120;
                InitPlotContext();
            }

        }

        public DelegateCommand ShowStockInfoCommand { set; get; }
        private void ShowStockInfo()
        {
            VA = VisibleArea.F10;
        }

        #endregion
    }
}
