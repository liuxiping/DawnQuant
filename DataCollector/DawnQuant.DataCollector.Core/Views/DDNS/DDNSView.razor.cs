using DynamicDns.Core;
using DynamicDns.TencentCloud;
using DynamicDns.TencentCloud.Http;

namespace DawnQuant.DataCollector.Core.Views.DDNS
{
    public partial class DDNSView
    {

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _tm = new Timer(async (t) =>
           {
               await UpdateIpAsync();
               await InvokeAsync(() =>
                 {
                     StateHasChanged();
                 });

           }, null, 0, 60 * 1000 * 5);
        }

        public string Message { get; set; } = "";

        private void ClearMessage()
        {
            Message = "";
        }

        string _ip = null;
        Timer _tm = null;

        /// <summary>
        /// 动态更新腾讯DDNS
        /// </summary>
        /// <returns></returns>
        private async Task UpdateIpAsync()
        {

            //检测IP网站
            //http://pv.sohu.com/cityjson
            //http://checkip.amazonaws.com

            try
            {
                HttpClient httpClient = new HttpClient();
                string ip = await httpClient.GetStringAsync("http://checkip.amazonaws.com");
                ip = ip.Trim();

                if (ip != _ip)
                {
                    if(!string.IsNullOrEmpty(Message))
                    {
                        Message += "\r\n";
                    }

                    Message += $"更新DNS,当前IP:{ip}，更新时间：{DateTime.Now.ToString()}\r\n";

                    _ip = ip;

                    IDynamicDns ddns = new TencentCloudDynamicDns(new TencentCloudOptions()
                    {
                        DefaultRequestMethod = RequestMethod.GET,

                        SecretId = "",
                        SecretKey = "",
                    });

                    Message += "更新 www.dawnquant.com，";
                    var res = await ddns.AddOrUpdateAsync("dawnquant.com", "www", "A", ip);
                    Message += $"Success?: {!res.Error}\r\n";

                    Message += "更新 dawnquant.com，";
                    res = await ddns.AddOrUpdateAsync("dawnquant.com", "@", "A", ip);
                    Message += $"Success?: {!res.Error}\r\n";

                    await InvokeAsync(() =>
                     {
                         StateHasChanged();
                     });
                }
            }
            catch (Exception e)
            {
                Message += $"ExceptionMessage: {e.Message ?? ""}\r\n";

                await InvokeAsync(() =>
                 {
                     StateHasChanged();
                 });
            }

        }
    
    }

}
