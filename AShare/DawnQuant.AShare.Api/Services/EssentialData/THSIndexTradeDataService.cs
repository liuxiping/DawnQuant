using AutoMapper;
using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Entities.EssentialData.Common;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class THSIndexTradeDataService : THSIndexTradeDataApi.THSIndexTradeDataApiBase
    {
        private readonly ILogger<THSIndexTradeDataService> _logger;
        private readonly IMapper _imapper;
        private readonly Func<string, KCycle, ITHSIndexTradeDataRepository> _repositoryFunc;
       
        public THSIndexTradeDataService(ILogger<THSIndexTradeDataService> logger,
           IMapper imapper,
           Func<string, KCycle, ITHSIndexTradeDataRepository> repositoryFunc)
        {
            _logger = logger;
            _imapper = imapper;
            _repositoryFunc = repositoryFunc;
        }

        public override Task<Empty> SaveAllTHSIndexTradeDatas(SaveAllTHSIndexTradeDatasRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var kCycle = _imapper.Map<KCycle>(request.KCycle);
                using (var stdr = _repositoryFunc(request.TSCode, kCycle))
                {

                    var datas = _imapper.Map<IEnumerable<THSIndexTradeData>>(request.Entities).ToList();

                    //数据清洗
                    var errdatas = datas.Where(
                                   p => p.Open == 0 || p.Close == 0 ||
                                   p.Low == 0 || p.High == 0 || p.Volume == 0).ToList();
                    if (errdatas != null && errdatas.Count > 0)
                    {
                        foreach (var err in errdatas)
                        {
                            datas.Remove(err);
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


        public override Task<Empty> SaveTHSIndexTradeDatas(SaveTHSIndexTradeDatasRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var kCycle = _imapper.Map<KCycle>(request.KCycle);
                using (var stdr = _repositoryFunc(request.TSCode, kCycle))
                {

                    var datas = _imapper.Map<IEnumerable<THSIndexTradeData>>(request.Entities).ToList();

                    //数据清洗
                    var errdatas = datas.Where(
                                   p => p.Open == 0 || p.Close == 0 ||
                                   p.Low == 0 || p.High == 0 || p.Volume == 0).ToList();
                    if (errdatas != null && errdatas.Count > 0)
                    {
                        foreach (var err in errdatas)
                        {
                            datas.Remove(err);
                        }
                    }
                    //保存交易数据  
                    stdr.Save(datas);

                    return new Empty();
                }
            });
        }


        public override Task BatchSaveInTHSIndexTD(BatchSaveInTHSIndexTDRequest request, IServerStreamWriter<BatchSaveInTHSIndexTDReponse> responseStream, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request.Entities.Any())
                {
                    int complete = 0;
                    int count = request.Entities.Count;
                    foreach (var entity in request.Entities)
                    {
                        var kCycle = _imapper.Map<KCycle>(entity.KCycle);

                        //只支持日线
                        if (kCycle == KCycle.Day)
                        {
                            using (var reposotory = _repositoryFunc(entity.TSCode, kCycle))
                            {
                                var data = _imapper.Map<THSIndexTradeData>(entity.Entity);

                                reposotory.Save(data);
                            }
                        }
                        complete++;

                        //第一个最后一个 每隔20通知
                        if (complete == 1 || complete % 20 == 0 || complete == count)
                        {
                            responseStream.WriteAsync(new  BatchSaveInTHSIndexTDReponse { TotalCount = count, CompletCount = complete });

                        }
                    }

                }
            });
        }

        public override Task<GetLatestTHSIndexTradeDataResponse> GetLatestTHSIndexTradeData(GetLatestTHSIndexTradeDataRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetLatestTHSIndexTradeDataResponse response = new GetLatestTHSIndexTradeDataResponse();

                var datas = GetTHSIndexTradeData(request.TSCode, _imapper.Map<KCycle>(request.KCycle), request.Size);

                response.Entities.AddRange(_imapper.Map<IEnumerable<THSIndexTradeDataDto>>(datas));
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
        private List<THSIndexTradeData> GetTHSIndexTradeData(string tsCode, KCycle kCycle, int totalSzie )
        {
            List<THSIndexTradeData> data = null;
            if (kCycle == KCycle.Day)//日线数据
            {
                data = GetDailyTHSIndexTradeData(tsCode, totalSzie);
            }
            else if (kCycle == KCycle.Week)//周线数据
            {
                data = GetWeekTHSIndexTradeData(tsCode, totalSzie);
            }

            else if (kCycle == KCycle.Month)
            {
                data = GetMonthTHSIndexTradeData(tsCode, totalSzie);
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
        private List<THSIndexTradeData> GetDailyTHSIndexTradeData(string tsCode, int totalSzie)
        {
            List<THSIndexTradeData> datas = null;
            using (ITHSIndexTradeDataRepository dailyRepository = _repositoryFunc(tsCode, KCycle.Day))
            {
                datas = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();
                datas.Reverse();
                return datas;
            }
        }

        /// <summary>
        /// 获取周线数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="totalSzie"></param>
        /// <returns></returns>
        private List<THSIndexTradeData> GetWeekTHSIndexTradeData(string tsCode, int totalSzie)
        {
            totalSzie = totalSzie * 5;

            using (ITHSIndexTradeDataRepository dailyRepository = _repositoryFunc(tsCode, KCycle.Day))
            {
                var tmpData = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();

               

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
        private List<THSIndexTradeData> GetMonthTHSIndexTradeData(string tsCode, int totalSzie)
        {
            totalSzie = totalSzie * 23;
            using (ITHSIndexTradeDataRepository dailyRepository = _repositoryFunc(tsCode, KCycle.Day))
            {
                var tmpData = dailyRepository.Entities.OrderByDescending(p => p.TradeDateTime).Take(totalSzie).ToList();
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
    }
}
