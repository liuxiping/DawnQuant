using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
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
        AShareDataMaintainService _aShareDataMaintainService;
        public DownloadDataWindow()
        {
            InitializeComponent();
            IsCreateFromLogin = true;
            IsDownloadAllData = false;

            _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _aShareDataMaintainService.DownLoadStockDataProgress += _aShareDataMaintainService_DownLoadStockDataProgress;

            DataContext = new DownloadDataWindowModel();

        }

        DownloadDataWindowModel Model
        {
            get { return (DownloadDataWindowModel)DataContext; }
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
        public bool IsCreateFromLogin { get; set; }

        /// <summary>
        /// 时候是登录窗体创建的
        /// </summary>
        public bool IsDownloadAllData { get; set; }

        private async void _downloadDataWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsCreateFromLogin)
            {
                this.ShowInTaskbar = true;
            }
            else
            {
                this.ShowInTaskbar = true;
            }
            await Task.Run(() =>
            {
                //更新数据自选股数据

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
                Visibility = Visibility.Hidden;
                mainWindow.Show();
            }
            Close();
        }

        private void _aShareDataMaintainService_DownLoadStockDataProgress(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                Model.Progress = msg;
            });

        }

        private void _downloadDataWindow_Closed(object sender, EventArgs e)
        {
            _aShareDataMaintainService.DownLoadStockDataProgress -= _aShareDataMaintainService_DownLoadStockDataProgress;
        }
    }


    public class DownloadDataWindowModel : ViewModelBase
    {
        string _progress = "正在下载交易数据,请稍后...";
        public string Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                SetProperty(ref _progress, value, nameof(Progress));
            }
        }
    }
}
