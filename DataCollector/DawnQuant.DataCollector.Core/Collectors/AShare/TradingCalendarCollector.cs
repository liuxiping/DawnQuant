using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Globalization;
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.BasicData;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class TradingCalendarCollector 
    {
        public TradingCalendarCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider )
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
        ILogger _logger;

        
        IPassportProvider _passportProvider;

        public void CollectCurrentYearTradingCalendar()
        {

        }

        private void SaveTradingCalendar(List<TradeCalResponseModel> result, GrpcChannel channel)
        {

            SaveTradingCalendarRequest request = new SaveTradingCalendarRequest();

            foreach (var item in result)
            {
                TradingCalendarDto tradingCalendar = new TradingCalendarDto();

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";
                tradingCalendar.Date = Timestamp.FromDateTime(
                    DateTime.SpecifyKind(DateTime.ParseExact(item.CalDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));

                tradingCalendar.IsOpen = item.IsOpen == "1" ? true : false;

                if (!string.IsNullOrEmpty(item.PretradeDate))
                {
                    tradingCalendar.PreDate = Timestamp.FromDateTime(
                        DateTime.SpecifyKind(DateTime.ParseExact(item.PretradeDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));
                }
                tradingCalendar.Exchange = item.Exchange?? "";


               request.Entities.Add(tradingCalendar);

            }

            if (request.Entities.Count > 0)
            {
                var client = new TradingCalendarApi.TradingCalendarApiClient(channel);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                client.SaveTradingCalendar(request, meta);

            }
        }

        public void CollectHistoryTradingCalendar()
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl, new GrpcChannelOptions
            {
                MaxReceiveMessageSize = null,
                MaxSendMessageSize = null
            });

            TuShare tu = new TuShare(_tushareUrl, _tushareToken);
            try
            {

                //上交所
                TradeCalRequestModel requestModelSSE = new TradeCalRequestModel();
                requestModelSSE.Exchange = StockEntityConst.SSE;
                var taskSSE = tu.GetData(requestModelSSE);
                taskSSE.Wait();
                SaveTradingCalendar(taskSSE.Result, channel);


                //深交所
                TradeCalRequestModel requestModelSZSE = new TradeCalRequestModel();
                requestModelSZSE.Exchange = StockEntityConst.SZSE;
                var taskSZSE = tu.GetData(requestModelSZSE);
                taskSZSE.Wait();
                SaveTradingCalendar(taskSZSE.Result, channel);


                //CFFEX 中金所
                TradeCalRequestModel requestModelCFFEX = new TradeCalRequestModel();
                requestModelCFFEX.Exchange = StockEntityConst.CFFEX;
                var taskCFFEX = tu.GetData(requestModelCFFEX);
                taskCFFEX.Wait();
                SaveTradingCalendar(taskCFFEX.Result, channel);

                //SHFE 上期所
                TradeCalRequestModel requestModelSHFE = new TradeCalRequestModel();
                requestModelSHFE.Exchange = StockEntityConst.SHFE;
                var taskSHFE = tu.GetData(requestModelSHFE);
                taskSHFE.Wait();
                SaveTradingCalendar(taskSHFE.Result, channel);

                // CZCE 郑商所
                TradeCalRequestModel requestModelCZCE = new TradeCalRequestModel();
                requestModelCZCE.Exchange = StockEntityConst.CZCE;
                var taskCZCE = tu.GetData(requestModelCZCE);
                taskCZCE.Wait();
                SaveTradingCalendar(taskCZCE.Result, channel);


                // DCE 大商所
                TradeCalRequestModel requestModelDCE = new TradeCalRequestModel();
                requestModelDCE.Exchange = StockEntityConst.DCE;
                var taskDCE = tu.GetData(requestModelDCE);
                taskDCE.Wait();
                SaveTradingCalendar(taskDCE.Result, channel);


                //  INE 上能源
                TradeCalRequestModel requestModelINE = new TradeCalRequestModel();
                requestModelINE.Exchange = StockEntityConst.INE;
                var taskINE = tu.GetData(requestModelINE);
                taskINE.Wait();
                SaveTradingCalendar(taskINE.Result, channel);
            }
            finally
            {
                if (channel != null)
                {
                    channel.Dispose();
                    
                }
            }
        }

    }
}
