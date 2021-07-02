

using DawnQuant.DataCollector.ViewModels.AShare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DawnQuant.DataCollector.Views.AShare
{
    /// <summary>
    /// Company.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryDataView : UserControl
    {
        public HistoryDataView()
        {
            InitializeComponent();

            DataContext = new HistoryDataViewModel();
        }

        private void txtCollectIndustryProgress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCollectIndustryProgress.Text))
            {
                txtCollectIndustryProgress.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtCollectIndustryProgress.Visibility = Visibility.Visible;
            }
        }

        private void txtCollectDailyTradeDataProgress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCollectDailyTradeDataProgress.Text))
            {
                txtCollectDailyTradeDataProgress.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtCollectDailyTradeDataProgress.Visibility = Visibility.Visible;
            }
        }

        private void txtCollectStockDailyIndicatorProgress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCollectStockDailyIndicatorProgress.Text))
            {
                txtCollectStockDailyIndicatorProgress.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtCollectStockDailyIndicatorProgress.Visibility = Visibility.Visible;
            }
        }
    }
}
