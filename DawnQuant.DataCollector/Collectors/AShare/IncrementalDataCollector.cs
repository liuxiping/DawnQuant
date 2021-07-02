using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Config;
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
using TuShareHttpSDKLibrary.Model.MarketData;

namespace DawnQuant.DataCollector.Collectors.AShare
{
    public class IncrementalDataCollector
    {

        ILogger _logger;

        IPassportProvider _passportProvider;

        public IncrementalDataCollector(IPassportProvider passportProvider,
            ILogger logger, CollectorConfig config)
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

        //事件通知
        public event Action<string> CollectInDTDProgressChanged;
        public event Action<string> CollectInDIProgressChanged;

        protected void OnCollectInDTDProgressChanged(string msg)
        {
            CollectInDTDProgressChanged?.Invoke(msg);
        }

        protected void OnCollectInDIProgressChanged(string msg)
        {
            CollectInDIProgressChanged?.Invoke(msg);
        }

        private Dictionary<string, StockTradeDataDto> CollectIncrementDailyTradeData(DateTime date, GrpcChannel channel)
        {

            Dictionary<string, StockTradeDataDto> stockTradeDatas = new Dictionary<string, StockTradeDataDto>();

            if (IsOpen(date, channel))
            {
                //读取增量数据
                TuShare tu = new TuShare(_tushareUrl, _tushareToken);
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
                        stockTradeDatas.Add(item.TsCode, tradeData);
                    }
                }
            }
            return stockTradeDatas;
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

        private Dictionary<string, StockDailyIndicatorDto> CollectIncrementStockDailyIndicator(DateTime date, GrpcChannel channel)
        {
            //交易所代码
            Dictionary<string, StockDailyIndicatorDto> stockDailyIndicatorDatas = new Dictionary<string, StockDailyIndicatorDto>();


            if (IsOpen(date, channel))
            {
                //读取增量数据
                TuShare tu = new TuShare(_tushareUrl, _tushareToken);
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

        public void CollectIncrementDailyTradeData(DateTime date)
        {
            string msg;

            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);

            try
            {
                var datas = CollectIncrementDailyTradeData(date, channel);

                int allCount = datas.Count;
                int complete = 0;

                var client = new StockTradeDataApi.StockTradeDataApiClient(channel);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                foreach (var d in datas)
                {
                    var request = new SaveStockTradeDataRequest { TSCode = d.Key, KCycle = KCycleDto.Day };
                    request.Entities.Add(d.Value);

                    client.SaveStockTradeData(request, meta);
                    complete++;
                    msg = $"增量日线数据已经成功采集{complete}个股票，总共{allCount}个股票";
                    OnCollectInDTDProgressChanged(msg);
                }

                msg = $"正在更新复权因子...";
                OnCollectInDTDProgressChanged(msg);
                var sclient = new StockTradeDataApi.StockTradeDataApiClient(channel);
                sclient.CalculateInsAdjustFactor(new CalculateInsAdjustFactorRequest { KCycle = KCycleDto.Day }, meta);
                msg = $"更新复权因子已完成";
                OnCollectInDTDProgressChanged(msg);

            }
            finally
            {
                channel?.Dispose();
            }
        }

        public void CollectIncrementStockDailyIndicator(DateTime date)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);

            try
            {
                var datas = CollectIncrementStockDailyIndicator(date, channel);

                int allCount = datas.Count;
                int complete = 0;

                var client = new StockDailyIndicatorApi.StockDailyIndicatorApiClient(channel);

                foreach (var d in datas)
                {
                    var request = new SaveStockDailyIndicatorRequest { TSCode = d.Key };
                    request.Entities.Add(d.Value);


                    Metadata meta = new Metadata();
                    meta.AddAuthorization(_passportProvider.AccessToken);

                    client.SaveStockDailyIndicator(request, meta);
                    complete++;

                    string msg = $"增量每日指标已经成功采集{complete}个股票，总共{allCount}个股票";
                    OnCollectInDIProgressChanged(msg);
                }
            }
            finally
            {
                channel?.Dispose();
            }
        }
    }
}
