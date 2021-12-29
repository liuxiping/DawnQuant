using AngleSharp.Html.Parser;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;


namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class IndustryCollector
    {

        public IndustryCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;

            _svcUrl = config.AShareApiUrl;

            UnCompleteStocks = new List<string>();
        }

        ILogger _logger;
        IPassportProvider _passportProvider;

        string _svcUrl;
        public string Msg { get; set; }
        public List<string> UnCompleteStocks { set; get; }

        public event Action ProgressChanged;

        protected void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged();
            }
        }


      
        /// <summary>
        /// 从同花顺提取行业数据
        /// </summary>
        public void CollectIndustryFromTHS()
        {

            GrpcChannel channel = null;
            List<string> alltscodes = new List<string>();
            List<string> completetscodes = new List<string>();
            try
            {
                channel = GrpcChannel.ForAddress(_svcUrl, new GrpcChannelOptions
                {
                    MaxReceiveMessageSize = null,
                    MaxSendMessageSize = null,
                });

                var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var response = client.GetAllTSCodes(new Empty(), meta);
                alltscodes.AddRange(response.TSCodes);
                int all = response.TSCodes.Count;
                int complete = 0;

                foreach (var tscode in response.TSCodes)
                {
                    CollectSingleStockIndustryFromTHS(tscode, channel);
                    completetscodes.Add(tscode);

                    complete++;
                    Msg = $"行业信息已经成功采集{complete}个股票，总共{all}个股票";
                    OnProgressChanged();
                }
            }
            finally
            {
                //出现异常
                UnCompleteStocks = alltscodes.Except(completetscodes).ToList();

                if (channel != null)
                {
                    channel.Dispose();

                }
            }
        }

        public void CollectIndustryFromTHS(List<string> stocks)
        {
            GrpcChannel channel = null;
            List<string> completetscodes = new List<string>();
            try
            {
                channel = GrpcChannel.ForAddress(_svcUrl, new GrpcChannelOptions
                {
                    MaxReceiveMessageSize = null,
                    MaxSendMessageSize = null,
                });

                int all = stocks.Count;
                int complete = 0;

                foreach (var tscode in stocks)
                {
                    CollectSingleStockIndustryFromTHS(tscode, channel);
                    completetscodes.Add(tscode);
                    complete++;
                    Msg = $"行业信息已经成功采集{complete}个股票，总共{all}个股票";
                    OnProgressChanged();
                }
            }
            finally
            {
                UnCompleteStocks = stocks.Except(completetscodes).ToList();
                if (channel != null)
                {
                    channel.Dispose();

                }
            }
        }

        /// <summary>
        /// 获取单个行业信息
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="channel"></param>
        private void CollectSingleStockIndustryFromTHS(string tsCode, GrpcChannel channel)
        {

            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id, @"/field.html");


            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);
                t.Wait();
                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                var industryNode = dom.QuerySelector(".page9table .threecate span");

                if (industryNode != null)
                {
                    string industryInfo = industryNode.InnerHtml.Trim();
                    var industrys = industryInfo.Split("--");
                    var first = industrys[0].Trim();
                    var second = industrys[1].Trim();
                    var three = industrys[2].Trim();
                    three = three.Substring(0, three.IndexOf("（"));

                    var clientIndustry = new IndustryApi.IndustryApiClient(channel);

                    Metadata meta = new Metadata();
                    meta.AddAuthorization(_passportProvider.AccessToken);

                    var industry = clientIndustry.ParseIndustry(new ParseIndustryRequest
                    {
                        First = first,
                        Second = second,
                        Three = three
                    }, meta);

                    var clientBasicStockInfo = new BasicStockInfoApi.BasicStockInfoApiClient(channel);

                    clientBasicStockInfo.UpdateIndustry(new UpdateIndustryRequest
                    {
                        IndustryId = industry.Entity.Id,
                        TSCode = tsCode
                    }, meta);

                }
                else
                {
                    _logger.LogError($"提取不到股票代码为{id}的行业信息");
                }



            }
        }

    }
}

