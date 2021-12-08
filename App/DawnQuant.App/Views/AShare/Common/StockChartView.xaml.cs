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
        }

        public StockChartViewModel Model 
        {
            get { return (StockChartViewModel)DataContext; }
            set { DataContext = value; } 
        }

     
        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
           ToggleButton btnCur=(ToggleButton)sender;
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
        private void NavigateToF10()
        {

            if (Model!=null && !string.IsNullOrEmpty(Model.F10Url))
            {
                _wbStockInfo.Navigate(Model.F10Url);
            }
            //if (wvStockInfo.CoreWebView2 == null)
            //{
            //    var t = wvStockInfo.EnsureCoreWebView2Async();
            //    t.ContinueWith(t =>
            //    {
            //        App.Current.Dispatcher.Invoke(() =>
            //        {
            //            wvStockInfo.CoreWebView2.Navigate(Model.F10Url);
            //        });

            //    });
            //}

            //else
            //{
            //    wvStockInfo.CoreWebView2.Navigate(Model.F10Url);
            //}

        }

        private void _financialChart_Loaded(object sender, RoutedEventArgs e)
        {

            _wbStockInfo.Navigating += WbStockInfo_Navigating;
        }

        private void WbStockInfo_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            SetWebBrowserSilent(sender as WebBrowser, true);
        }


        private void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }



        private void _financialChart_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //选择了查看资料
            if (_btnStockInfo.IsChecked == true)
            {
                NavigateToF10();
            }
        }


        private void fchart_MouseMove(object sender, MouseEventArgs e)
        {
            // Obtain hit information under the test point.
            ChartHitInfo hitInfo = fchart.CalcHitInfo(e.GetPosition(fchart));
            StringBuilder builder = new StringBuilder();

            //hitInfo.

            // Check  whether the chart element is under the test point and if so - obtain the element's content.
            if (hitInfo.InDiagram)
            {
                builder.AppendLine("In diagram");
            }
            if (hitInfo.InAxis)
                builder.AppendLine("In axis:" + hitInfo.Axis.Name);
            if (hitInfo.AxisLabel != null)
                builder.AppendLine("Axis Label:\n" + hitInfo.AxisLabel.Name);
            if (hitInfo.AxisTitle != null)
                builder.AppendLine("Axis title:\n" + hitInfo.AxisTitle.Content);
            if (hitInfo.InTitle)
                builder.AppendLine("In chart title:\n " + hitInfo.Title.Content);
            if (hitInfo.InLegend)
                builder.AppendLine("In legend");
            if (hitInfo.InSeries)
                builder.AppendLine("In series: " + hitInfo.Series.Name);
            if (hitInfo.InSeriesLabel)
            {
                builder.AppendLine("In series label");
                builder.AppendLine("Series Label:" + hitInfo.SeriesLabel.Name);
            }

            if (hitInfo.InSeriesPoint)
            {
                builder.AppendLine("Argument: " + hitInfo.SeriesPoint.Argument);
                builder.AppendLine("Value: " + hitInfo.SeriesPoint.Value);
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
    }
}
