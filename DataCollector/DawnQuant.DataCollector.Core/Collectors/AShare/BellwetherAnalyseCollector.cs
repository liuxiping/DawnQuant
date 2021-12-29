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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    /// <summary>
    /// 龙头股采集
    /// </summary>
    public class BellwetherAnalyseCollector
    {
        public BellwetherAnalyseCollector(ILogger logger, CollectorConfig config,
         IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;

            _config = config;
        }

        CollectorConfig _config;
        ILogger _logger;
        IPassportProvider _passportProvider;


        /// <summary>
        /// 同花顺公司亮点
        /// </summary>
        public event Action<string> AnalyseBellwetherFromTHSLightspotProgressChanged;

        public List<string> UnCompleteStocks { get; set; }

        protected void OnAnalyseBellwetherFromTHSLightspotProgressChanged(string msg)
        {

            AnalyseBellwetherFromTHSLightspotProgressChanged?.Invoke(msg);

        }

        /// <summary>
        /// 行业对比
        /// </summary>
        public List<string> UnCompleteIndustries { get; set; }
        public event Action<string> AnalyseBellwetherFromTHSIndustryCompareProgressChanged;
        protected void OnAnalyseBellwetherFromTHSIndustryCompareProgressChanged(string msg)
        {

            AnalyseBellwetherFromTHSIndustryCompareProgressChanged?.Invoke(msg);

        }

        /// <summary>
        /// 从同花顺公司亮点中提取龙头股
        /// </summary>
        public void AnalyseBellwetherFromTHSCompanyLightspot()
        {

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
            List<string> alltscodes = new List<string>();
            List<string> completetscodes = new List<string>();
            List<BellwetherDto> bellwethers = new List<BellwetherDto>();

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
                   if(AnalyseSingleBellwetherFromTHSCompanyLightspot(tscode,out  string lightspot))
                    {
                        //1 表示公司亮点分析出来
                        bellwethers.Add(new BellwetherDto { TSCode=tscode,Source=1,Remark= lightspot });
                    }
                    completetscodes.Add(tscode);
                    complete++;
                    string msg = $"龙头股(同花顺公司亮点)已经成功分析{complete}个股票，总共{allCount}个股票";
                    OnAnalyseBellwetherFromTHSLightspotProgressChanged(msg);

                }
                
            }
            finally
            {
                UnCompleteStocks = alltscodes.Except(completetscodes).ToList();

                if (bellwethers.Count > 0)
                {
                    string msg = $"成功分析出{bellwethers.Count}个龙头股票(同花顺公司亮点)，正在保存数据...";
                    OnAnalyseBellwetherFromTHSLightspotProgressChanged(msg);
                    var bclient = new BellwetherApi.BellwetherApiClient(channel);
                    var request = new SaveBellwethersRequest();
                    request.Entities.AddRange(bellwethers);
                    bclient.SaveBellwethers(request, meta);
                    msg = $"成功分析出{bellwethers.Count}个龙头股票(同花顺公司亮点)，数据保存成功";
                    OnAnalyseBellwetherFromTHSLightspotProgressChanged(msg);
                }

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
        private  bool AnalyseSingleBellwetherFromTHSCompanyLightspot(string tsCode,out string outLightspot)
        {
            bool isBellwether=false;
            outLightspot = "";
            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id + "/");

            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);

                t.Wait();

                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                var lightspotNode = dom.QuerySelector("#profile > div.bd > table:nth-child(1) > tbody > tr:nth-child(1) > td:nth-child(1) > span.tip.f14.fl.core-view-text");

                if (lightspotNode != null)
                {
                    string lightspot = lightspotNode.TextContent.Trim();

                    outLightspot = lightspot;

                    isBellwether = IsBellwetherFromCompanyLightspot(lightspot);

                }
                else
                {
                    _logger.LogError($"提取不到股票代码为{id}的行业信息");
                }

                return isBellwether;
            }
        }

        private bool IsBellwetherFromCompanyLightspot(string lightspot)
        {
            bool bellwether = false;
            if (!string.IsNullOrWhiteSpace(lightspot))
            {
                foreach (var key in _config.BellwetherKeyWords)
                {
                    if (lightspot.Contains(key))
                    {
                        bellwether = true;
                        break;
                    }
                }
            }
            return bellwether;
        }


        /// <summary>
        /// 从同花顺行业对比中提取行业龙头股
        /// </summary>
        public void AnalyseBellwetherFromTHSIndustryCompare()
        {

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
            List<string> allIndustries = new List<string>();

            List<string> completetIndustries = new List<string>();

            List<BellwetherDto> bellwethers = new List<BellwetherDto>();

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            try
            {
                var client = new IndustryApi.IndustryApiClient(channel);

                var response = client.GetThreeLevelIndustries(new Empty(), meta);


                allIndustries= response.Entities.Select(x => x.Id.ToString()).ToList();

                int allCount = response.Entities.Count;

                int complete = 0;

                var tempStockIds=new List<string>();

                foreach (var industry in response.Entities)
                {
                    //根据行业获取

                    var bclient = new BasicStockInfoApi.BasicStockInfoApiClient(channel);
                    var iResponse = bclient.GetThreeStockByIndustry(new GetThreeStockByIndustryRequest() { IndustryId = industry.Id }, meta);

                    string tscode1 = "";
                    if (iResponse.TSCodes.Count > 0)
                    {
                        tscode1 = iResponse.TSCodes[0];
                    }
                    string tscode2 = "";
                    if (iResponse.TSCodes.Count > 1)
                    {
                        tscode2 = iResponse.TSCodes[1];
                    }
                    string tscode3 = "";
                    if (iResponse.TSCodes.Count > 2)
                    {
                        tscode3 = iResponse.TSCodes[2];
                    }

                    //第一个数据尝试
                    if (!string.IsNullOrEmpty(tscode1))
                    {
                        var t1 = AnalyseSingleBellwetherFromTHSIndustryCompare(tscode1);
                        if (t1 != null && t1.Count > 0)
                        {
                            tempStockIds.AddRange(t1);
                            //1 表示公司亮点分析出来
                        }
                        else
                        {
                            //第一个数据没有采集到 使用第二个
                            if (!string.IsNullOrEmpty(tscode2))
                            {
                                var t2 = AnalyseSingleBellwetherFromTHSIndustryCompare(tscode2);
                                if (t2 != null && t2.Count > 0)
                                {
                                    tempStockIds.AddRange(t2);
                                }
                                else
                                {
                                    //第二个数据没有采集到 使用第三个
                                    if (!string.IsNullOrEmpty(tscode3))
                                    {
                                        var t3 = AnalyseSingleBellwetherFromTHSIndustryCompare(tscode3);
                                        if (t3 != null && t3.Count > 0)
                                        {
                                            tempStockIds.AddRange(t3);
                                        }
                                    }
                                }
                            }
                        }
                        completetIndustries.Add(industry.Id.ToString());
                        complete++;

                        string msg = $"龙头股(同花顺行业对比)已经成功分析{complete}个行业，总共{allCount}个行业";
                        OnAnalyseBellwetherFromTHSIndustryCompareProgressChanged(msg);

                    }

                    
                }

                foreach (var t in tempStockIds.Distinct())
                {
                    //钻换成TSCodes
                    string tscode = "";
                    if (t.Substring(0, 1) == "0" ||//深圳主板
                        t.Substring(0, 1)=="3")//深圳创业板
                    {
                        tscode = t + ".SZ";
                    }
                    else if (t.Substring(0,1)=="6") //上海
                    {
                        tscode = t + ".SH";
                    }
                    else
                    {
                        continue;
                    }
                    bellwethers.Add(new BellwetherDto { TSCode = tscode, Source = 2 });
                }
            }
            finally
            {
                UnCompleteIndustries = allIndustries.Except(completetIndustries).ToList();

                if (bellwethers.Count > 0)
                {
                    string msg = $"成功分析出{bellwethers.Count}个龙头股票(同花顺行业对比)，正在保存数据...";
                    OnAnalyseBellwetherFromTHSIndustryCompareProgressChanged(msg);
                    var bclient = new BellwetherApi.BellwetherApiClient(channel);
                    var request = new SaveBellwethersRequest();
                    request.Entities.AddRange(bellwethers);
                    bclient.SaveBellwethers(request, meta);
                    msg = $"成功分析出{bellwethers.Count}个龙头股票(同花顺行业对比)，数据保存成功";
                    OnAnalyseBellwetherFromTHSIndustryCompareProgressChanged(msg);
                }

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
        private List<string> AnalyseSingleBellwetherFromTHSIndustryCompare(string tsCode)
        {
            var alltscodes = new List<string>();
           
            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id + "/field.html");

            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);

                t.Wait();

                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                var industryCompareNode = dom.QuerySelector("#fieldsChartData");

                if (industryCompareNode != null)
                {
                    string data = industryCompareNode.GetAttribute("value");

                    alltscodes = AnalyseBellwetherFromIndustryCompare(data);

                }
                else
                {
                    _logger.LogError($"提取不到股票代码为{id}的行业信息");
                }

                return alltscodes;
            }
        }


        /// <summary>
        /// 分析提取龙头股
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<string> AnalyseBellwetherFromIndustryCompare(string data)
        {
            List<string> ftscodes = new List<string>();
            if (!string.IsNullOrEmpty(data))
            {
                var dom = JsonDocument.Parse(data);

                //最近一期的数据
                var recent = dom.RootElement.EnumerateObject().ToList()[0];

                //提取数据
                List<Performance> performances = new List<Performance>();
                foreach (var item in recent.Value.EnumerateArray())
                {
                    var d = item.EnumerateArray().ToList();
                    Performance p = new Performance();

                    p.StockId = d[0].ToString();
                    p.StockName = d[1].ToString();
                    p.EarningsPerShare = double.Parse(d[2].ToString());
                    p.BVPS = double.Parse(d[3].ToString());
                    p.CashFlowPerShare = double.Parse(d[4].ToString());
                    p.RetainedProfits = double.Parse(d[5].ToString());
                    p.OperatingIncome = double.Parse(d[6].ToString());
                    p.TotalAssets = double.Parse(d[7].ToString());
                    p.ROE = double.Parse(d[8].ToString());
                    p.EquityRatio = double.Parse(d[9].ToString());
                    p.GrossProfitMargin = double.Parse(d[10].ToString());
                    performances.Add(p);
                }

                //提取龙头股

                //净资产收益率排行
                int take = 6;
                var roe = performances.OrderByDescending(p => p.ROE).Take(take);
                var rf = performances.OrderByDescending(p => p.RetainedProfits).Take(take);
                var io = performances.OrderByDescending(p => p.OperatingIncome).Take(take);

                //取交集
                var inter = roe.Intersect(rf).Intersect(io);

                //最终只取三个 净利润排序
                 ftscodes = inter.OrderByDescending(p => p.RetainedProfits).Select(p => p.StockId).Take(3).ToList();
            }
            return ftscodes;


         }

    }

   internal class Performance
    {
        public string StockId { get; set; }
        public string StockName { get; set; }
        
        /// <summary>
        /// 每股收益
        /// </summary>
        public double EarningsPerShare { get; set; }

        /// <summary>
        /// 每股净资产
        /// </summary>
        public double BVPS { get; set; }

        /// <summary>
        /// 每股现金流
        /// </summary>
        public double CashFlowPerShare { get; set; }

        /// <summary>
        /// 净利润
        /// </summary>
        public double RetainedProfits { get; set; }


        /// <summary>
        /// 营业总收入
        /// </summary>
        public double OperatingIncome { get; set; }

        /// <summary>
        /// 总资产
        /// </summary>
        public double TotalAssets  { get; set; }

        /// <summary>
        /// 净资产收益率
        /// </summary>
        public double ROE { get; set; }


        /// <summary>
        /// 股东权益比率
        /// </summary>
        public double EquityRatio { get; set; }

        /// <summary>
        /// 销售毛利率
        /// </summary>
        public double GrossProfitMargin { get; set; }
    }
}
