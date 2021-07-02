using DawnQuant.DataCollector.ViewModels.AShare;
using System.Windows;
using System.Windows.Controls;

namespace DawnQuant.DataCollector.Views.AShare
{
    /// <summary>
    /// IncrementDataTask.xaml 的交互逻辑
    /// </summary>
    public partial class IncrementDataView : UserControl
    {
        public IncrementDataView()
        {
            InitializeComponent();
            DataContext = new IncrementDataViewModel();
           
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


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            //自动开启
            var model = (IncrementDataViewModel)DataContext;
            model.StartIDITaskCommand.Execute(null);
            model.StartIDTDTaskCommand.Execute(null);

        }
    }
}
