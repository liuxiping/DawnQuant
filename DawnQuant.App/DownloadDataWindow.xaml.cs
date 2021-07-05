using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Xpf.Core;
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

namespace DawnQuant.App
{
    /// <summary>
    /// LoadFinancialDataView.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadDataWindow : Window
    {
        public DownloadDataWindow()
        {
            InitializeComponent();
            IsCreateFromLogin = true;
        }
        private void _downloadDataWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();

            }
        }
        /// <summary>
        /// 时候是登录窗体创建的
        /// </summary>
        public bool  IsCreateFromLogin { get; set; }

        private void _downloadDataWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //更新数据自选股数据
            AShareDataMaintainService _aShareDataMaintainService=IOCUtil.Container.Resolve< AShareDataMaintainService>();

            var t = Task.Run(() =>
            {
                _aShareDataMaintainService.DownLoadStockData();
            });

            t.ContinueWith((t) =>
            {
                Dispatcher.Invoke(() =>
                {

                    if (IsCreateFromLogin)
                    {
                        this.ShowInTaskbar = false;
                        Visibility = Visibility.Hidden;
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                       
                    }
                    Close();
                });
            });
        }
    }
}
