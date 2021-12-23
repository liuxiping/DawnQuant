using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.Passport;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System.Globalization;

using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.Base;

namespace DawnQuant.DataCollector.Core.Collectors.AShare
{
    public class BasicStockInfoCollector 
    {

        /// <summary>
        /// 股票基本信息采集
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="passportProvider"></param>
        public BasicStockInfoCollector(ILogger logger, CollectorConfig config,
            IPassportProvider passportProvider)
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

        public void CollectAllBasicStockInfoFromTushare()
        {
            try
            {
                //传入接口地址及Token
                TuShare tu = new TuShare(_tushareUrl, _tushareToken);

                //实例化股票列表 接口：stock_basic 的Model
                StoreBasicRequestModel modelSSEL = new StoreBasicRequestModel();
                //设置交易所 SSE上交所 SZSE深交所 HKEX港交所(未上线)
                modelSSEL.Exchange = StockEntityConst.SSE;
                //设置上市状态： L上市 D退市 P暂停上市，默认L
                modelSSEL.ListStatus = StockEntityConst.Listing;

                var taskSSEL = tu.GetData(modelSSEL);
                taskSSEL.Wait();
                SaveBasicStockInfo(taskSSEL.Result);

                StoreBasicRequestModel modelSSED = new StoreBasicRequestModel();
                modelSSED.Exchange = StockEntityConst.SSE;
                modelSSED.ListStatus = StockEntityConst.DeListing;
                var taskSSED = tu.GetData(modelSSED);
                taskSSED.Wait();
                SaveBasicStockInfo(taskSSED.Result);

                StoreBasicRequestModel modelSSEP = new StoreBasicRequestModel();
                modelSSEP.Exchange = StockEntityConst.SSE;
                modelSSEP.ListStatus = StockEntityConst.PauseListing;
                var taskSSEP = tu.GetData(modelSSEP);
                taskSSEP.Wait();
                SaveBasicStockInfo(taskSSEP.Result);

                //深圳证券交易所

                StoreBasicRequestModel modelSZSEL = new StoreBasicRequestModel();
                modelSZSEL.Exchange = StockEntityConst.SZSE;
                modelSZSEL.ListStatus = StockEntityConst.Listing;
                var taskSZSEL = tu.GetData(modelSZSEL);
                taskSZSEL.Wait();
                SaveBasicStockInfo(taskSZSEL.Result);

                StoreBasicRequestModel modelSZSEP = new StoreBasicRequestModel();
                modelSZSEP.Exchange = StockEntityConst.SZSE;
                modelSZSEP.ListStatus = StockEntityConst.PauseListing;
                var taskSZSEP = tu.GetData(modelSZSEP);
                taskSZSEP.Wait();
                SaveBasicStockInfo(taskSZSEP.Result);

                StoreBasicRequestModel modelSZSED = new StoreBasicRequestModel();
                modelSZSED.Exchange = StockEntityConst.SZSE;
                modelSZSED.ListStatus = StockEntityConst.DeListing;
                var taskSZSED = tu.GetData(modelSZSED);
                taskSZSED.Wait();
                SaveBasicStockInfo(taskSZSED.Result);
            }
            catch (Exception ex)
            {
                string msg = "采集股票基本信息发生错误：\r\n" + ex.Message+"\r\n"+ex.StackTrace;
                _logger.LogError(msg);
                throw;
            }
        }

        /// <summary>
        /// 保存股市基本信息
        /// </summary>
        /// <param name="result"></param>
        private void SaveBasicStockInfo(List<StoreBasicResponseModel> result)
        {

            SaveBasicStockInfoRequest request = new SaveBasicStockInfoRequest();

            foreach (var item in result)
            {
                BasicStockInfoDto stockInfoDto = new BasicStockInfoDto();

                stockInfoDto.TSCode = item.TsCode.ToUpper();
                stockInfoDto.StockCode = item.Symbol ?? "";
                stockInfoDto.StockName = item.Name ?? "";
                stockInfoDto.FullName = item.FullName ?? "";
                stockInfoDto.EnFullName = item.EnName ?? "";

                stockInfoDto.Area = item.Area ?? "";
                stockInfoDto.PrimaryIndustry = item.Industry ?? "";
                stockInfoDto.MarketType = item.Market ?? "";
                stockInfoDto.Exchange = item.Exchange ?? "";
                stockInfoDto.Currency = item.CurrType ?? "";

                stockInfoDto.ListedStatus = item.ListStatus ?? "";
                stockInfoDto.StockConnect = item.IsHS ?? "";


                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";
                var listDate = DateTime.ParseExact(item.ListDate, "yyyyMMdd", dtFormat);
                stockInfoDto.ListingDate = Timestamp.FromDateTime(DateTime.SpecifyKind(listDate, DateTimeKind.Utc));


                if (stockInfoDto.ListedStatus == StockEntityConst.DeListing && 
                    !string.IsNullOrEmpty(item.DelistDate))
                {
                    var delistingDate = DateTime.ParseExact(item.DelistDate, "yyyyMMdd", dtFormat);
                    stockInfoDto.DelistingDate = Timestamp.FromDateTime(DateTime.SpecifyKind(delistingDate, DateTimeKind.Utc)) ;
                }
                request.Entities.Add(stockInfoDto);
            }

            if (request.Entities.Count > 0)
            {
                GrpcChannel channel = null;

                try
                {
                    channel = GrpcChannel.ForAddress(_apiUrl);

                    var client = new BasicStockInfoApi.BasicStockInfoApiClient(channel);

                    Metadata meta = new Metadata();
                    meta.AddAuthorization(_passportProvider.AccessToken);
                    client.SaveBasicStockInfo(request,
                       meta);
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
}
