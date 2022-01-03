
using Autofac;
using DawnQuant.App.Models;
using DawnQuant.App.Utils;
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

namespace DawnQuant.App.Views.Layout
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
                txtCurTime.Text = DateTime.Now.ToString("HH:mm:ss");
            });
            
        }

        Timer _timer;

        private void FooterView_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppLocalConfig.Instance.LastUpdateAllDataDateTime != null)
            {
                imgLastUpdateTime.Visibility = Visibility.Visible;
                var dt = AppLocalConfig.Instance.LastUpdateAllDataDateTime.Value.ToString("yyyy-MM-dd HH:mm");
                txtLastUpdateTime.Text = $"上次全部数据更新时间：{dt}";
            }

            var notify = IOCUtil.Container.Resolve<MessageUtil>();

            notify.DownloadAShareDataProgress += (complete, total) =>
           {
              
               string msg = $"正在下载交易数据，已经完成{complete}个，总共{total}个";

               Dispatcher.Invoke(() =>
               {
                   
                   imgNotify.Visibility = Visibility.Visible;
                   txtNotify.Text = msg;
               });

           };

            notify.DownloadAShareDataComplete += (isALL) => 
            {
                if (isALL)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var dt = AppLocalConfig.Instance.LastUpdateAllDataDateTime.Value.ToString("yyyy-MM-dd HH:mm");

                        var msg = $"全部数据更新完成,更新时间{dt}";
                        txtNotify.Text = msg;

                        txtLastUpdateTime.Text = $"上次全部数据更新时间：{dt}";
                    });

                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        var dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        var msg = $"策略数据更新完成,更新时间{dt}";
                        txtNotify.Text = msg;
                    });
                }


            };

            _timer = new Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void FooterView_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
    }
}
