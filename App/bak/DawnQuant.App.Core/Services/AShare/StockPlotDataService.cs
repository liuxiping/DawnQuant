using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Models.AShare.EssentialData;
using DawnQuant.App.Core.Utils;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.Passport;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.Services.AShare
{
    /// <summary>
    /// K线图
    /// </summary>
    public class StockPlotDataService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;


        public StockPlotDataService(GrpcChannelSet grpcChannelSet,
         IPassportProvider passportProvider, IMapper mapper,
         IMemoryCache memoryCache)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
            _memoryCache= memoryCache;
        }


        /// <summary>
        /// 获取绘图数据，本地有从本地下载，如果没有从服务器下载
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="size"></param>
        /// <param name="adjustedState"></param>
        /// <returns></returns>
        public StockPlotContext GetStockPlotContext(string tsCode, KCycle kCycle, int size, AdjustedState adjustedState)
        {
            StockPlotContext spdata;

            string key = tsCode + "_" + kCycle.ToString() + "_StockPlotData_"+   size.ToString()+"_"+ adjustedState.ToString();

          
            if (!_memoryCache.TryGetValue<StockPlotContext>(key, out spdata))
            {
                spdata = new StockPlotContext();

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
                int realTakeSize = data.Count() >= size ? data.Count() - size : 0;

                spdata.PlotDatas = _mapper.Map<ObservableCollection<StockPlotData>>(data.Skip(realTakeSize).Take(size));

                #region 计算均线
                if (spdata.ShowMA5)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 5).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA5 = ma;
                        i++;
                    }
                }
                if (spdata.ShowMA10)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 10).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA10 = ma;
                        i++;
                    }
                }
                if (spdata.ShowMA20)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 20).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA20 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA30)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 30).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA30 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA60)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 60).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA60 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA120)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 120).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA120 = ma;
                        i++;
                    }
                }

                if (spdata.ShowMA250)
                {
                    int i = 0;
                    foreach (var ma in TechnicalIndicatorUtil.SMA(data, 250).Skip(realTakeSize).Take(size))
                    {
                        spdata.PlotDatas[i].MA250 = ma;
                        i++;
                    }
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
                KCycle= _mapper.Map<KCycleDto>( kCycle), Size=size ,AdjustedState = _mapper.Map <AdjustedStateDto>( adjustedState)
           }, meta);

          return  _mapper.Map<List<StockTradeData>>(r.Entities);
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
            string fileName = StockTradeDataUtil.GetStockTradeDataFileName(tsCode, KCycle.Day);
            if (File.Exists(fileName))
            {
                data = ReadStockTradeData(fileName, totalSzie, adjustedState);

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
            string fileName = StockTradeDataUtil.GetStockTradeDataFileName(tsCode, KCycle.Day);
            if (File.Exists(fileName))
            {

                var temp = ReadStockTradeData(fileName, totalSzie, adjustedState);
                data = StockTradeDataUtil.ToWeekCycle(temp);
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
            string fileName = StockTradeDataUtil.GetStockTradeDataFileName(tsCode, KCycle.Day);

            if (File.Exists(fileName))
            {
                var temp = ReadStockTradeData(fileName, totalSzie, adjustedState);
                data = StockTradeDataUtil.ToMonthCycle(temp);
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
                br.BaseStream.Seek(6*8, SeekOrigin.Begin);
                basePrice = br.ReadDouble();

                //数据够 截取数据
                if (br.BaseStream.Length >= 8 * 9 * totalSzie)
                {

                    //定位数据
                    br.BaseStream.Seek(0, SeekOrigin.End);
                    br.BaseStream.Seek(-8 * 9 * totalSzie, SeekOrigin.End);
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
    }
}
