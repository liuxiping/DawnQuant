using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Text;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    /// <summary>
    /// 股东人数和十大流通股
    /// </summary>
    public class HolderCollector
    {

        public HolderCollector(ILogger logger, CollectorConfig config,
          IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;

            _tushareToken = config.TushareToken;
            _tushareUrl = config.TushareUrl;
            _apiUrl = config.AShareApiUrl;
        }

        string _tushareToken;
        string _tushareUrl;
        string _apiUrl;
        int _threadCount = 5;

        ILogger _logger;
        IPassportProvider _passportProvider;


        public event Action<string> CollectHolderNumberProgressChanged;


        public List<string> UnCompleteStocks { get; set; }

        protected void OnCollectHolderNumberProgressChanged(string msg)
        {
            if (CollectHolderNumberProgressChanged != null)
            {
                CollectHolderNumberProgressChanged(msg);
            }
        }

        /// <summary>
        /// 采集最近10期股东人数和最近5期10大流通股
        /// </summary>
        public void CollectHolder()
        {

            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);
            List<string> alltscodes = new List<string>();
            List<string> completetscodes = new List<string>();
            try
            {
                object lockObj = new object();

                var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var response = client.GetAllTSCodes(new Empty(), meta);

                alltscodes.AddRange(response.TSCodes);

                int allCount = response.TSCodes.Count;

                int complete = 0;
                foreach (var tscode in alltscodes)
                {
                   // CollectSingleStockHolderNumber(tscode, channel);
                    lock (lockObj)
                    {
                        complete++;
                        completetscodes.Add(tscode);
                    }
                    string msg = $"股东人数已经成功采集{complete}个股票，总共{allCount}个股票";
                    OnCollectHolderNumberProgressChanged(msg);

                }
            }
            finally
            {
                UnCompleteStocks = alltscodes.Except(completetscodes).ToList();
                if (channel != null)
                {
                    channel.Dispose();
                }
            }
        }


        /// <summary>
        /// 采集单个股票最近10期股东人数和最近5期10大流通股
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="channel"></param>
        private void CollectSingleStockHolder(string tsCode, GrpcChannel channel)
        {

            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id, @"/holder.html");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fieldUrl);
            //  SetTHSWebRequestHeader((HttpWebRequest)request);
            using (WebResponse webResponse = request.GetResponse())
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                htmlDocument.Load(webResponse.GetResponseStream(), Encoding.GetEncoding("GBK"));

                //提取股东人数信息
                #region
                List<HolderNumberDto> holderNumberDtos = new List<HolderNumberDto>();

                HtmlNode holderNumberDataNode = htmlDocument.DocumentNode.
                 SelectSingleNode("//*[@id=\"gdrsTable\"]/descendant::div[@class=\"data_tbody\"]");
                //提取指标日期数据
                if (holderNumberDataNode != null)
                {
                    var reportingPeriodNodes = holderNumberDataNode.SelectNodes
                        ("//*[@class=\"top_thead\"]/descendant::div[@class=\"td_w]");

                    //提取指标日期
                    foreach (var node in reportingPeriodNodes)
                    {
                        HolderNumberDto holderNumberDto = new HolderNumberDto();
                        holderNumberDto.TSCode = tsCode;

                        //数据样例2021-09-30
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-MM-dd";
                        holderNumberDto.ReportingPeriod = Timestamp.FromDateTime(
                            DateTime.Parse(node.InnerText, dtFormat));

                        holderNumberDtos.Add(holderNumberDto);
                    }
                    //提取数据 只提取股东人数和变化比例
                    var dataNodes = holderNumberDataNode.SelectNodes
                        ("//*[@class=\"tbody\"]/descendant::tr");
                    //提取股东人数数据
                    var numNodes = dataNodes[0].SelectNodes("//*[@class=\"td_w\"]");
                    int i = 0;
                    foreach (var node in numNodes)
                    {
                        //数据样例13.65万
                        string num = node.InnerText;
                        num = num.Substring(0, num.Length - 1);

                        holderNumberDtos[i].HolderNum = double.Parse(num);
                        i++;
                    }
                    var changeNodes = dataNodes[1].SelectNodes("//span");
                    i = 0;
                    foreach (var node in numNodes)
                    {
                        //数据样例 +19.80% -4.91%
                        string ratio = node.InnerText.Substring(0, node.InnerText.Length - 1);
                        holderNumberDtos[i].Change = double.Parse(ratio);
                        i++;
                    }
                }
                #endregion

                //提取10大流通股信息

                List<Top10FloatHolderDto> top10FloatHolderDtos = new List<Top10FloatHolderDto>();

                HtmlNode top10DataNode = htmlDocument.DocumentNode.
                 SelectSingleNode("//*[@id=\"bd_1\"]");
                //提取指标日期数据
                if (holderNumberDataNode != null)
                {
                    var reportingPeriodNodes = holderNumberDataNode.SelectNodes
                        ("//*[@class=\"m_tab mt15\"]/descendant::a");

                    //提取指标日期
                    foreach (var node in reportingPeriodNodes)
                    {
                        HolderNumberDto holderNumberDto = new HolderNumberDto();
                        holderNumberDto.TSCode = tsCode;

                        //数据样例2021-09-30
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyy-MM-dd";
                        holderNumberDto.ReportingPeriod = Timestamp.FromDateTime(
                            DateTime.Parse(node.InnerText, dtFormat));

                        holderNumberDtos.Add(holderNumberDto);
                    }

                    //提取十大流通股
                    var dataNodes = holderNumberDataNode.SelectNodes
                        ("//*[@class=\"m_tab_content2 clearfix\"]");

                    //报告期
                    foreach (var node in dataNodes)
                    {
                        //十大流通股
                        var top10dataNodes = node.SelectNodes("/table/tbody[0]/descendant::tr");

                        foreach (var top10Node in top10dataNodes)
                        {

                        }
                    }
                    //提取股东人数数据
                    var numNodes = dataNodes[0].SelectNodes("/");
                    int i = 0;
                    foreach (var node in numNodes)
                    {
                        //数据样例13.65万
                        string num = node.InnerText;
                        num = num.Substring(0, num.Length - 1);

                        holderNumberDtos[i].HolderNum = double.Parse(num);
                        i++;
                    }
                    var changeNodes = dataNodes[1].SelectNodes("//span");
                    i = 0;
                    foreach (var node in numNodes)
                    {
                        //数据样例 +19.80% -4.91%
                        string ratio = node.InnerText.Substring(0, node.InnerText.Length - 1);
                        holderNumberDtos[i].Change = double.Parse(ratio);
                        i++;
                    }
                }


                {
                    _logger.LogError($"提取不到股票代码为{id}的行业信息");
                }


                //TuShare tu = new TuShare(_tushareUrl, _tushareToken);

                //StkHoldernumberRequestModel requestModel = new StkHoldernumberRequestModel();
                //requestModel.TsCode = tsCode;
                //requestModel.StartDate = DateTime.Now.AddYears(-2).ToString("yyyyMMdd");
                //requestModel.Enddate = DateTime.Now.ToString("yyyyMMdd");
                //var task = tu.GetData(requestModel);
                //task.Wait();

                //if (task.Result != null && task.Result.Count > 0)
                //{

                //    SaveHolderNumberRequest request = new SaveHolderNumberRequest();
                //    foreach (var tuhn in task.Result)
                //    {
                //        HolderNumberDto holderNumberDto = new HolderNumberDto();
                //        holderNumberDto.TSCode = tuhn.TsCode;
                //        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                //        dtFormat.ShortDatePattern = "yyyyMMdd";
                //        holderNumberDto.ReportingPeriod = Timestamp.FromDateTime(DateTime.Parse(tuhn.AnnDate, dtFormat));
                //        holderNumberDto.EndDate = Timestamp.FromDateTime(DateTime.Parse(tuhn.EndDate, dtFormat));
                //        holderNumberDto.HolderNum = tuhn.HolderNum;

                //        request.Entities.Add(holderNumberDto);
                //    }
                //    if (request.Entities.Count > 0)
                //    {
                //        try
                //        {
                //            var client = new HolderNumberApi.HolderNumberApiClient(channel);
                //            Metadata meta = new Metadata();
                //            meta.AddAuthorization(_passportProvider.AccessToken);
                //            client.SaveHolderNumber(request, meta);
                //        }
                //        finally
                //        {
                //            if (channel != null)
                //            {
                //                channel.Dispose();

                //            }
                //        }
                //    }
                //}

            }
        }


       
    }
}
