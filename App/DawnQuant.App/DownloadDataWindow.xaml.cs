using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            IsDownloadAllData = false;
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

        /// <summary>
        /// 时候是登录窗体创建的
        /// </summary>
        public bool IsDownloadAllData { get; set; }

        private async void _downloadDataWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                //更新数据自选股数据
                AShareDataMaintainService _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();

                //删除数据
                if (IsDownloadAllData)
                {
                    string dataPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data");
                    Directory.Delete(dataPath, true);
                }

                _aShareDataMaintainService.DownLoadStockData();
              

            }).ConfigureAwait(true);

            if (IsCreateFromLogin)
            {
                var mainWindow = new MainWindow();

                this.ShowInTaskbar = false;
                Height = 0;
                Width = 0;
                Visibility = Visibility.Hidden;
                mainWindow.Show();
            }
            Close();


        }
    }
}
