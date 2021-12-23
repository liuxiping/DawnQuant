using AngleSharp.Html.Parser;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
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

            _apiUrl = config.AShareApiUrl;
        }


        string _apiUrl;

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
        public void CollectHolderFromTHS()
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
                    CollectSingleStockHolderFromTHS(tscode, channel);
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
        private void CollectSingleStockHolderFromTHS(string tsCode, GrpcChannel channel)
        {

            string id = tsCode.Substring(0, 6);

            //获取股东信息
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id, @"/holder.html");
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fieldUrl);

            using (WebResponse webResponse = request.GetResponse())
            {

                #region 提取股东人数信息
                List<HolderNumberDto> holderNumberDtos = new List<HolderNumberDto>();

                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(webResponse.GetResponseStream());

                //报告日期
                string dateCssSelector = "#gdrsTable > div > div > div.data_tbody > table.top_thead > tbody > tr > th  >div";
                var dateNodes = dom.QuerySelectorAll(dateCssSelector);
                foreach (var node in dateNodes)
                {
                    HolderNumberDto holderNumberDto = new HolderNumberDto();
                    holderNumberDto.TSCode = tsCode;

                    // 数据样例2021 - 09 - 30
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    holderNumberDto.ReportingPeriod = Timestamp.FromDateTime(
                        DateTime.Parse(node.InnerHtml, dtFormat));
                    holderNumberDtos.Add(holderNumberDto);
                }


                //股东人数
                string numCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(1)>td>div";

                var numNodes = dom.QuerySelectorAll(numCssSelector);

                int i = 0;
                foreach (var node in numNodes)
                {
                    //  数据样例13.65万
                    string num = node.InnerHtml;
                    num = num.Substring(0, num.Length - 1);

                    holderNumberDtos[i].HolderNum = double.Parse(num);
                    i++;
                }

                //变化比例
                string changeCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(2)>td>span";
                var numNchangeNodesodes = dom.QuerySelectorAll(changeCssSelector);

                i = 0;
                foreach (var node in numNodes)
                {
                    // 数据样例 + 19.80 % -4.91 %
                    string ratio = node.InnerHtml.Substring(0, node.InnerHtml.Length - 1);
                    holderNumberDtos[i].Change = double.Parse(ratio);
                    i++;
                }

                #endregion

                #region 提取10大流通股信息

                List<Top10FloatHolderDto> top10FloatHolderDtos = new List<Top10FloatHolderDto>();

                // 提取指标日期
                string top10reportingPeriodCssSelector = "#bd_1 > div.m_tab.mt15 > ul > li>a";
                var top10reportingPeriodNodes = dom.QuerySelectorAll(top10reportingPeriodCssSelector);
                foreach (var node in top10reportingPeriodNodes)
                {
                    Top10FloatHolderDto top10FloatHolderDto = new Top10FloatHolderDto();
                    top10FloatHolderDto.TSCode = tsCode;

                    // 数据样例2021 - 09 - 30
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd";
                    top10FloatHolderDto.EndDate = Timestamp.FromDateTime(
                    DateTime.Parse(node.InnerHtml, dtFormat));

                    top10FloatHolderDtos.Add(top10FloatHolderDto);
                }

                //提取10大流通股东
                i = 0;
                foreach (var top10 in top10FloatHolderDtos)
                {
                    string top10Selector = $"#fher_{i + 1} > table > tbody > tr";

                    var top10DataNodes = dom.QuerySelectorAll(top10Selector);

                    foreach (var node in top10DataNodes)
                    {
                        //股东名称
                        top10.HolderName = node.QuerySelector("th>a").InnerHtml;

                        //持股数量
                        var amount = node.QuerySelector("td:nth-child(2)").InnerHtml;
                        top10.HoldAmount = double.Parse(amount.Substring(0, amount.Length - 1));

                        //持股变化 变动性质:1 不变，2新进，3增加，4减少
                        var shareholdingChangeNode = node.QuerySelector("td:nth-child(3)>span");
                        if (shareholdingChangeNode == null)
                        {
                            //不变
                            top10.HoldChangeCharacter = 1;
                            top10.HoldChange = 0;
                        }
                        else
                        {

                            string s = shareholdingChangeNode.InnerHtml.Trim();
                            if (s == "新进")
                            {
                                top10.HoldChangeCharacter = 2;
                                top10.HoldChange = top10.HoldAmount;
                            }
                            else
                            {
                                s = s.Substring(0, s.Length - 1);
                                top10.HoldChange = double.Parse(s);
                                if (top10.HoldChange > 0)
                                {
                                    //增加
                                    top10.HoldChangeCharacter = 3;
                                }
                                else
                                {
                                    //减少
                                    top10.HoldChangeCharacter = 4;
                                }
                            }
                        }
                    }
                }

                #endregion


                //保存数据
                var client = new IndustryApi.IndustryApiClient(channel);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
            }
        }
    }
}
