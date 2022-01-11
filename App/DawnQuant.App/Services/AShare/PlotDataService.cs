using AutoMapper;
using DawnQuant.App.Models;
using DawnQuant.App.Models.AShare;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Utils;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.Passport;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;


namespace DawnQuant.App.Services.AShare
{
    /// <summary>
    /// K线图
    /// </summary>
    public class PlotDataService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _imapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PlotDataService> _logger;


        public PlotDataService()
        {
            _grpcChannelSet = IOCUtil.Container.Resolve< GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>(); 
            _imapper = IOCUtil.Container.Resolve<IMapper>() ;
            _memoryCache = IOCUtil.Container.Resolve<IMemoryCache>();
            _logger=IOCUtil.Container.Resolve<ILogger<PlotDataService>>();
        }

        /// <summary>
        /// 获取绘图数据，本地有从本地下载，如果没有从服务器下载
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="size"></param>
        /// <param name="adjustedState"></param>
        /// <returns></returns>
        public PlotContext GetStockPlotContext(string tsCode, KCycle kCycle, int size, AdjustedState adjustedState)
        {
            PlotContext spdata;

            string key = tsCode + "_" + kCycle.ToString() + "_StockPlotData_"+   size.ToString()+"_"+ adjustedState.ToString();

          
            if (!_memoryCache.TryGetValue<PlotContext>(key, out spdata))
            {
                spdata = new PlotContext();

                spdata.TSCode = tsCode;
                spdata.KCycle = kCycle;

                if (kCycle == KCycle.Day)
                {
                    spdata.ShowMA5 = true;
                    spdata.ShowMA10 = true;
                    spdata.ShowMA20 = true;
                    spdata.ShowMA30 = true;
                    spdata.ShowMA60 = true;
                    spdata.ShowMA120 = true;
                    spdata.ShowMA250 = true;
                }
                else
                {
                    spdata.ShowMA5 = true;
                    spdata.ShowMA10 = true;
                    spdata.ShowMA20 = true;
                    spdata.ShowMA30 = true;
                    spdata.ShowMA60 = true;
                    spdata.ShowMA120 = false;
                    spdata.ShowMA250 = false;
                }

                //读取交易数据
                int extraSize = spdata.GetExtraDataSize();
                int totalSzie = extraSize + size;

                var data = GetStockTradeData(tsCode, kCycle, totalSzie, adjustedState);

                //真实跳过数据
                int realSkipSize = data.Count() >= size ? data.Count() - size : 0;

                spdata.PlotDatas = _imapper.Map<ObservableCollection<PlotData>>(data.Skip(realSkipSize).Take(size));

                #region 计算均线

                var wd= data.OrderBy(p=>p.TradeDateTime).Select(p=>p.Close).ToList();
                if (spdata.ShowMA5)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 5).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA5 = ma;
                        i++;
                    }
                }
                if (spdata.ShowMA10)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 10).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA10 = ma;
                        i++;
                    }
                }
                if (spdata.ShowMA20)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 20).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA20 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA30)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 30).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA30 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA60)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 60).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA60 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA120)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 120).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA120 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA250)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 250).Skip(realSkipSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA250 = ma;
                        i++;
                    }
                }
                #endregion

                #region  macd
                var macd = TechnicalIndicatorUtil.MACD(wd);
                var m=macd.Macd.Skip(realSkipSize).Take(size).ToList();
                var s = macd.MacdSignal.Skip(realSkipSize).Take(size).ToList();
                var h = macd.MacdHist.Skip(realSkipSize).Take(size).ToList();
                for (int i=0; i< spdata.PlotDatas.Count; i++)
                {
                    spdata.PlotDatas[i].MACD = m[i];
                    spdata.PlotDatas[i].MacdSignal = s[i];
                    spdata.PlotDatas[i].MacdHist = h[i];
                }
                #endregion

                //去掉本机内存缓存
                //_memoryCache.Set(key, spdata);
            }

            return spdata;
        }


       /// <summary>
       /// 从服务器读取数据
       /// </summary>
       /// <param name="tsCode"></param>
       /// <param name="kCycle"></param>
       /// <param name="size"></param>
       /// <param name="adjustedState"></param>
       /// <returns></returns>
        private List<StockTradeData> GetDailyStockTradeDataFromServer(string tsCode, KCycle kCycle, int size, AdjustedState adjustedState)
        {
            var client = new StockTradeDataApi.StockTradeDataApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);


           var r= client.GetLatestTradeData(new  GetLatestTradeDataRequest { TSCode = tsCode,
                KCycle= _imapper.Map<KCycleDto>( kCycle), Size=size ,AdjustedState = _imapper.Map <AdjustedStateDto>( adjustedState)
           }, meta);

          return  _imapper.Map<List<StockTradeData>>(r.Entities);
        }

        /// <summary>
        ///返回正序排列指定大小的数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetStockTradeData(string tsCode, KCycle kCycle, int totalSzie, AdjustedState adjustedState)
        {
            List<StockTradeData> data = null;
            if (kCycle == KCycle.Day)//日线数据
            {
                data = GetDailyStockTradeData(tsCode, totalSzie,adjustedState);
            }
            else if (kCycle == KCycle.Week)//周线数据
            {
                data = GetWeekStockTradeData(tsCode, totalSzie,adjustedState);
            }

            else if (kCycle == KCycle.Month)
            {
                data = GetMonthStockTradeData(tsCode, totalSzie,adjustedState);
            }
            else
            {
                throw new NotSupportedException("未提供支持");
            }

            return data;
        }


        /// <summary>
        /// 获取日线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetDailyStockTradeData(string tsCode,int totalSzie, AdjustedState adjustedState)
        {
            List<StockTradeData> data = null;
            string fileName = Helper.GetStockTradeDataFileName(tsCode, KCycle.Day);
            if (File.Exists(fileName))
            {
                try
                {
                    data = ReadStockTradeData(fileName, totalSzie, adjustedState);
                }
                catch (Exception ex)
                {
                    //读取异常 文件出问题 损坏 数据不完整等
                    //直接重服务器读取数据

                    File.Delete(fileName);

                    _logger.LogError($"读取代码{tsCode}交易数据出现异常：\r\n{ex.Message}\r\n{ex.StackTrace}");

                    data = GetDailyStockTradeDataFromServer(tsCode, KCycle.Day, totalSzie, adjustedState);

                }

            }
            else
            {
                data = GetDailyStockTradeDataFromServer(tsCode, KCycle.Day, totalSzie, adjustedState);
            }

          

            return data;
        }


        /// <summary>
        /// 获取周线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetWeekStockTradeData(string tsCode, int totalSzie, AdjustedState adjustedState)
        {
            totalSzie = totalSzie * 5;

            List<StockTradeData> data = null;
            string fileName = Helper.GetStockTradeDataFileName(tsCode, KCycle.Day);
            if (File.Exists(fileName))
            {

                try
                {
                    var temp = ReadStockTradeData(fileName, totalSzie, adjustedState);
                    data = ResampleBasedOnDailyDataUtil.ToWeekCycle(temp);
                }
                catch(Exception ex)
                {
                    //读取异常 文件出问题 损坏 数据不完整等
                    //直接重服务器读取数据

                    data = GetDailyStockTradeDataFromServer(tsCode, KCycle.Week, totalSzie, adjustedState);

                    File.Delete(fileName);

                    _logger.LogError($"读取代码{tsCode}交易数据出现异常：\r\n{ex.Message}\r\n{ex.StackTrace}");
                }
            }
            else
            {
                //从服务器获取数据
                data = GetDailyStockTradeDataFromServer(tsCode, KCycle.Week, totalSzie, adjustedState);

            }
            return data;
        }

        /// <summary>
        /// 获取月线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetMonthStockTradeData(string tsCode, int totalSzie, AdjustedState adjustedState)
        {
            totalSzie = totalSzie * 23;

            List<StockTradeData> data = null;
            string fileName = Helper.GetStockTradeDataFileName(tsCode, KCycle.Day);

            if (File.Exists(fileName))
            {
                try
                {
                    var temp = ReadStockTradeData(fileName, totalSzie, adjustedState);
                    data = ResampleBasedOnDailyDataUtil.ToMonthCycle(temp);
                }
                catch (Exception ex)
                {
                    data = GetDailyStockTradeDataFromServer(tsCode, KCycle.Month, totalSzie, adjustedState);
                    File.Delete(fileName);
                    _logger.LogError($"读取代码{tsCode}交易数据出现异常：\r\n{ex.Message}\r\n{ex.StackTrace}");
                }
            }
            else
            {
                //从服务器获取数据
                data= GetDailyStockTradeDataFromServer(tsCode, KCycle.Month, totalSzie, adjustedState);
            }

            return data;
        }



        /// <summary>
        /// 从文件中获取指定大小的数据
        /// </summary>
        /// <param name="filename"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private List<StockTradeData> ReadStockTradeData(string filename, int totalSzie,AdjustedState adjustedState)
        {
            List<StockTradeData> datas = new List<StockTradeData>();
            DateTime last = DateTime.Now;

            double basePrice = 0;
            using (FileStream fs = new FileStream(filename , FileMode.Open, FileAccess.Read,FileShare.Read))
            {
                BinaryReader br = new BinaryReader(fs);

                //读取最后一日收盘价格
                br.BaseStream.Seek(6*10, SeekOrigin.Begin);
                basePrice = br.ReadDouble();

                //数据够 截取数据
                if (br.BaseStream.Length >= 8 * 11 * totalSzie)
                {

                    //定位数据
                    br.BaseStream.Seek(0, SeekOrigin.End);
                    br.BaseStream.Seek(-8 * 11 * totalSzie, SeekOrigin.End);
                }
                else
                {
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                }

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    StockTradeData d = new StockTradeData();
                    d.Open = br.ReadDouble();
                    d.Close = br.ReadDouble();
                    d.High = br.ReadDouble();
                    d.Low = br.ReadDouble();
                    d.Volume = br.ReadDouble();
                    d.Amount = br.ReadDouble();
                    d.PreClose = br.ReadDouble();
                    d.Turnover = br.ReadDouble();
                    d.TurnoverFree = br.ReadDouble();
                    d.AdjustFactor = br.ReadDouble();
                    d.TradeDateTime = DateTime.FromBinary(br.ReadInt64());
                    datas.Add(d);
                }
            }

            if (adjustedState == AdjustedState.Pre)
            {
                 AdjustCalculatorUtil.CalculatePrePrice(datas);
            }
            else if (adjustedState == AdjustedState.After)
            {
                 AdjustCalculatorUtil.CalculateAfterPrice(datas, basePrice);
            }
            return datas;
        }



        /// <summary>
        /// 同花顺指数绘图数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public PlotContext  GetTHSIndexPlotContext(string tsCode, KCycle kCycle, int size)
        {
            PlotContext spdata;

            string key = tsCode + "_" + kCycle.ToString() + "_PlotData_" + size.ToString();

            if (!_memoryCache.TryGetValue<PlotContext>(key, out spdata))
            {
                spdata = new PlotContext();

                spdata.TSCode = tsCode;
                spdata.KCycle = kCycle;

                if (kCycle == KCycle.Day)
                {
                    spdata.ShowMA5 = true;
                    spdata.ShowMA10 = true;
                    spdata.ShowMA20 = true;
                    spdata.ShowMA30 = true;
                    spdata.ShowMA60 = true;
                    spdata.ShowMA120 = true;
                    spdata.ShowMA250 = true;
                }
                else
                {
                    spdata.ShowMA5 = true;
                    spdata.ShowMA10 = true;
                    spdata.ShowMA20 = true;
                    spdata.ShowMA30 = true;
                    spdata.ShowMA60 = true;
                    spdata.ShowMA120 = false;
                    spdata.ShowMA250 = false;
                }

                //读取交易数据
                int extraSize = spdata.GetExtraDataSize();
                int totalSzie = extraSize + size;

                var data = GetTHSIndexDailyTradeDataFromServer(tsCode, kCycle, totalSzie);

                //真实跳过数据
                int realSkipSize = data.Count() >= size ? data.Count() - size : 0;

                spdata.PlotDatas = _imapper.Map<ObservableCollection<PlotData>>(data.Skip(realSkipSize).Take(size));

                if (data != null && data.Count() > 0)
                {
                    #region 计算均线

                    var wd = data.OrderBy(p => p.TradeDateTime).Select(p => p.Close).ToList();

                    if (spdata.ShowMA5)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 5).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA5 = ma;
                            i++;
                        }
                    }
                    if (spdata.ShowMA10)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 10).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA10 = ma;
                            i++;
                        }
                    }
                    if (spdata.ShowMA20)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 20).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA20 = ma;
                            i++;
                        }
                    }

                    if (spdata.ShowMA30)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 30).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA30 = ma;
                            i++;
                        }
                    }

                    if (spdata.ShowMA60)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 60).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA60 = ma;
                            i++;
                        }
                    }

                    if (spdata.ShowMA120)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 120).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA120 = ma;
                            i++;
                        }
                    }

                    if (spdata.ShowMA250)
                    {
                        int i = 0;
                        foreach (var ma in TechnicalIndicatorUtil.SMA(wd, 250).Skip(realSkipSize).Take(size))
                        {
                            spdata.PlotDatas[i].MA250 = ma;
                            i++;
                        }
                    }
                    #endregion

                    #region  macd
                    var macd = TechnicalIndicatorUtil.MACD(wd);
                    var m = macd.Macd.Skip(realSkipSize).Take(size).ToList();
                    var s = macd.MacdSignal.Skip(realSkipSize).Take(size).ToList();
                    var h = macd.MacdHist.Skip(realSkipSize).Take(size).ToList();
                    for (int i = 0; i < spdata.PlotDatas.Count; i++)
                    {
                        spdata.PlotDatas[i].MACD = m[i];
                        spdata.PlotDatas[i].MacdSignal = s[i];
                        spdata.PlotDatas[i].MacdHist = h[i];
                    }
                    #endregion
                }
                //去掉本机内存缓存
                //_memoryCache.Set(key, spdata);
            }

            return spdata;
        }


        /// <summary>
        /// 从服务器读取同花顺指数交易数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="size"></param>
      
        /// <returns></returns>
        private List<THSIndexTradeData> GetTHSIndexDailyTradeDataFromServer(string tsCode, KCycle kCycle, int size )
        {
            var client = new THSIndexTradeDataApi.THSIndexTradeDataApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            var r = client.GetLatestTHSIndexTradeData(new GetLatestTHSIndexTradeDataRequest
            {
                TSCode = tsCode,
                KCycle = _imapper.Map<KCycleDto>(kCycle),
                Size = size,
            }, meta);
            return _imapper.Map<List<THSIndexTradeData>>(r.Entities);
        }

    }
}
