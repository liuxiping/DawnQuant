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
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.Index.THSIndex;
using TuShareHttpSDKLibrary.Model.Index.THSIndexMember;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    /// <summary>
    /// 从tushare 采集 同花顺指数数据
    /// </summary>
    public class THSIndexCollector
    {
        public THSIndexCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _config = config;
        }

        CollectorConfig _config;
        ILogger _logger;
        IPassportProvider _passportProvider;

        public event Action<string> CollectTHSIndexMemberProgress;
        protected void OnCollectTHSIndexMemberProgress(string msg)
        {
            CollectTHSIndexMemberProgress?.Invoke(msg);
        }

        public event Action<string> CollectTHSIndexDailyTradeDataProgress;
        protected void OnCollectTHSIndexDailyTradeDataProgress(string msg)
        {
            CollectTHSIndexDailyTradeDataProgress?.Invoke(msg);
        }

        /// <summary>
        /// 同花顺指数
        /// </summary>
        public void CollectTHSIndexFromTushare()
        {
            GrpcChannel channel = null;
            try
            {
                TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);

                THSIndexRequestModel requestModel = new THSIndexRequestModel();

                //A股
                requestModel.Exchange = "A";
                var call = tu.GetData(requestModel);
                call.Wait();
                var datas = call.Result;

                List<THSIndexDto> dtos = new List<THSIndexDto>();

                foreach (var item in datas)
                {
                    THSIndexDto dto = new THSIndexDto();
                    dto.TSCode = item.TSCode;
                    dto.Name = item.Name;
                    dto.Count = item.Count;
                    dto.Type = item.Type;
                    dto.Exchange = item.Exchange;

                    if (!string.IsNullOrEmpty(item.ListDate))
                    {
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyyMMdd";
                        dto.ListDate = Timestamp.FromDateTime(
                          DateTime.SpecifyKind(DateTime.ParseExact(item.ListDate, "yyyyMMdd", dtFormat),
                          DateTimeKind.Utc));
                    }


                    dtos.Add(dto);
                }

                channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
                var client = new THSIndexApi.THSIndexApiClient(channel);
                var request = new SaveTHSIndexesRequest();
                request.Entities.AddRange(dtos);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                client.SaveTHSIndexes(request, meta);

            }
            catch (Exception ex)
            {
                if (channel != null)
                {
                    channel.Dispose();

                }
                string msg = "采集上市公司信息发生错误：\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                _logger.LogError(msg);
                throw;
            }
        }


        /// <summary>
        /// 同花顺指数成员
        /// </summary>
        /// <returns></returns>
        public async Task CollectTHSIndexMemberFromTushare()
        {

            GrpcChannel channel = null;
            try
            {
                //提取指数代码
                channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
                var client = new THSIndexApi.THSIndexApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var indexes = client.GetAllTHSIndexes(new Empty(), meta);

                var tscodes = indexes.Entities.Where(p => p.ListDate != null)
                    .Select(p => p.TSCode).ToList();

                tscodes = tscodes.Distinct().ToList();

                int allCount = tscodes.Count;
                int complete = 0;

                //清空数据
                var mclient = new THSIndexMemberApi.THSIndexMemberApiClient(channel);
                mclient.EmptyTHSIndexMembers(new Empty(), meta);

                foreach (var tscode in tscodes)
                {
                    //一分钟只能调用200次 延时下
                    await Task.Delay(350);

                    TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);

                    THSIndexMemberRequestModel requestModel = new THSIndexMemberRequestModel();


                    requestModel.TSCode = tscode;
                    List<string> fields = new List<string>();
                    fields.AddRange(new[] { "ts_code", "code", "name", "weight", "in_date", "out_date", "is_new" });
                    var call = tu.GetData(requestModel, string.Join(",", fields));
                    call.Wait();
                    var datas = call.Result;

                    List<THSIndexMemberDto> dtos = new List<THSIndexMemberDto>();

                    foreach (var item in datas)
                    {
                        THSIndexMemberDto dto = new THSIndexMemberDto();
                        dto.TSCode = item.TSCode;
                        dto.Code = item.Code;
                        dto.Name = item.Name;
                        dto.IsNew = item.IsNew;

                        if (!string.IsNullOrEmpty(item.InDate))
                        {
                            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                            dtFormat.ShortDatePattern = "yyyyMMdd";
                            dto.InDate = Timestamp.FromDateTime(
                              DateTime.SpecifyKind(DateTime.ParseExact(item.InDate, "yyyyMMdd", dtFormat),
                              DateTimeKind.Utc));
                        }

                        if (!string.IsNullOrEmpty(item.OutDate))
                        {
                            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                            dtFormat.ShortDatePattern = "yyyyMMdd";
                            dto.OutDate = Timestamp.FromDateTime(
                              DateTime.SpecifyKind(DateTime.ParseExact(item.OutDate, "yyyyMMdd", dtFormat),
                              DateTimeKind.Utc));
                        }


                        dtos.Add(dto);
                    }

                    var request = new SaveTHSIndexMembersRequest();
                    request.Entities.AddRange(dtos);
                    mclient.SaveTHSIndexMembers(request, meta);

                    complete++;
                    string msg = $"同花顺指数成分股(Tushare)已经成功采集{complete}个指数，总共{allCount}个指数";

                    OnCollectTHSIndexMemberProgress(msg);

                }

            }
            catch (Exception ex)
            {
                if (channel != null)
                {
                    channel.Dispose();

                }
                string msg = "采集同花顺指数成分股(Tushare)发生错误：\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                _logger.LogError(msg);
                throw;
            }
        }


        /// <summary>
        /// 指数日线数据
        /// </summary>
        public void CollectTHSIndexDailyTradeDataFromTushare()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl))
            {

                var client = new THSIndexApi.THSIndexApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var response = client.GetAllTHSIndexes(new Empty(), meta);

                var indexes = response.Entities.Where(p => p.ListDate != null).DistinctBy(p => p.TSCode).ToList();

                int allCount = indexes.Count();
                int complete = 0;

                foreach (var indexDto in indexes)
                {
                    CollectSingleTHSIndexDailyTradeDataFromTushare(indexDto, channel);
                    complete++;
                    string msg = $"同花顺指数日线数据已经成功采集{complete}个指数，总共{allCount}个指数";
                    CollectTHSIndexDailyTradeDataProgress(msg);
                }

            }

        }

        /// <summary>
        /// 提取单个数据
        /// </summary>
        /// <param name="tscode"></param>
        /// <param name="channel"></param>
        private void CollectSingleTHSIndexDailyTradeDataFromTushare(THSIndexDto indexDto, GrpcChannel channel)
        {
            //每次最多返回5000条数据。必须分批次存取数据
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            //上市日期
            DateTime listingDate = indexDto.ListDate.ToDateTime();

            //一次取20年数据，获取需要取数据的次数
            int loopCount = (int)Math.Ceiling(
                ((DateTime.Now.Year - listingDate.Year) + 1) / 20.0);

            TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);

            THSIndexDailyTradeDataRequestModel requestModel = new THSIndexDailyTradeDataRequestModel();
            requestModel.TSCode = indexDto.TSCode;

            for (int i = 0; i < loopCount; i++)
            {
                if (i == 0)
                {
                    requestModel.StartDate = listingDate.ToString("yyyyMMdd");
                }
                else
                {
                    requestModel.StartDate = listingDate.AddYears((i * 20)).Year.ToString() + "0101";
                }

                DateTime endDate = new DateTime(listingDate.Year + ((i + 1) * 20 - 1), 12, 31);
                requestModel.EndDate = endDate.ToString("yyyyMMdd");

                List<string> fields = new List<string>();
                fields.AddRange(new[]
                { "ts_code", "trade_date", "close", "open", "high","low",
                    "pre_close", "avg_price","change","pct_change","vol","turnover_rate"
                    ,"total_mv","float_mv"
                });

                //提取数据
                var task = tu.GetData(requestModel, string.Join(",", fields));
                task.Wait();

                SaveSingleTHSIndexDailyTradeDataFromTushare(task.Result, indexDto.TSCode, channel);
            }
        }


        private void SaveSingleTHSIndexDailyTradeDataFromTushare(List<THSIndexDailyTradeDataResponseModel> response, string tscode, GrpcChannel channel)
        {
            SaveAllTHSIndexTradeDatasRequest request = new SaveAllTHSIndexTradeDatasRequest();
            request.TSCode = tscode;
            request.KCycle = KCycleDto.Day;

            foreach (var item in response)
            {
                THSIndexTradeDataDto tradeDataDto = new THSIndexTradeDataDto();

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";
                tradeDataDto.TradeDateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(
                    DateTime.ParseExact(item.TradeDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));

                tradeDataDto.Open = item.Open;
                tradeDataDto.Close = item.Close;
                tradeDataDto.High = item.High;
                tradeDataDto.Low = item.Low;
                tradeDataDto.Amount = item.Vol * item.AvgPrice;
                tradeDataDto.Volume = item.Vol;
                tradeDataDto.PreClose = item.PreClose;
                tradeDataDto.Change = item.Change;
                tradeDataDto.AvgClose = item.AvgPrice;
                tradeDataDto.FloatMV = item.FloatMV;
                tradeDataDto.TotalMV = item.TotalMV;
                tradeDataDto.PctChange = item.PctChange;

                tradeDataDto.Turnover = item.TurnoverRate;


                request.Entities.Add(tradeDataDto);
            }

            if (request.Entities.Count > 0)
            {
                var client = new THSIndexTradeDataApi.THSIndexTradeDataApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                client.SaveAllTHSIndexTradeDatas(request, meta);
            }
        }


        // <summary>
        /// 同花顺指数成员
        /// </summary>
        /// <returns></returns>
        public  void CollectTHSIndexMemberFromTHS()
        {
            GrpcChannel channel = null;
            try
            {
                //提取指数代码
                channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
                var client = new THSIndexApi.THSIndexApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var indexes = client.GetAllTHSIndexes(new Empty(), meta);

                var tscodes = indexes.Entities.Where(p => p.ListDate != null)
                    .Select(p => p.TSCode).ToList();

                tscodes = tscodes.Distinct().ToList();

                int allCount = tscodes.Count;
                int complete = 0;

                var mclient = new THSIndexMemberApi.THSIndexMemberApiClient(channel);

                foreach (var tscode in tscodes)
                {

                    var names = CollectSingleTHSIndexMemberFromTHS(tscode);
                    if (names != null && names.Count > 0)
                    {
                        var request = new SaveTHSIndexMembersByNameRequest();
                        request.TSCode= tscode;
                        request.Names.AddRange(names);
                        mclient.SaveTHSIndexMembersByName(request, meta);
                    }

                    complete++;
                    string msg = $"同花顺指数成分股(THS)已经成功采集{complete}个指数，总共{allCount}个指数";
                    OnCollectTHSIndexMemberProgress(msg);

                }

            }
            catch (Exception ex)
            {
                if (channel != null)
                {
                    channel.Dispose();

                }
                string msg = "采集同花顺指数成分股(THS)发生错误：\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                _logger.LogError(msg);
                throw;
            }
        }

        public  List<string> CollectSingleTHSIndexMemberFromTHS(string tsCode)
        {
            List<string> names = new List<string>();

            //http://basic.10jqka.com.cn/885945/
            string id = tsCode.Substring(0, 6);
            //获取行业分类
            string fieldUrl = string.Concat(@"http://basic.10jqka.com.cn/", id, @"/");

            using (HttpClient client = new HttpClient())
            {
                var t = client.GetStreamAsync(fieldUrl);
                t.Wait();
                HtmlParser htmlParser = new HtmlParser();
                var dom = htmlParser.ParseDocument(t.Result);

                if(id== "885950")
                {

                }

                string css1 = "#c_table > table > tbody > tr > td:nth-child(2) a";

                string css2 = "#hy3_table_1 > tbody > tr > td:nth-child(2) a";

                var nameNodes = dom.QuerySelectorAll(css1);

                if(nameNodes==null || nameNodes.Count() <= 0)
                {
                    nameNodes= dom.QuerySelectorAll(css2);
                }

                if (nameNodes != null && nameNodes.Count()>0)
                {
                    foreach (var nameNode in nameNodes)
                    {
                        names.Add(nameNode.TextContent);
                    }

                }
                else
                {
                    _logger.LogError($"提取不到同花顺指数代码为{id}的成分股信息");
                }




            }

            return names;
        }




        
    }
}
