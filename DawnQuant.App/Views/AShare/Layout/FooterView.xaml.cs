
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DawnQuant.App.Views.AShare.Layout
{
    /// <summary>
    /// FooterView.xaml 的交互逻辑
    /// </summary>
    public partial class FooterView : UserControl
    {
        public FooterView()
        {
            InitializeComponent();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.Current?.Dispatcher?.Invoke(() =>
            {
                txtCurTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            });
            
        }

        Timer _timer;

        private void MvxWpfView_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void MvxWpfView_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}
