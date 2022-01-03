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
        public DownloadDataWindow()
        {
            InitializeComponent();
            DataContext = new DownloadDataWindowModel();
            Model.DownlaodDataComplete += Model_DownlaodDataComplete;

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

        private void _downloadDataWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 数据下载完成 
        /// </summary>
        private void Model_DownlaodDataComplete()
        {
            Close();
        }

    }


    public class DownloadDataWindowModel : ViewModelBase
    {
        AShareDataMaintainService _aShareDataMaintainService;
        SettingService _settingService;

        public DownloadDataWindowModel()
        {
            _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _aShareDataMaintainService.DownLoadStockDataProgress += _aShareDataMaintainService_DownLoadStockDataProgress;
            _settingService = IOCUtil.Container.Resolve<SettingService>();

            //下载数据
            DownlaodData();
        }

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

        public event Action DownlaodDataComplete;
        protected void OnDownlaodDataComplete()
        {
            DownlaodDataComplete?.Invoke();
        }


        private void _aShareDataMaintainService_DownLoadStockDataProgress(int complete, int total)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Progress = $"正在下载交易数据，已经完成{complete}个，总共{total}个";

            });

        }
     
        /// <summary>
        /// 下载交易数据
        /// </summary>
        /// <returns></returns>
        private Task DownlaodData()
        {
            return Task.Run(() =>
            {
                string dataPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data");
                Directory.Delete(dataPath, true);

                App.IsUpdateAllAShareData = true;

                _aShareDataMaintainService.DownLoadStockData();

                App.AShareSetting.LastUpdateAllDataDateTime = DateTime.Now;
                _settingService.SaveSetting(App.AShareSetting.Serialize());

                App.IsUpdateAllAShareData = false;

                OnDownlaodDataComplete();
            });

        }
    }
}
