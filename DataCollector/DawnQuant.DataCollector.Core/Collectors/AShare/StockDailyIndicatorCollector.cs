using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Globalization;
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.MarketData;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class StockDailyIndicatorCollector 
    {
        ILogger _logger;
        IPassportProvider _passportProvider;


        public StockDailyIndicatorCollector(
            ILogger logger,
            CollectorConfig config,
            IPassportProvider passportProvider)
        {
            _logger = logger;
            _passportProvider = passportProvider;
            _tushareToken = config.TushareToken;
            _tushareUrl = config.TushareUrl;
            _apiUrl = config.AShareApiUrl;
            _threadCount = config.StockDailyIndicatorCollectorThreadCount;
            UnCompleteStocks = new List<string>();
        }

        string _tushareToken;
        string _tushareUrl;
        string _apiUrl;
        int _threadCount = 5;

        public event Action ProgressChanged;

        public string StockId { get; set; }
        public string Msg { get; set ; }
        public List<string> UnCompleteStocks { get; set ; }


        /// <summary>
        /// 数据清洗
        /// </summary>
       

        public void CollectHistoryStockDailyIndicator()
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

                var response = client.GetAllTSCodes(new Empty(),meta);


                alltscodes.AddRange(response.TSCodes);

                int allCount = response.TSCodes.Count;

                //分片分别开启多个线程采集
                int length = (int)Math.Ceiling(response.TSCodes.Count / (float)_threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(response.TSCodes.Skip(i * length).Take(length).ToList());
                }
                List<Task> tasks = new List<Task>();

                int complete = 0;
                //开启多个线程采集日线数据
                foreach (var spiltTSCodes in lists)
                {
                    var t = new Task(() =>
                    {
                        foreach (var tscode in spiltTSCodes)
                        {
                            CollectSingleStockDailyIndicator(tscode, channel);
                            lock (lockObj)
                            {
                                complete++;
                                completetscodes.Add(tscode);
                            }
                            Msg = $"每日指标已经成功采集{complete}个股票，总共{allCount}个股票"; 
                            OnProgressChanged();

                        }
                    });

                    tasks.Add(t);
                    t.Start();

                }

                Task.WaitAll(tasks.ToArray());
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

        protected void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged();
            }
        }

        private void CollectSingleStockDailyIndicator(string tscode, GrpcChannel channel)
        {
            var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            GetBasicStockInfoRequest request = new GetBasicStockInfoRequest();
            request.TSCodes.Add(tscode);

            var response = client.GetBasicStockInfo(request, meta);

            if (response == null || response.Entities.Count<1)
            {
                return;
            }

            DateTime listingDate = response.Entities[0].ListingDate.ToDateTime();

            //一次取20年数据
            int loopCount = (int)Math.Ceiling(
                ((DateTime.Now.Year - listingDate.Year) + 1) / 20.0);

            TuShare tu = new TuShare(_tushareUrl, _tushareToken);
            DailyBasicRequestModel requestModel = new DailyBasicRequestModel();
            requestModel.TsCode = tscode;
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

                //提取数据
                var task = tu.GetData(requestModel);
                task.Wait();

                SaveSingleStockDailyIndicator(task.Result,tscode,channel);
            }
        }

        public void SaveSingleStockDailyIndicator(List<DailyBasicResponseModel> dailyResponseModels,string tscode,GrpcChannel channel)
        {
            List<StockDailyIndicatorDto> stockDailyIndicators = new List<StockDailyIndicatorDto>();

            foreach (var item in dailyResponseModels)
            {
                StockDailyIndicatorDto indicator = new StockDailyIndicatorDto();

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";


                indicator.TradeDate =Timestamp.FromDateTime(DateTime.
                    SpecifyKind(DateTime.ParseExact(item.TradeDate, "yyyyMMdd", dtFormat),
                    DateTimeKind.Utc));

                indicator.Close = item.Close;
                indicator.Turnover = item.TurnoverRate;
                indicator.TurnoverFree = item.TurnoverRateF;
                indicator.VolumeRatio = item.VolumeRatio;

                indicator.PE = item.Pe==0?null: item.Pe;
                indicator.PETTM = item.PeTtm == 0 ? null : item.PeTtm;

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

                stockDailyIndicators.Add(indicator);
            }

            if (stockDailyIndicators.Count > 0)
            {
                var client = new StockDailyIndicatorApi.StockDailyIndicatorApiClient(channel);

                SaveStockDailyIndicatorRequest request = new SaveStockDailyIndicatorRequest { TSCode = tscode };
                request.Entities.AddRange(stockDailyIndicators);

                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                client.SaveStockDailyIndicator(request, meta);
            }
        }

        public void CollectHistoryStockDailyIndicator(List<string> stocks)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);
            List<string> completetscodes = new List<string>();
            try
            {
                object lockObj = new object();
                int allCount = stocks.Count;
                int complete = 0;

                //分片分别开启多个线程采集
                int length = (int)Math.Ceiling(stocks.Count / (float)_threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(stocks.Skip(i * length).Take(length).ToList());
                }

                List<Task> tasks = new List<Task>();
                //开启多个线程采集日线数据
                foreach (var spiltTSCodes in lists)
                {
                    var t = new Task(() =>
                    {
                        foreach (var tscode in spiltTSCodes)
                        {
                            CollectSingleStockDailyIndicator(tscode, channel);
                            lock (lockObj)
                            {
                                complete++;
                                completetscodes.Add(tscode);
                            }
                            Msg = $"每日指标已经成功采集{complete}个股票，总共{allCount}个股票";
                            OnProgressChanged();

                        }
                    });

                    tasks.Add(t);
                    t.Start();

                }

                Task.WaitAll(tasks.ToArray());
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
    }
}
