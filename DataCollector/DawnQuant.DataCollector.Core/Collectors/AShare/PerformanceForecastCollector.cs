using AngleSharp.Html.Parser;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class PerformanceForecastCollector
    {

        public PerformanceForecastCollector(ILogger logger, CollectorConfig config,
         IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _config = config;
        }

        CollectorConfig _config;
        ILogger _logger;
        IPassportProvider _passportProvider;


        public event Action<string> CollectPerformanceForecastProgressChanged;
        public List<string> UnCompleteStocks { get; set; }
        protected void OnCollectPerformanceForecastProgressChanged(string msg)
        {

            CollectPerformanceForecastProgressChanged?.Invoke(msg);

        }

        /// <summary>
        /// 从同花顺公司亮点中提取龙头股
        /// </summary>
        public void CollectPerformanceForecastFromTHS()
        {

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
            List<string> alltscodes = new List<string>();
            List<string> completetscodes = new List<string>();
            List<PerformanceForecastDto> pfs = new List<PerformanceForecastDto>();


            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            try
            {

                var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);
                var response = client.GetAllTSCodes(new Empty(), meta);
                alltscodes.AddRange(response.TSCodes);
                int allCount = response.TSCodes.Count;

                int complete = 0;

                foreach (var tscode in alltscodes)
                {
                    var pf = CollectSinglePerformanceForecastFromTHS(tscode);
                    if(pf != null)
                    {
                        pf.TSCode = tscode;
                        pf.Source = 1;
                        pfs.Add(pf);
                       

                    }
                    completetscodes.Add(tscode);
                    complete++;
                    string msg = $"业绩预测已经成功分析{complete}个股票，总共{allCount}个股票";
                    OnCollectPerformanceForecastProgressChanged(msg);

                }

             

            }
            finally
            {
                //保存提取的数据
                if (pfs.Count > 0)
                {
                    string msg = $"业绩预测已经成功提取个{pfs.Count}股票，正在保存数据...";
                    OnCollectPerformanceForecastProgressChanged(msg);
                    var pfclient = new PerformanceForecastApi.PerformanceForecastApiClient(channel);
                    var request = new SavePerformanceForecastsRequest();
                    request.Entities.AddRange(pfs);
                    pfclient.SavePerformanceForecasts(request, meta);

                    msg = $"业绩预测已经成功提取个{pfs.Count}股票，保存数据完成！";
                    OnCollectPerformanceForecastProgressChanged(msg);
                }

                UnCompleteStocks = alltscodes.Except(completetscodes).ToList();
                if (channel != null)
                {
                    channel.Dispose();
                }
            }
        }


        /// <summary>
        /// 提取单个公司信息
        /// </summary>
        /// <param name="tscode"></param>
        /// <param name="channel"></param>
        /// <exception cref="NotImplementedException"></exception>
        private PerformanceForecastDto CollectSinglePerformanceForecastFromTHS(string tsCode)
        {
            PerformanceForecastDto pf = null;
            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id + "/worth.html");

            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);

                t.Wait();

                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                var forecastNode = dom.QuerySelector("#forecast > div.bd > p.tip.clearfix");

                if (forecastNode != null)
                {
                    string p = forecastNode.TextContent.Trim();

                     pf = ParsePerformanceForecast(p);

                }
                else
                {
                    _logger.LogError($"提取不到股票代码为{id}的业绩预测");
                }

                return pf;
            }
        }

        /// <summary>
        /// 提起业绩预测信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private PerformanceForecastDto ParsePerformanceForecast(string p)
        {
            PerformanceForecastDto pf = null;

            if (!string.IsNullOrEmpty(p) && p != "本年度暂无机构做出业绩预测")
            {
                pf = new PerformanceForecastDto();

                //分析数据
                var ps = p.Split("；");
                //截止日期
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";
                var date = p.Substring(ps[0].IndexOf("截至") + 2, 10);

                pf.EndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Parse(date, dtFormat), DateTimeKind.Utc));

                //机构数量
                int counts = ps[0].IndexOf("共有");
                int counte = ps[0].IndexOf("家机构");
                var count = ps[0].Substring(ps[0].IndexOf("共有") + 2, counte - counts - 3);
                pf.ForecastOrgCount = int.Parse(count);



                var p2 = new List<string>();
                foreach (var s in ps[1].Split("，"))
                {
                    p2.Add(s.Trim());
                }

                //每股收益
                int eps_s = p2[0].IndexOf("益") + 1;
                int eps_e = p2[0].Length - 2;
                var eps = p2[0].Substring(eps_s, eps_e - eps_s + 1).Trim();
                if (!string.IsNullOrEmpty(eps))
                {
                    pf.EarningsPerShare = double.Parse(eps);
                }

                //每股收益变动
                int epsRatio_s = p2[1].IndexOf("比") + 3;
                int epsRatio_e = p2[1].Length - 2;
                var epsRatio = p2[1].Substring(epsRatio_s, epsRatio_e - epsRatio_s + 1);
                if (!string.IsNullOrEmpty(epsRatio))
                {
                    pf.EarningsPerShareChangeRatio = double.Parse(epsRatio);

                    if (p2[1].Substring(epsRatio_s + 1-3, 2) == "下降")
                    {
                        pf.EarningsPerShareChangeRatio = -pf.EarningsPerShareChangeRatio;
                    }
                }


                //净利润
                int rf_s = p2[2].IndexOf("润") + 2;
                int rf_e = p2[2].Length - 3;
                var rf = p2[2].Substring(rf_s, rf_e - rf_s);
                if (!string.IsNullOrEmpty(rf))
                {
                    pf.RetainedProfits = double.Parse(rf);
                }

                //净利润
                int rfRatio_s = p2[3].IndexOf("比") + 3;
                int rfRatio_e = p2[3].Length - 2;
                var rfRatio = p2[3].Substring(rfRatio_s, rfRatio_e - rfRatio_s + 1);

                if (!string.IsNullOrEmpty(rfRatio))
                {

                    pf.RetainedProfitsChangeRatio = double.Parse(rfRatio);
                    if (p2[3].Substring(rfRatio_s -3+ 1, 2) == "下降")
                    {
                        pf.RetainedProfitsChangeRatio = -pf.RetainedProfitsChangeRatio;
                    }
                }
            }

            return pf;
        }
    }

}
