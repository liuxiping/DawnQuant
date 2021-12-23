using AutoMapper;
using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class StockTradeDataService : StockTradeDataApi.StockTradeDataApiBase
    {
        private readonly ILogger<StockTradeDataService> _logger;
        private readonly IMapper _imapper;
        private readonly Func<string, KCycle, IStockTradeDataRepository>
            _stockTradeDataRepositoryFunc;

        private readonly Func<string, IStockDailyIndicatorRepository>
             _stockDailyIndicatorRepositoryFunc;

        private readonly IBasicStockInfoRepository _basicStockInfoRepository;


        double _threadCount = 5.0;

        public StockTradeDataService(ILogger<StockTradeDataService> logger,
           IMapper imapper, IBasicStockInfoRepository basicStockInfoRepository,
           Func<string, KCycle, IStockTradeDataRepository> stockTradeDataRepositoryFunc,
           Func<string, IStockDailyIndicatorRepository> stockDailyIndicatorRepositoryFunc)
        {
            _logger = logger;
            _imapper = imapper;
            _stockTradeDataRepositoryFunc = stockTradeDataRepositoryFunc;
            _basicStockInfoRepository = basicStockInfoRepository;
            _stockDailyIndicatorRepositoryFunc = stockDailyIndicatorRepositoryFunc;
        }


        public override Task<Empty> SaveStockTradeData(SaveStockTradeDataRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var kCycle = _imapper.Map<KCycle>(request.KCycle);
                using (var stockTradeDataRepository = _stockTradeDataRepositoryFunc(request.TSCode, kCycle))
                {

                    var datas = _imapper.Map<IEnumerable<StockTradeData>>(request.Entities);

                    stockTradeDataRepository.Save(datas);

                    return new Empty();
                }
            });
        }



        public override Task<Empty> SaveAllStockTradeData(SaveAllStockTradeDataRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var kCycle = _imapper.Map<KCycle>(request.KCycle);
                using (var stdr = _stockTradeDataRepositoryFunc(request.TSCode, kCycle))
                {

                    var datas = _imapper.Map<IEnumerable<StockTradeData>>(request.Entities).ToList();

                    //数据清洗
                    var errdatas = datas.Where(
                                   p => p.Open == 0 || p.Close == 0|| 
                                   p.Low==0|| p.High==0 ||p.Volume==0).ToList();
                    if (errdatas != null && errdatas.Count > 0)
                    {
                        foreach (var err in errdatas)
                        {
                            datas.Remove(err);
                        }
                    }

                    //计算复权因子
                    AdjustCalculator.CalculateAllAdjustFactor(datas);

                    //同步换手率
                    using (var dir = _stockDailyIndicatorRepositoryFunc(request.TSCode))
                    {
                        var dis = dir.Entities.Select(p =>
                        new
                        {
                            Turnover = p.Turnover,
                            TurnoverFree = p.TurnoverFree,
                            TradeDate = p.TradeDate
                        }).ToList();

                        if (dis != null && dis.Count > 0)
                        {
                            //更新交易数据
                            for (int i = 0; i < datas.Count; i++)
                            {
                                var d = dis.Where(p => p.TradeDate == datas[i].TradeDateTime).FirstOrDefault();

                                if (d != null)
                                {
                                    datas[i].Turnover = d.Turnover;
                                    datas[i].TurnoverFree = d.TurnoverFree;
                                }
                            }
                           
                        }
                        else
                        {
                            _logger.LogWarning($"代码：{request.TSCode}没有每日指标数据。");

                        }
                    }

                    //清空
                    stdr.Empty();

                    //保存交易数据  
                    stdr.Save(datas);

                    return new Empty();
                }
            });
        }

        /// <summary>
        /// 保存增量数据并计算复权因子
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SaveInSTDAndCAF(SaveInSTDAndCAFRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var kCycle = _imapper.Map<KCycle>(request.KCycle);

                //只支持日线
                if(kCycle !=  KCycle.Day)
                {
                    return null;

                }

                using (var stockTradeDataRepository = _stockTradeDataRepositoryFunc(request.TSCode, kCycle))
                {
                    var data = _imapper.Map<StockTradeData>(request.Entity);

                    //计算复权因子
                    //获取当前数据的前一个交易数据
                    var predata = stockTradeDataRepository.Entities.Where(p=>p.TradeDateTime<data.TradeDateTime)
                    .OrderByDescending(p => p.TradeDateTime).Take(1).FirstOrDefault();
                    if (predata != null)
                    {
                        double gain = (data.Close - data.PreClose) / data.PreClose;
                        data.AdjustFactor = predata.AdjustFactor * (1 + gain);

                    }
                    stockTradeDataRepository.Save(data);

                    return new Empty();
                }
            });
           
        }

        public override async Task GetTradeData(GetTradeDataRequest request, IServerStreamWriter<GetTradeDataResponse> responseStream, ServerCallContext context)
        {
            var kCycle = _imapper.Map<KCycle>(request.KCycle);
            using (var stockTradeDataRepository = _stockTradeDataRepositoryFunc(request.TSCode, kCycle))
            {
                int pageSize = 5000;
                int allPage = 1;

                //返回所有数据
                if (request.StartDateTime == null && request.EndDateTime == null)
                {
                    int allCount = stockTradeDataRepository.Entities.Count();


                    //每次返回5000条数据
                    allPage = (int)Math.Ceiling((double)allCount / 5000);


                    for (int i = 0; i < allPage; i++)
                    {
                        var tradeData = stockTradeDataRepository.Entities.OrderBy(p => p.TradeDateTime)
                            .Skip(pageSize * i).Take(pageSize);

                        GetTradeDataResponse response = new GetTradeDataResponse();
                        response.Entities.AddRange(_imapper.Map<IEnumerable<StockTradeDataDto>>(tradeData));
                        await responseStream.WriteAsync(response);
                    }
                }
                else if ((request.StartDateTime == null && request.EndDateTime != null) ||
                    (request.StartDateTime != null && request.EndDateTime == null))
                {
                    throw new Exception("开始时间和结束时间必须同时设置或者同时不设置");
                }
                else
                {
                    DateTime start = request.StartDateTime.ToDateTime();
                    DateTime end = request.EndDateTime.ToDateTime();

                    if (end < start)
                    {
                        throw new Exception("结束时间必须大于等于开始时间");
                    }
                    else
                    {
                        int allCount = stockTradeDataRepository.Entities
                            .Where(p => p.TradeDateTime >= start && p.TradeDateTime <= end).Count();


                        //每次返回5000条数据

                        allPage = (int)Math.Ceiling((double)allCount / 5000);


                        for (int i = 0; i < allPage; i++)
                        {
                            var tradeData = stockTradeDataRepository.Entities.Where(p => p.TradeDateTime >= start && p.TradeDateTime <= end)
                                .OrderBy(p => p.TradeDateTime)
                                .Skip(pageSize * i).Take(pageSize);

                            GetTradeDataResponse response = new GetTradeDataResponse();
                            response.Entities.AddRange(_imapper.Map<IEnumerable<StockTradeDataDto>>(tradeData));
                            await responseStream.WriteAsync(response);
                        }
                    }
                }
            }
        }

        public override Task<GetLatestTradeDataResponse> GetLatestTradeData(GetLatestTradeDataRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 GetLatestTradeDataResponse response = new GetLatestTradeDataResponse();

                 var datas = GetStockTradeData(request.TSCode, _imapper.Map<KCycle>(request.KCycle), request.Size, request.AdjustedState);

                 response.Entities.AddRange(_imapper.Map<IEnumerable<StockTradeDataDto>>(datas));
                 return response;
             });

        }


        #region 
        /// <summary>
        ///返回正序排列指定大小的数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetStockTradeData(string tsCode, KCycle kCycle, int totalSzie, AdjustedStateDto adjustedState)
        {
            List<StockTradeData> data = null;
            if (kCycle == KCycle.Day)//日线数据
            {
                data = GetDailyStockTradeData(tsCode, totalSzie, adjustedState);
            }
            else if (kCycle == KCycle.Week)//周线数据
            {
                data = GetWeekStockTradeData(tsCode, totalSzie, adjustedState);
            }

            else if (kCycle == KCycle.Month)
            {
                data = GetMonthStockTradeData(tsCode, totalSzie, adjustedState);
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
        private List<StockTradeData> GetDailyStockTradeData(string tsCode, int totalSzie, AdjustedStateDto adjustedState)
        {
            List<StockTradeData> datas = null;
            using (IStockTradeDataRepository dailyRepository = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
            {
                datas = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();
                datas.Reverse();

                if (adjustedState == AdjustedStateDto.Pre)
                {
                    AdjustCalculator.CalculatePrePrice(datas);
                }
                else if (adjustedState == AdjustedStateDto.After)
                {
                    double basePrice = dailyRepository.Entities.OrderBy(p => p.TradeDateTime)
                        .Take(1).Select(p => p.PreClose).First();
                    AdjustCalculator.CalculateAfterPrice(datas, basePrice);
                }

                return datas;
            }
        }

        /// <summary>
        /// 获取周线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetWeekStockTradeData(string tsCode, int totalSzie, AdjustedStateDto adjustedState)
        {
            totalSzie = totalSzie * 5;

            using (IStockTradeDataRepository dailyRepository = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
            {
                var tmpData = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();

                if (adjustedState == AdjustedStateDto.Pre)
                {
                    AdjustCalculator.CalculatePrePrice(tmpData);
                }
                else if (adjustedState == AdjustedStateDto.After)
                {
                    double basePrice = dailyRepository.Entities.OrderBy(p => p.TradeDateTime)
                        .Take(1).Select(p => p.PreClose).First();
                    AdjustCalculator.CalculateAfterPrice(tmpData, basePrice);
                }

                //日线转周线
                return ResampleBasedOnDailyData.ToWeekCycle(tmpData);
            }

        }


        /// <summary>
        /// 获取月线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<StockTradeData> GetMonthStockTradeData(string tsCode, int totalSzie, AdjustedStateDto adjustedState)
        {
            totalSzie = totalSzie * 23;
            using (IStockTradeDataRepository dailyRepository = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
            {
                var tmpData = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();

                if (adjustedState == AdjustedStateDto.Pre)
                {
                    AdjustCalculator.CalculatePrePrice(tmpData);
                }
                else if (adjustedState == AdjustedStateDto.After)
                {
                    double basePrice = dailyRepository.Entities.OrderBy(p => p.TradeDateTime)
                        .Take(1).Select(p => p.PreClose).First();
                    AdjustCalculator.CalculateAfterPrice(tmpData, basePrice);
                }

                //日线转月线
                return ResampleBasedOnDailyData.ToMonthCycle(tmpData);
            }

        }

        /// <summary>
        /// 获取一年中第几周
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetWeekNumOfTheYear(DateTime date)
        {
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            Calendar myCal = cultureInfo.Calendar;
            CalendarWeekRule calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            int weekOfYear = myCal.GetWeekOfYear(date, calendarWeekRule, dayOfWeek);
            return weekOfYear;
        }

        #endregion

        /// <summary>
        /// 全量计算复权因子
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override Task CalculateAllAdjustFactor(CalculateAllAdjustFactorRequest request, IServerStreamWriter<CalculateAllAdjustFactorResponse> responseStream, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                if (request.KCycle == KCycleDto.Day)
                {
                    CalculateAllAdjustFactorOnDailyData(responseStream);
                }
                else
                {
                    throw new NotSupportedException("只支持日线数据");
                }

            });


        }




        private void CalculateAllAdjustFactorOnDailyData(IServerStreamWriter<CalculateAllAdjustFactorResponse> responseStream)
        {

            List<string> tsCodes = _basicStockInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing
            || p.ListedStatus == StockEssentialDataConst.PauseListing).Select(p => p.TSCode).ToList();


            object lockObj = new object();

            int allCount = tsCodes.Count;
            int complete = 0;

            //分片分别开启多个线程更新数据
            int length = (int)Math.Ceiling(tsCodes.Count / _threadCount);

            List<List<string>> lists = new List<List<string>>();

            for (int i = 0; i < _threadCount; i++)
            {
                lists.Add(tsCodes.Skip(i * length).Take(length).ToList());
            }

            List<Task> tasks = new List<Task>();

            //开启多个线程计算复权因子
            foreach (var spiltTSCodes in lists)
            {
                var task = Task.Run(() =>
                 {
                    //计算日线复权因子
                    foreach (var tsCode in spiltTSCodes)
                     {
                         using (var tdr = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
                         {
                             var datas = tdr.Entities.OrderBy(p => p.TradeDateTime).ToList();
                             AdjustCalculator.CalculateAllAdjustFactor(datas);
                             tdr.Save(datas);

                             lock (lockObj)
                             {
                                 complete++;
                             }

                             CalculateAllAdjustFactorResponse response = new CalculateAllAdjustFactorResponse();
                             response.Message = $"全量计算复权因子已经成功完成{complete}个股票，总共{allCount}个股票";
                             responseStream.WriteAsync(response);

                         }
                     }

                 });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private void CalculateAllAdjustFactorOn5MData()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 增量更新复权因子
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override Task CalculateInsAdjustFactor(CalculateInsAdjustFactorRequest request, IServerStreamWriter<CalculateInsAdjustFactorResponse> responseStream, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request.KCycle == KCycleDto.Day)
                {
                    CalculateInsAdjustFactorOnDailyData(responseStream);
                }

                else
                {
                    throw new NotSupportedException("只支持日线数据");
                }
            });
        }


        /// <summary>
        /// 增量更新日线复权因子
        /// </summary>
        private void CalculateInsAdjustFactorOnDailyData(IServerStreamWriter<CalculateInsAdjustFactorResponse> responseStream)
        {
            List<string> tsCodes = _basicStockInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing ||
            p.ListedStatus == StockEssentialDataConst.PauseListing).Select(p => p.TSCode).ToList();

            object lockObj = new object();

            int allCount = tsCodes.Count;
            int complete = 0;
            //分片分别开启多个线程更新数据
            int length = (int)Math.Ceiling(tsCodes.Count / _threadCount);

            List<List<string>> lists = new List<List<string>>();

            for (int i = 0; i < _threadCount; i++)
            {
                lists.Add(tsCodes.Skip(i * length).Take(length).ToList());
            }
            List<Task> tasks = new List<Task>();

            //开启多个线程计算复权因子
            foreach (var spiltTSCodes in lists)
            {
                var task = Task.Run(() =>
                {
                    //计算日线复权因子
                    foreach (var tsCode in spiltTSCodes)
                    {
                        using (var tdr = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
                        {
                            var datas = tdr.Entities.OrderByDescending(p => p.TradeDateTime).Take(2).ToList();
                            datas.Reverse();
                            if (datas.Count == 2)
                            {
                                double gain = (datas[1].Close - datas[1].PreClose) / datas[1].PreClose;
                                datas[1].AdjustFactor = datas[0].AdjustFactor * (1 + gain);
                                tdr.Save(datas[1]);

                                lock (lockObj)
                                {
                                    complete++;
                                }

                                CalculateInsAdjustFactorResponse response = new CalculateInsAdjustFactorResponse();
                                response.Message = $"增量计算复权因子已经成功完成{complete}个股票，总共{allCount}个股票";
                                responseStream.WriteAsync(response);
                            }
                        }
                    }

                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }


        /// <summary>
        /// 同步换手率
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task SyncTurnover(Empty request, IServerStreamWriter<SyncTurnoverResponse> responseStream, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                List<string> tsCodes = _basicStockInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing
              || p.ListedStatus == StockEssentialDataConst.PauseListing).Select(p => p.TSCode).ToList();


                object lockObj = new object();

                int allCount = tsCodes.Count;
                int complete = 0;

                //分片分别开启多个线程更新数据
                int length = (int)Math.Ceiling(tsCodes.Count / _threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(tsCodes.Skip(i * length).Take(length).ToList());
                }

                List<Task> tasks = new List<Task>();

                //开启多个线程计算复权因子
                foreach (var spiltTSCodes in lists)
                {
                    var task = Task.Run(() =>
                    {
                        //开始更新换手率数据
                        foreach (var tsCode in spiltTSCodes)
                        {
                            using (var tdr = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
                            {
                                //交易数据
                                var datas = tdr.Entities.OrderBy(p => p.TradeDateTime).ToList();

                                if (datas != null && datas.Count > 0)
                                {

                                    //每日指标数据
                                    using (var dir = _stockDailyIndicatorRepositoryFunc(tsCode))
                                    {
                                        var dis = dir.Entities.Select(p =>
                                        new
                                        {
                                            Turnover = p.Turnover,
                                            TurnoverFree = p.TurnoverFree,
                                            TradeDate = p.TradeDate
                                        }).ToList();

                                        if (dis != null && dis.Count > 0)
                                        {
                                            //更新交易数据
                                            for (int i = 0; i < datas.Count; i++)
                                            {
                                                var d = dis.Where(p => p.TradeDate == datas[i].TradeDateTime).FirstOrDefault();

                                                if (d != null)
                                                {
                                                    datas[i].Turnover = d.Turnover;
                                                    datas[i].TurnoverFree = d.TurnoverFree;
                                                }
                                            }
                                            tdr.Save(datas);
                                        }
                                        else
                                        {
                                            _logger.LogDebug($"代码：{tsCode}没有每日指标数据。");

                                        }
                                    }
                                    lock (lockObj)
                                    {
                                        complete++;
                                    }

                                    SyncTurnoverResponse response = new SyncTurnoverResponse();
                                    response.Message = $"同步换手率已经成功完成{complete}个股票，总共{allCount}个股票";
                                    responseStream.WriteAsync(response);
                                }
                                else
                                {
                                    _logger.LogDebug($"代码：{tsCode}没有交易数据。");
                                }

                            }
                        }

                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            });

        }


        public override Task InSyncTurnover(Empty request, IServerStreamWriter<InSyncTurnoverResponse> responseStream, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                List<string> tsCodes = _basicStockInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing
              || p.ListedStatus == StockEssentialDataConst.PauseListing).Select(p => p.TSCode).ToList();


                object lockObj = new object();

                int allCount = tsCodes.Count;
                int complete = 0;

                //分片分别开启多个线程更新数据
                int length = (int)Math.Ceiling(tsCodes.Count / _threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(tsCodes.Skip(i * length).Take(length).ToList());
                }

                List<Task> tasks = new List<Task>();

                //开启多个线程计算复权因子
                foreach (var spiltTSCodes in lists)
                {
                    var task = Task.Run(() =>
                    {
                        //开始更新换手率数据
                        foreach (var tsCode in spiltTSCodes)
                        {
                            using (var tdr = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
                            {
                                //取最新交易数据
                                var data = tdr.Entities.OrderByDescending(p => p.TradeDateTime).Take(1).ToList();

                                if (data != null && data.Count > 0)
                                {
                                    //每日指标数据
                                    using (var dir = _stockDailyIndicatorRepositoryFunc(tsCode))
                                    {
                                        bool needUpdate = false;
                                        //更新交易数据
                                        for (int i = 0; i < data.Count; i++)
                                        {
                                            var di = dir.Entities
                                            .Where(p => p.TradeDate == data[i].TradeDateTime)
                                            .Select(p => new
                                            {
                                                Turnover = p.Turnover,
                                                TurnoverFree = p.TurnoverFree,
                                                TradeDate = p.TradeDate
                                            }).FirstOrDefault();


                                            if (di != null)
                                            {
                                                data[i].Turnover = di.Turnover;
                                                data[i].TurnoverFree = di.TurnoverFree;
                                                needUpdate = true;
                                            }
                                        }

                                        if (needUpdate)
                                        {
                                            tdr.Save(data);
                                        }

                                    }


                                }
                                else
                                {
                                    _logger.LogDebug($"代码：{tsCode}没有交易数据。");
                                }

                                lock (lockObj)
                                {
                                    complete++;
                                }

                                InSyncTurnoverResponse response = new InSyncTurnoverResponse();
                                response.Message = $"增量同步换手率已经成功完成{complete}个股票，总共{allCount}个股票";
                                responseStream.WriteAsync(response);

                            }
                        }

                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            });
        }


        /// <summary>
        /// 清洗数据  删除停牌日无效数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task DataCleaning(Empty request, IServerStreamWriter<DataCleaningResponse> responseStream, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                List<string> tsCodes = _basicStockInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing
              || p.ListedStatus == StockEssentialDataConst.PauseListing).Select(p => p.TSCode).ToList();


                object lockObj = new object();

                int allCount = tsCodes.Count;
                int complete = 0;

                //分片分别开启多个线程更新数据
                int length = (int)Math.Ceiling(tsCodes.Count / _threadCount);

                List<List<string>> lists = new List<List<string>>();

                for (int i = 0; i < _threadCount; i++)
                {
                    lists.Add(tsCodes.Skip(i * length).Take(length).ToList());
                }

                List<Task> tasks = new List<Task>();

                //开启多个线程清洗数据
                foreach (var spiltTSCodes in lists)
                {
                    var task = Task.Run(() =>
                    {
                        //开始更新换手率数据
                        foreach (var tsCode in spiltTSCodes)
                        {
                            using (var tdr = _stockTradeDataRepositoryFunc(tsCode, KCycle.Day))
                            {
                                //清洗停牌数据
                                var datas = tdr.Entities.Where(
                                    p => p.Open == 0 || p.Close == 0 ||
                                   p.Low == 0 || p.High == 0||p.Volume==0 ).ToList();

                                if (datas != null && datas.Count > 0)
                                {

                                    tdr.Delete(datas);
                                }

                                lock (lockObj)
                                {
                                    complete++;
                                }

                                DataCleaningResponse response = new DataCleaningResponse();
                                response.Message = $"清洗数据已经成功完成{complete}个股票，总共{allCount}个股票";
                                responseStream.WriteAsync(response);
                            }
                        }

                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            });
        }


       
    }
}
