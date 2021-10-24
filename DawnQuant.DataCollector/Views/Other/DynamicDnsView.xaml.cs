using DynamicDns.Core;
using DynamicDns.TencentCloud;
using DynamicDns.TencentCloud.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DawnQuant.DataCollector.Views.Other
{
    /// <summary>
    /// DynamicDnsView.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicDnsView : UserControl
    {
        public DynamicDnsView()
        {
            InitializeComponent();
        }

        string _ip = null;
        DispatcherTimer _tm = null;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _tm = new DispatcherTimer();

            //http://pv.sohu.com/cityjson
            //http://checkip.amazonaws.com


            _tm.Interval = new TimeSpan(0, 5, 0);
            _tm.Tick += _tm_Tick; ;
            

            _tm.Start();

            //程序启动更新Dns
            _tm_Tick(null, null);

        }

        private void _tm_Tick(object sender, EventArgs e)
        {
            var t = UpdateIpAsync();
        }

       

        private async Task UpdateIpAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string ip = await httpClient.GetStringAsync("http://checkip.amazonaws.com");
                ip = ip.Trim();

                if (ip != _ip)
                {
                    _txtMsg.Text += $"更新DNS,当前IP:{ip}\r\n更新时间：{DateTime.Now.ToString()}\r\n\r\n";

                    _ip = ip;

                    IDynamicDns ddns = new TencentCloudDynamicDns(new TencentCloudOptions()
                    {
                        DefaultRequestMethod = RequestMethod.GET,

                        SecretId = "AKIDEH5csZliNlrhLlMTbXFhU6RWehX6lASS",
                        SecretKey = "2BAsNo1ygqkSvFXbP1aHQoRm63s37yco",
                    });

                    _txtMsg.Text += "更新 www.dawnquant.com\r\n";
                    var res = await ddns.AddOrUpdateAsync("dawnquant.com", "www", "A", ip);
                    _txtMsg.Text += $"Success?: {!res.Error}\r\n";
                    _txtMsg.Text += "\r\n";

                    _txtMsg.Text += "更新 dawnquant.com\r\n";
                    res = await ddns.AddOrUpdateAsync("dawnquant.com", "@", "A", ip);
                    _txtMsg.Text += $"Success?: {!res.Error}\r\n";
                    _txtMsg.Text += "\r\n";
                }
            }
            catch (Exception e)
            {
                _txtMsg.Text += $"ExceptionMessage: {e.Message ?? ""}\r\n\r\n";
            }

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _tm.Tick -= _tm_Tick;
            _tm.Stop();
            _tm = null;
        }
    }
}
