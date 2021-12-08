using DawnQuant.App.Core.Models.AShare.EssentialData;
using DawnQuant.App.Core.Services.AShare;


namespace DawnQuant.App.Core.ViewModels.AShare.Common
{
    public class StockChartViewModel
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

        public StockChartViewModel(StockPlotDataService stockPlotDataService)
        {
            _stockPlotDataService = stockPlotDataService;
            _adjustedState = AdjustedState.Pre;
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
                    StockPlotContext pc = null;
                    await Task.Run(() =>
                     {
                         pc = _stockPlotDataService.GetStockPlotContext(TSCode, KCycle, _tradeDataPageSize, AdjustedState);

                     }).ConfigureAwait(true);
                    pc.Name = StockName;
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
        public StockPlotContext PlotContext { get; set; }


        public VisibleArea VA { get; set; }//显示区域


        public string F10Url
        {
            get { return "http://basic.10jqka.com.cn/" +TSCode.Substring(0,6) + "/"; }
          
        }

        //当前节点交易信息
        public CurSelectStockInfo CurSelStockInfo { get; set; }


        //复权信息
        AdjustedState   _adjustedState;
        public AdjustedState AdjustedState
        {
            set
            {
                _adjustedState= value;
                    OnAdjustedStateChange();
                
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
        private async void ShowDayCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Day)
            {
                KCycle = KCycle.Day;
              await  InitPlotContext();
            }

        }

        private async void ShowWeekCycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Week)
            {
                KCycle = KCycle.Week;

              await  InitPlotContext();
            }

        }

        private async void ShowMonthCycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Month)
            {
                KCycle = KCycle.Month;
              await  InitPlotContext();
            }

        }

        private async void ShowM30Cycle()
        {
            VA = VisibleArea.Chart;

            if (KCycle != KCycle.Minute30)
            {
                KCycle = KCycle.Minute30;
               await InitPlotContext();
            }

        }
        private async void ShowM60Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute60)
            {
             
                KCycle = KCycle.Minute60;
               await InitPlotContext();
            }

        }
        private async void ShowM120Cycle()
        {
            VA = VisibleArea.Chart;
            if (KCycle != KCycle.Minute120)
            {
             
                KCycle = KCycle.Minute120;
              await  InitPlotContext();
            }

        }

        private void ShowStockInfo()
        {
            VA = VisibleArea.F10;
        }

        #endregion
    }
}
