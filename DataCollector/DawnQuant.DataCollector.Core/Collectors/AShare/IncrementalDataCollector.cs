using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.DataCollector.Core.Utils;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.Index.THSIndex;
using TuShareHttpSDKLibrary.Model.MarketData;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class IncrementalDataCollector
    {

        ILogger _logger;
        JobMessageUtil _jobMessageUtil;
        IPassportProvider _passportProvider;
        CollectorConfig _config;
        public IncrementalDataCollector(IPassportProvider passportProvider,
            ILogger logger, CollectorConfig config, JobMessageUtil jobMessageUtil)
        {

            _logger = logger;
            _passportProvider = passportProvider;
            _config = config;
            _jobMessageUtil = jobMessageUtil;
        }

        #region  从新浪获取实时数据
        const string DTDUrlPart1FromSina =
           @"http://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/Market_Center.getHQNodeData?page=";
        const string DTDUrlPart2FromSina =
            @"&num=80&sort=symbol&asc=1&node=hs_a";

        /// <summary>
        /// 获取新浪所有A股实时数据分页地址
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private string GetDTDPagedUrl(int page)
        {
            return DTDUrlPart1FromSina + page.ToString() + DTDUrlPart2FromSina;
        }


        /// <summary>
        /// 从新浪获取实时数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private  Dictionary<string, StockTradeDataDto> CollectInDailyTradeDataFromSina(DateTime date, GrpcChannel channel)
        {

            Dictionary<string, StockTradeDataDto> stockTradeDatas = new Dictionary<string, StockTradeDataDto>();

            if (IsOpen(date, channel))
            {
                //读取增量数据
                using (HttpClient httpClient = new HttpClient())
                {
                    int page = 1;
                    while (true)
                    {
                        var tStream =  httpClient.GetStreamAsync(GetDTDPagedUrl(page));

                        tStream.Wait();
                        var tJsonDocument =  JsonDocument.Parse(tStream.Result);


                        if (tJsonDocument != null && tJsonDocument.RootElement.GetArrayLength() > 0)
                        {
                            foreach (JsonElement e in tJsonDocument.RootElement.EnumerateArray())
                            {

                                StockTradeDataDto tradeData = new StockTradeDataDto();
                                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                                tradeData.TradeDateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(
                                    date, DateTimeKind.Utc));

                                tradeData.Open = double.Parse( e.GetProperty("open").GetString());
                                tradeData.Close = double.Parse(e.GetProperty("trade").GetString());
                                tradeData.High = double.Parse(e.GetProperty("high").GetString());
                                tradeData.Low = double.Parse(e.GetProperty("low").GetString());
                                tradeData.Amount = e.GetProperty("amount").GetDouble()/1000;
                                tradeData.Volume = e.GetProperty("volume").GetDouble()/100;
                                tradeData.PreClose = double.Parse(e.GetProperty("settlement").GetString());
                                tradeData.Turnover = e.GetProperty("turnoverratio").GetDouble();

                                string symbol = e.GetProperty("symbol").GetString().ToUpper();
                                string tsCode = symbol.Substring(2, 6)+"."+ symbol.Substring(0, 2);

                                //无效数据
                                if (tradeData.Open == 0 || tradeData.Close == 0 || tradeData.High == 0 ||
                                     tradeData.Low == 0 || tradeData.Volume == 0 || tradeData.Amount == 0)
                                {
                                    continue;
                                }

                                stockTradeDatas.Add(tsCode, tradeData);
                            }
                        }
                        else
                        {
                            break;
                        }
                        page++;
                    }

                }

            }

            return stockTradeDatas;
        }

        /// <summary>
        /// 从新浪更新实时数据
        /// </summary>
        /// <param name="date"></param>
        public async  Task CollectInDailyTradeDataFromSina(DateTime date)
        {
            string msg;

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);

            try
            {
                msg = $"正在从Sina获取增量日线数据...";
                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);

                var datas =   CollectInDailyTradeDataFromSina(date, channel);

                int allCount = datas.Count;

                msg = $"从Sina成功获取{allCount}个增量日线数据...";
                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);

                var client = new StockTradeDataApi.StockTradeDataApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                List<SingleStockTradeData> tradeDatas = new List<SingleStockTradeData>();
                foreach (var d in datas)
                {
                    tradeDatas.Add(new SingleStockTradeData
                    {
                        KCycle = KCycleDto.Day,
                        TSCode = d.Key,
                        Entity = d.Value
                    });

                }
                BatchSaveInSTDAndCAFRequest request = new BatchSaveInSTDAndCAFRequest();
                request.Entities.Add(tradeDatas);

                using (var call = client.BatchSaveInSTDAndCAF(request, meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        msg = $"正在保存增量日线数据(Sina)，已经成功保存{response.CompletCount}个，总共{response.TotalCount}个";
                        _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);
                    }
                }

                msg = $"保存增量日线数据(Sina)已完成，总共保存{datas.Count}个";

                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);



            }
            finally
            {
                channel?.Dispose();
            }
        }

        #endregion


        private Dictionary<string, StockTradeDataDto> CollectIncrementDailyTradeDataFromTushare(DateTime date, GrpcChannel channel)
        {

            Dictionary<string, StockTradeDataDto> stockTradeDatas = new Dictionary<string, StockTradeDataDto>();

            if (IsOpen(date, channel))
            {
                //读取增量数据
                TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);
                DailyRequestModel requestModel = new DailyRequestModel();
                requestModel.TradeDate = date.ToString("yyyyMMdd");

                var task = tu.GetData(requestModel);
                task.Wait();

                if (task.Result != null && task.Result.Count > 0)
                {
                    foreach (var item in task.Result)
                    {
                        StockTradeDataDto tradeData = new StockTradeDataDto();
                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyyMMdd";
                        tradeData.TradeDateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(
                            DateTime.ParseExact(item.TradeDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));

                        tradeData.Open = item.Open;
                        tradeData.Close = item.Close;
                        tradeData.High = item.High;
                        tradeData.Low = item.Low;
                        tradeData.Amount = item.Amount;
                        tradeData.Volume = item.Vol;
                        tradeData.PreClose = item.PreClose;

                        if (tradeData.Open == 0 || tradeData.Close == 0 || tradeData.High == 0 ||
                           tradeData.Low == 0 || tradeData.Volume == 0 || tradeData.Amount == 0)
                        {
                            continue;
                        }

                        stockTradeDatas.Add(item.TsCode, tradeData);
                    }
                }
            }
            return stockTradeDatas;
        }


        private Dictionary<string, StockDailyIndicatorDto> CollectInStockDailyIndicatorFromTushare(DateTime date, GrpcChannel channel)
        {
            //交易所代码
            Dictionary<string, StockDailyIndicatorDto> stockDailyIndicatorDatas = new Dictionary<string, StockDailyIndicatorDto>();


            if (IsOpen(date, channel))
            {
                //读取增量数据
                TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);
                DailyBasicRequestModel requestModel = new DailyBasicRequestModel();
                requestModel.TradeDate = date.ToString("yyyyMMdd");

                var task = tu.GetData(requestModel);
                task.Wait();

                if (task.Result != null && task.Result.Count > 0)
                {
                    foreach (var item in task.Result)
                    {
                        StockDailyIndicatorDto indicator = new StockDailyIndicatorDto();

                        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                        dtFormat.ShortDatePattern = "yyyyMMdd";
                        indicator.TradeDate = Timestamp.FromDateTime(DateTime.SpecifyKind(
                            DateTime.ParseExact(item.TradeDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));

                        indicator.Close = item.Close;
                        indicator.Turnover = item.TurnoverRate;
                        indicator.TurnoverFree = item.TurnoverRateF;
                        indicator.VolumeRatio = item.VolumeRatio;
                        indicator.PE = item.Pe;
                        indicator.PETTM = item.PeTtm;
                        indicator.PB = item.Pb;

                        indicator.PS = item.Ps;
                        indicator.PSTTM = item.PsTtm;
                        indicator.DV = item.DvRatio;
                        indicator.DVTTM = item.DvTtm;
                        indicator.TotalShare = item.TotalShare;
                        indicator.FloatShare = item.FloatShare;
                        indicator.FreeShare = item.FreeShare;

                        indicator.TotalMarketValue = item.TotalMv;
                        indicator.CirculateMarketValue = item.CircMv;

                        stockDailyIndicatorDatas.Add(item.TsCode, indicator);
                    }
                }
            }
            return stockDailyIndicatorDatas;
        }


        /// <summary>
        /// 从Tushare 获取增量日线数据
        /// </summary>
        /// <param name="date"></param>
        public  async Task CollectInDailyTradeDataFromTushare(DateTime date)
        {
            string msg;

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);

            try
            {
                msg = $"正在从Tushare获取增量日线数据...";
                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);

                var datas = CollectIncrementDailyTradeDataFromTushare(date, channel);

                int allCount = datas.Count;

                msg = $"从Tushare成功获取{allCount}个增量日线数据...";
                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);


                var client = new StockTradeDataApi.StockTradeDataApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                List<SingleStockTradeData> tradeDatas = new List<SingleStockTradeData>();
                foreach (var d in datas)
                {
                    tradeDatas.Add(new SingleStockTradeData { KCycle = KCycleDto.Day,
                        TSCode = d.Key, Entity = d.Value });
                }

                BatchSaveInSTDAndCAFRequest request = new BatchSaveInSTDAndCAFRequest();
                request.Entities.Add(tradeDatas);

                using (var call = client.BatchSaveInSTDAndCAF(request, meta))
                {
                    while( await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        msg = $"正在保存增量日线数据(Tushare)，已经成功保存{response.CompletCount}个，总共{response.TotalCount}个";
                        _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);
                    }
                }

                msg = $"保存增量日线数据(Tushare)已完成，总共保存{datas.Count}个";

                _jobMessageUtil.OnStockDailyTradeDataJobProgressChanged(msg);



            }
            finally
            {
                channel?.Dispose();
            }
        }


        /// <summary>
        /// 从Tushare 获取增量每日指标
        /// </summary>
        /// <param name="date"></param>
        public async Task CollectInStockDailyIndicatorFromTushare(DateTime date)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);

            try
            {
                string msg = $"正在从Tushare获取增量每日指标...";

                _jobMessageUtil.OnStockDailyIndicatorJobProgressChanged(msg);

                var datas = CollectInStockDailyIndicatorFromTushare(date, channel);
                int allCount = datas.Count;

                msg = $"从Tushare成功获取{allCount}个增量每日指标...";
                _jobMessageUtil.OnStockDailyIndicatorJobProgressChanged(msg);

                List<SingleStockDailyIndicator> dailyIndicators = new List<SingleStockDailyIndicator>();
                foreach (var d in datas)
                {
                    dailyIndicators.Add(new SingleStockDailyIndicator { TSCode = d.Key, Entity = d.Value });
                }

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var client = new StockDailyIndicatorApi.StockDailyIndicatorApiClient(channel);

                var request = new BatchSaveStockDailyIndicatorRequest();
                request.Entities.AddRange(dailyIndicators);

                using (var call = client.BatchSaveStockDailyIndicator(request))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        msg = $"正在保存增量每日指标(Tushare)，已经成功保存{response.CompletCount}个，总共{response.TotalCount}个";
                        _jobMessageUtil.OnStockDailyIndicatorJobProgressChanged(msg);
                    }
                }

                msg = $"保存增量每日指标(Tushare)已完成，总共保存{datas.Count}个";

                _jobMessageUtil.OnStockDailyIndicatorJobProgressChanged(msg);

              
            }
            finally
            {
                channel?.Dispose();
            }
        }


        /// <summary>
        /// 同步换手率
        /// </summary>
        public async Task InSyncTurnoverAsync()
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);
            try
            {
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var sclient = new StockTradeDataApi.StockTradeDataApiClient(channel);

                using (var call = sclient.InSyncTurnover(new Empty(), meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;

                        var msg = $"增量同步换手率已经完成{response.CompletCount}个股票，总共{response.TotalCount}个股票"; 

                        _jobMessageUtil.OnInSyncTrunoverJobProgressChanged(msg);
                    }
                }
            }
            finally
            {
                if (channel != null)
                {
                    channel.Dispose();
                }
            }
        }



        /// <summary>
        /// 当天是否开市
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            var date = DateTime.Now.Date;

            bool isOpen = false;
            using (GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl))
            {
                var utcdate = DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day), DateTimeKind.Utc);
                //交易所代码
                var tcClient = new TradingCalendarApi.TradingCalendarApiClient(channel);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var tr = tcClient.MarketIsOpen(new MarketIsOpenRequest
                {
                    Exchange = "SSE",
                    Date = Timestamp.FromDateTime(utcdate)
                }, meta);

                isOpen = tr.IsOpen;
            }

            return isOpen;
        }
        private bool IsOpen(DateTime date, GrpcChannel channel)
        {
            var utcdate = DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day), DateTimeKind.Utc);
            //交易所代码
            var tcClient = new TradingCalendarApi.TradingCalendarApiClient(channel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            var tr = tcClient.MarketIsOpen(new MarketIsOpenRequest
            {
                Exchange = "SSE",
                Date = Timestamp.FromDateTime(utcdate)
            }, meta);

            return tr.IsOpen;
        }



        /// <summary>
        /// 指数增量日线数据
        /// </summary>
        public async Task CollectInTHSIndexDailyTradeDataFromTushareAsync( DateTime date)
        {
            string msg;

            GrpcChannel channel = GrpcChannel.ForAddress(_config.AShareApiUrl);

            try
            {
                msg = $"正在从Tushare获取增量同花顺指数日线数据...";
                _jobMessageUtil.OnTHSIndexDailyTradeDataJobProgressChanged(msg);

                var datas = CollectInTHSIndexDailyTradeDataFromTushare(date, channel);

                int allCount = datas.Count;

                msg = $"从Tushare成功获取{allCount}个增量同花顺指数日线数据...";
                _jobMessageUtil.OnTHSIndexDailyTradeDataJobProgressChanged(msg);

                var client = new THSIndexTradeDataApi.THSIndexTradeDataApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                List<SingleTHSIndexTradeData> tradeDatas = new List<SingleTHSIndexTradeData>();
                foreach (var d in datas)
                {
                    tradeDatas.Add(new SingleTHSIndexTradeData
                    {
                        KCycle = KCycleDto.Day,
                        TSCode = d.Key,
                        Entity = d.Value
                    });
                }

                BatchSaveInTHSIndexTDRequest request = new BatchSaveInTHSIndexTDRequest();
                request.Entities.Add(tradeDatas);

                using (var call = client.BatchSaveInTHSIndexTD(request, meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        msg = $"正在保存增量同花顺指数日线数据(Tushare)，已经成功保存{response.CompletCount}个，总共{response.TotalCount}个";
                        _jobMessageUtil.OnTHSIndexDailyTradeDataJobProgressChanged(msg);
                    }
                }

                msg = $"保存增量同花顺指数日线数据(Tushare)已完成，总共保存{datas.Count}个";

                _jobMessageUtil.OnTHSIndexDailyTradeDataJobProgressChanged(msg);
            }
            finally
            {
                channel?.Dispose();
            }

        }


        private Dictionary<string, THSIndexTradeDataDto> CollectInTHSIndexDailyTradeDataFromTushare(DateTime date, GrpcChannel channel)
        {

            Dictionary<string, THSIndexTradeDataDto> stockTradeDatas = new Dictionary<string, THSIndexTradeDataDto>();

            if (IsOpen(date, channel))
            {
                //读取增量数据
                TuShare tu = new TuShare(_config.TushareUrl, _config.TushareToken);
                THSIndexDailyTradeDataRequestModel requestModel = new THSIndexDailyTradeDataRequestModel();
                requestModel.TradeDate = date.ToString("yyyyMMdd");

                List<string> fields = new List<string>();
                fields.AddRange(new[]
                { "ts_code", "trade_date", "close", "open", "high","low",
                    "pre_close", "avg_price","change","pct_change","vol","turnover_rate"
                    ,"total_mv","float_mv" });

                var task = tu.GetData(requestModel, string.Join(",", fields));

                task.Wait();

                if (task.Result != null && task.Result.Count > 0)
                {
                    foreach (var item in task.Result)
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

                        if (tradeDataDto.Open == 0 || tradeDataDto.Close == 0 || tradeDataDto.High == 0 ||
                           tradeDataDto.Low == 0 || tradeDataDto.Volume == 0 )
                        {
                            continue;
                        }
                        stockTradeDatas.Add(item.TSCode, tradeDataDto);
                    }
                }
            }
            return stockTradeDatas;
        }


    }
}
