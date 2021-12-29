using AngleSharp.Html.Dom;
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
                    string msg = $"股东信息已经成功采集{complete}个股票，总共{allCount}个股票";
                    OnCollectHolderNumberProgressChanged(msg);

                }
                OnCollectHolderNumberProgressChanged($"股东信息已经成功采集{complete}个股票，总共{allCount}个股票，股东信息已成功采集");
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

            using (HttpClient client = new HttpClient())
            {
               var t= client.GetStreamAsync(fieldUrl);
                t.Wait();
                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                string header = "#gdrsTable > div > div > div.left_thead > table.tbody > tbody > tr:nth-child(2) > th";
                var headerNode = dom.QuerySelector(header);
                if (headerNode != null)
                {
                    //不仅在A股上市还在其他市场上市
                   if( headerNode.InnerHtml.Trim() == "A股股东数（户）")
                    {
                        CollectSingleStockHolderFromTHSNotOnlyAShare(tsCode, channel, dom);
                    }
                    else
                    {
                        CollectSingleStockHolderFromTHSOnlyAShare(tsCode, channel, dom);
                    }
                }
                else
                {
                    _logger.LogError($"提取不到股票代码为{id}的股东信息");
                }
            }
        }




        /// <summary>
        /// 只在A股上市的公司
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="channel"></param>
        private void CollectSingleStockHolderFromTHSOnlyAShare(string tsCode, GrpcChannel channel,IHtmlDocument dom)
        {
            #region 提取股东人数信息
            List<HolderNumberDto> holderNumberDtos = new List<HolderNumberDto>();
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

                var date = DateTime.Parse(node.InnerHtml, dtFormat);

                holderNumberDto.EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(date, DateTimeKind.Utc));

                holderNumberDtos.Add(holderNumberDto);
            }


            //股东人数
            string numCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(1)>td>div";

            var numNodes = dom.QuerySelectorAll(numCssSelector);

            int i = 0;
            foreach (var node in numNodes)
            {
                //  数据样例13.65万
                string num = node.InnerHtml.Trim();
                holderNumberDtos[i].HolderNum = ParseNum(num);
                i++;
            }



            //变化比例
            string changeCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(2)>td";
            var numNchangeNodesodes = dom.QuerySelectorAll(changeCssSelector);

            i = 0;
            foreach (var node in numNchangeNodesodes)
            {
                // 数据样例 + 19.80 % -4.91 %
                string ratio = node.TextContent.Trim();
                holderNumberDtos[i].Change = ParseRatio(ratio);
                i++;
            }



            #endregion

            #region 提取10大流通股信息
            List<Top10FloatHolderDto> top10FloatHolderDtos = CollectTop10FloatHolder(dom, tsCode);
            #endregion


            //保存数据

            var client = new HolderApi.HolderApiClient(channel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            //保存股东人数
            var hnRequest = new SaveHolderNumberRequest();
            hnRequest.Entities.AddRange(holderNumberDtos);
            client.SaveHolderNumber(hnRequest, meta);

            //保存10大流通股
            var top10Request = new SaveTop10FloatHolderRequest();
            top10Request.Entities.AddRange(top10FloatHolderDtos);
            client.SaveTop10FloatHolder(top10Request, meta);

        }


        /// <summary>
        /// 不仅在A股上市的公司，也在其他市场上市
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="channel"></param>
        private void CollectSingleStockHolderFromTHSNotOnlyAShare(string tsCode, GrpcChannel channel, IHtmlDocument dom)
        {
            #region 提取股东人数信息
            List<HolderNumberDto> holderNumberDtos = new List<HolderNumberDto>();
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

                var date = DateTime.Parse(node.InnerHtml, dtFormat);

                holderNumberDto.EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(date, DateTimeKind.Utc));

                holderNumberDtos.Add(holderNumberDto);
            }


            //股东人数
            string numCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(2)>td";

            var numNodes = dom.QuerySelectorAll(numCssSelector);

            int i = 0;
            foreach (var node in numNodes)
            {
                //  数据样例13.65万
                string num = node.InnerHtml.Trim();
                holderNumberDtos[i].HolderNum = ParseNum(num);
                i++;
            }



            //变化比例
            string changeCssSelector = "#gdrsTable > div > div > div.data_tbody > table.tbody > tbody > tr:nth-child(3)>td";
            var numNchangeNodesodes = dom.QuerySelectorAll(changeCssSelector);

            i = 0;
            foreach (var node in numNchangeNodesodes)
            {
                // 数据样例 + 19.80 % -4.91 %
                string ratio = node.TextContent.Trim();
                holderNumberDtos[i].Change = ParseRatio(ratio);
                i++;
            }



            #endregion

            #region 提取10大流通股信息
            List<Top10FloatHolderDto> top10FloatHolderDtos = CollectTop10FloatHolder(dom,tsCode);
            #endregion

            //保存数据
            var client = new HolderApi.HolderApiClient(channel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            //保存股东人数
            var hnRequest = new SaveHolderNumberRequest();
            hnRequest.Entities.AddRange(holderNumberDtos);
            client.SaveHolderNumber(hnRequest, meta);

            //保存10大流通股
            var top10Request = new SaveTop10FloatHolderRequest();
            top10Request.Entities.AddRange(top10FloatHolderDtos);
            client.SaveTop10FloatHolder(top10Request, meta);

        }


        /// <summary>
        /// 提取10大流通股
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="tsCode"></param>
        /// <returns></returns>
        private List<Top10FloatHolderDto> CollectTop10FloatHolder(IHtmlDocument dom,string tsCode)
        {
          
            List<Top10FloatHolderDto> top10FloatHolderDtos = new List<Top10FloatHolderDto>();

            // 提取指标日期
            string top10EndDateCssSelector = "#bd_1 > div.m_tab.mt15 > ul > li>a";
            var top10EndDateNodes = dom.QuerySelectorAll(top10EndDateCssSelector);

            int  i = 0;
            foreach (var node in top10EndDateNodes)
            {
                // 数据样例2021 - 09 - 30
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyy-MM-dd";

                var date = DateTime.Parse(node.InnerHtml.Trim(), dtFormat);

                var EndDate = Timestamp.FromDateTime(
                DateTime.SpecifyKind(date, DateTimeKind.Utc));

                string top10Selector = $"#fher_{i + 1} > table > tbody:nth-child(3) > tr";
                i++;
                var top10DataNodes = dom.QuerySelectorAll(top10Selector);

                foreach (var top10node in top10DataNodes)
                {
                    Top10FloatHolderDto top10FloatHolderDto = new Top10FloatHolderDto();
                    top10FloatHolderDto.TSCode = tsCode;
                    top10FloatHolderDto.EndDate = EndDate;

                    //股东名称
                    top10FloatHolderDto.HolderName = top10node.QuerySelector("th>a").InnerHtml.Trim();

                    //持股数量
                    var amount = top10node.QuerySelector("td:nth-child(2)").InnerHtml.Trim();
                    top10FloatHolderDto.HoldAmount = ParseNum(amount);



                    //持股变化 变动性质:1 不变，2新进，3增加，4减少
                    var shareholdingChangeNode = top10node.QuerySelector("td:nth-child(3)>span");
                    if (shareholdingChangeNode == null)
                    {
                        //不变
                        top10FloatHolderDto.HoldChangeCharacter = 1;
                        top10FloatHolderDto.HoldChange = 0;
                    }
                    else
                    {

                        string s = shareholdingChangeNode.TextContent.Trim();
                        if (s == "新进")
                        {
                            top10FloatHolderDto.HoldChangeCharacter = 2;
                            top10FloatHolderDto.HoldChange = top10FloatHolderDto.HoldAmount;
                        }
                        else
                        {

                            top10FloatHolderDto.HoldChange = ParseNum(s);

                            if (top10FloatHolderDto.HoldChange > 0)
                            {
                                //增加
                                top10FloatHolderDto.HoldChangeCharacter = 3;
                            }
                            else
                            {
                                //减少
                                top10FloatHolderDto.HoldChangeCharacter = 4;
                            }
                        }

                    }

                    //比例
                    var ratio = top10node.QuerySelector("td:nth-child(4)").InnerHtml.Trim();

                    top10FloatHolderDto.HoldRatio = ParseRatio(ratio);

                    top10FloatHolderDtos.Add(top10FloatHolderDto);
                }
            }


            return top10FloatHolderDtos;
        }


        /// <summary>
        /// 统一转换为万为单位
        /// </summary>
        /// <param name="strNum"></param>
        /// <returns></returns>
        private double ParseNum(string strNum)
        {
            double num = 0;
            string qian = "千";
            string wan = "万";
            string yi = "亿";

            if (strNum.Contains(qian))
            {
                num = double.Parse(strNum.Substring(0, strNum.Length - 1)) / 10;
            }
            else if (strNum.Contains(wan))
            {
                num = double.Parse(strNum.Substring(0, strNum.Length - 1));
            }
            else if (strNum.Contains(yi))
            {
                num = double.Parse(strNum.Substring(0, strNum.Length - 1)) * 10000;
            }
            else if (strNum == "-")
            {
                //-1 表示没有数据
                num = -1;
            }
            else
            {
                num = double.Parse(strNum) / 10000;

            }

            return num;
        }


        /// <summary>
        /// 提取变动比率
        /// </summary>
        /// <param name="strRatio"></param>
        /// <returns></returns>
        private double ParseRatio(string strRatio)
        {
            double ratio = -1;
            if (strRatio == "-")
            {
                ratio = -1;
            }
            else
            {
                ratio = double.Parse(strRatio.Substring(0, strRatio.Length - 1));
            }
            return ratio;
        }
    }
}
