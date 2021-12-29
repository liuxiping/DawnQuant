using DawnQuant.App.ViewModels.AShare.Common;
using DevExpress.Xpf.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Models.AShare.EssentialData;


namespace DawnQuant.App.Views.AShare.Common
{
    /// <summary>
    /// FinancialChartUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class StockChartView : UserControl
    {
        public StockChartView()
        {
            InitializeComponent();

            _fchart.AddHandler(ChartControl.MouseLeftButtonUpEvent,
                new MouseButtonEventHandler(_fchart_MouseLeftButtonUp), true);
        }

        public StockChartViewModel Model
        {
            get { return (StockChartViewModel)DataContext; }
            set { DataContext = value; }
        }


        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btnCur = (ToggleButton)sender;
            foreach (var ctrl in _spMenu.Children)
            {
                ToggleButton btn = (ToggleButton)ctrl;
                btn.IsChecked = false;
            }
            btnCur.IsChecked = true;

            //选择了查看资料
            if (_btnStockInfo.IsChecked == true)
            {
                NavigateToF10();
            }
        }

        /// <summary>
        /// 导航到F10
        /// </summary>
        private async void NavigateToF10()
        {

            //if (Model != null && !string.IsNullOrEmpty(Model.F10Url))
            //{
            //    _wbStockInfo.Navigate(Model.F10Url);
            //}
            if (_wvStockInfo.CoreWebView2 == null)
            {
               await _wvStockInfo.EnsureCoreWebView2Async();
            }
            _wvStockInfo.CoreWebView2.Navigate(Model.F10Url);
        

        }

        private void _financialChart_Loaded(object sender, RoutedEventArgs e)
        {

          //  _wbStockInfo.Navigating += WbStockInfo_Navigating;
        }

        #region 废弃
         
        //private void WbStockInfo_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    SetWebBrowserSilent(sender as WebBrowser, true);
        //}


        //private void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        //{
        //    FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
        //    if (fi != null)
        //    {
        //        object browser = fi.GetValue(webBrowser);
        //        if (browser != null)
        //            browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
        //    }
        //}

        #endregion 

        private void _financialChart_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //选择了查看资料
            if (_btnStockInfo.IsChecked == true)
            {
                NavigateToF10();
            }
        }




        #region 复权操作
        private void _miNone_Click(object sender, RoutedEventArgs e)
        {

            UnCheckAllAdjustedState();
            _miNone.IsChecked = true;
            Model.AdjustedState = AdjustedState.None;
            _miShow.Header = "除权";
        }

        private void _miPre_Click(object sender, RoutedEventArgs e)
        {
            UnCheckAllAdjustedState();
            _miPre.IsChecked = true;
            Model.AdjustedState = AdjustedState.Pre;
            _miShow.Header = "前复权";
        }

        private void _miAfter_Click(object sender, RoutedEventArgs e)
        {
            UnCheckAllAdjustedState();
            _miAfter.IsChecked = true;
            Model.AdjustedState = AdjustedState.After;
            _miShow.Header = "后复权";
        }

        private void UnCheckAllAdjustedState()
        {
            foreach (var item in _miShow.Items)
            {
                ((MenuItem)item).IsChecked = false;
            }
        }

        #endregion

        private void _fchart_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {
            if (e.CrosshairElementGroups.Count != 0)
            {
                foreach (CrosshairElement crosshairElement in e.CrosshairElementGroups[0].CrosshairElements)
                {
                    if (crosshairElement != null)
                    {
                        if (crosshairElement.Series.Name == "_bssGain")
                        {
                            crosshairElement.LabelElement.Text = string.Format("涨幅：{0:0.00%}", crosshairElement.SeriesPoint.Value*100);

                            if (crosshairElement.SeriesPoint.Value>=0)
                            {
                                crosshairElement.LabelElement.MarkerFill = Brushes.Red;
                            }
                            else
                            {
                                crosshairElement.LabelElement.MarkerFill = Brushes.Green;
                            }
                          

                        }
                        if (crosshairElement.Series.Name == "_bssAM")
                        {
                            crosshairElement.LabelElement.Text = string.Format("振幅：{0:0.00%}", crosshairElement.SeriesPoint.Value * 100);

                        }
                        if (crosshairElement.Series.Name == "_bssVol")
                        {
                         
                            crosshairElement.LabelElement.Text = $"成交量：{crosshairElement.SeriesPoint.Value /10000:F1}万";

                        }
                        if (crosshairElement.Series.Name == "_turnoverRate")
                        {
                            crosshairElement.LabelElement.Text = $"换手率(实)： {crosshairElement.SeriesPoint.Value * 100:P2}";
                        }
                    }
                }
            }
        }


        StatisticsWindowView _statisticsWindowView = null;

        /// <summary>
        /// 统计功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fchart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
              

                if (_fchart.SelectedItems?.Count > 0)
                {
                    var selectedItems = new List<StockPlotData>();
                  
                    for(int i=0;i< _fchart.SelectedItems.Count; i++)
                    {
                        selectedItems.Add((StockPlotData)_fchart.SelectedItems[i]);
                    }

                    Point rp = Mouse.GetPosition(e.Source as FrameworkElement);
                    Point sp = (e.Source as FrameworkElement).PointToScreen(rp);

                    if(_statisticsWindowView!=null)
                    {
                        _statisticsWindowView.Close();
                    }

                     _statisticsWindowView  = new StatisticsWindowView();

                    //准备数据

                    StatisticsWindowViewModel m = new StatisticsWindowViewModel();

                    m.Title = $"{Model.StockName}  个股K线区间统计";

                    m.Start = selectedItems.First().FormatedTradeDateTime;
                    m.End = selectedItems.Last().FormatedTradeDateTime;
                    m.CycleCount = selectedItems.Count;

                    m.FirstPrice = selectedItems.First().Close;
                    m.EndPrice = selectedItems.Last().Close;

                    m.MaxPrice = selectedItems.Max(p=>p.Close);
                    m.MinPrice = selectedItems.Min(p => p.Close);

                    m.Turnover = selectedItems.Sum(p=>p.Turnover)/100;
                    m.TurnoverFree= selectedItems.Sum(p=>p.TurnoverFree)/100;

                    _statisticsWindowView.Model = m;

                    _statisticsWindowView.Left = sp.X;
                    _statisticsWindowView.Top = sp.Y;

                    _statisticsWindowView.Show();
                }
            }
        }
    }
}
