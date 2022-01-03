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
    /// <summary>
    /// 日线数据采集
    /// </summary>
    public class DailyStockTradeDataCollector
    {

        ILogger _logger;

        IPassportProvider _passportProvider;

        public DailyStockTradeDataCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider)
        {

            _passportProvider = passportProvider;
            _logger = logger;
            UnCompleteStocks = new List<string>();

            _tushareToken = config.TushareToken;
            _tushareUrl = config.TushareUrl;

            _apiUrl = config.AShareApiUrl;

            _threadCount = config.DailyStockTradeDataCollectorThreadCount;
        }

        string _tushareToken;
        string _tushareUrl;
        string _apiUrl;
        int _threadCount = 5;

        #region 事件通知
        public event Action<string> DailyStockTradeDataJobProgressChanged;

        public event Action<string> SyncTurnoverJobProgressChanged;

        public event Action<string> DataCleaningJobProgressChanged;


        protected void OnDailyStockTradeDataJobProgressChanged(string msg)
        {
            if (DailyStockTradeDataJobProgressChanged != null)
            {
                DailyStockTradeDataJobProgressChanged(msg);
            }
        }

        protected void OnDataCleaningJobProgressChanged(string msg)
        {
            if (DataCleaningJobProgressChanged != null)
            {
                DataCleaningJobProgressChanged(msg);
            }
        }

        protected void OnSyncTurnoverJobProgressChanged(string msg)
        {
            if (SyncTurnoverJobProgressChanged != null)
            {
                SyncTurnoverJobProgressChanged(msg);
            }
        }

        #endregion

        public List<string> UnCompleteStocks { get; set; }

        /// <summary>
        /// 获取从上市日到今日的所有日线数据，数据清洗，计算复权因子
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        public  void CollectHistoryDailyTradeDataFromTushare()
        {
            using (GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl))
            {
                var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                var response = client.GetAllTSCodes(new Empty(), meta);

                CollectHistoryDailyTradeDataFromTushare(response.TSCodes.ToList(),channel);
            }
               
        }


        /// <summary>
        /// 采集历史日线数据
        /// </summary>
        /// <param name="stocks"></param>
        public  void CollectHistoryDailyTradeDataFromTushare(List<string> stocks,GrpcChannel channel=null)
        {
            string msg = "";
            List<string> completetscodes = new List<string>();
            if (channel == null)
            {
                channel = GrpcChannel.ForAddress(_apiUrl);
            }
            try
            {
                object lockObj = new object();

                int allCount = stocks.Count;
                //分片分别开启多个线程采集
                int length = (int)Math.Ceiling(stocks.Count / (float)_threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(stocks.Skip(i * length).Take(length).ToList());
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
                            CollectSingleStockDailyTradeDataFromTushare(tscode, channel);
                            lock (lockObj)
                            {
                                completetscodes.Add(tscode);
                                complete++;
                            }
                            msg = $"日线历史数据已经成功采集{complete}个股票，总共{allCount}个股票";
                            OnDailyStockTradeDataJobProgressChanged(msg);

                        }
                    });
                    tasks.Add(t);
                    t.Start();

                }

                Task.WaitAll(tasks.ToArray());

                msg = $"日线历史数据已经成功采集{complete}个股票，总共{allCount}个股票。";
                OnDailyStockTradeDataJobProgressChanged(msg);
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


        /// <summary>
        /// 采集单个股票数据
        /// </summary>
        /// <param name="tscode"></param>
        /// <param name="channel"></param>
        private void CollectSingleStockDailyTradeDataFromTushare(string tscode, GrpcChannel channel)
        {
            //每次最多返回5000条数据。必须分批次存取数据

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            GetBasicStockInfoRequest request = new GetBasicStockInfoRequest();
            request.TSCodes.Add(tscode);

            var response = client.GetBasicStockInfo(request, meta);

            if (response == null || response.Entities.Count < 1)
            {
                return;
            }

            //上市日期
            DateTime listingDate = response.Entities[0].ListingDate.ToDateTime();

            //一次取20年数据，获取需要取数据的次数
            int loopCount = (int)Math.Ceiling(
                ((DateTime.Now.Year - listingDate.Year) + 1) / 20.0);

            TuShare tu = new TuShare(_tushareUrl, _tushareToken);
            DailyRequestModel requestModel = new DailyRequestModel();
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

                SaveSingleStockDailyTradeDataFromTushare(task.Result, tscode, channel);
            }

        }

        /// <summary>
        /// 保存单个股票日线交易数据
        /// </summary>
        /// <param name="dailyResponseModels"></param>
        /// <param name="tscode"></param>
        /// <param name="channel"></param>
        private void SaveSingleStockDailyTradeDataFromTushare(List<DailyResponseModel> dailyResponseModels, string tscode, GrpcChannel channel)
        {
            SaveAllStockTradeDataRequest request = new SaveAllStockTradeDataRequest();
            request.TSCode = tscode;
            request.KCycle = KCycleDto.Day;


            foreach (var item in dailyResponseModels)
            {
                StockTradeDataDto tradeDataDto = new StockTradeDataDto();

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";
                tradeDataDto.TradeDateTime = Timestamp.FromDateTime(DateTime.SpecifyKind(
                    DateTime.ParseExact(item.TradeDate, "yyyyMMdd", dtFormat), DateTimeKind.Utc));

                tradeDataDto.Open = item.Open;
                tradeDataDto.Close = item.Close;
                tradeDataDto.High = item.High;
                tradeDataDto.Low = item.Low;
                tradeDataDto.Amount = item.Amount;
                tradeDataDto.Volume = item.Vol;
                tradeDataDto.PreClose = item.PreClose;

                request.Entities.Add(tradeDataDto);
            }

            if (request.Entities.Count > 0)
            {
                var client = new StockTradeDataApi.StockTradeDataApiClient(channel);
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);
                client.SaveAllStockTradeData(request, meta);
            }
        }

        /// <summary>
        /// 计算复权因子
        /// </summary>
        public async Task CalculateAllAdjustFactorAsync()
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);
            try
            {
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var sclient = new StockTradeDataApi.StockTradeDataApiClient(channel);

                using (var call = sclient.CalculateAllAdjustFactor(new CalculateAllAdjustFactorRequest { KCycle = KCycleDto.Day }, meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response = call.ResponseStream.Current;
                        var msg = $"计算复权因子已经完成{response.CompletCount}个股票，总共{response.TotalCount}个股票";
                        OnDailyStockTradeDataJobProgressChanged(msg);
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
        /// 数据清洗
        /// </summary>
        /// <returns></returns>
        public async Task DataCleaningAsync()
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);
            try
            {
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var sclient = new StockTradeDataApi.StockTradeDataApiClient(channel);

                using (var call = sclient.DataCleaning(new Empty(), meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var response=call.ResponseStream.Current;

                        var msg = $"数据清洗已经完成{response.CompletCount}个股票，总共{response.TotalCount}个股票";

                        OnDataCleaningJobProgressChanged(msg);
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
        /// 同步换手率
        /// </summary>
        public async Task SyncTurnoverAsync()
        {
            GrpcChannel channel = GrpcChannel.ForAddress(_apiUrl);
            try
            {
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider.AccessToken);

                var sclient = new StockTradeDataApi.StockTradeDataApiClient(channel);

                using (var call = sclient.SyncTurnover(new Empty() , meta))
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                       var response = call.ResponseStream.Current;
                        var msg = $"同步换手率已经完成{response.CompletCount}个股票，总共{response.TotalCount}个股票";
                        OnDataCleaningJobProgressChanged(msg);
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
    }
}
