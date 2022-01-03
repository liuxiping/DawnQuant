using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using DawnQuant.AShare.Entities;
using BFSE = DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class StockDailyIndicatorService : StockDailyIndicatorApi.StockDailyIndicatorApiBase
    {

        private readonly ILogger<StockDailyIndicatorService> _logger;
        private readonly IMapper _imapper;
        private readonly Func<string, IStockDailyIndicatorRepository>
            _stockDailyIndicatorRepositoryFunc;

        public StockDailyIndicatorService(ILogger<StockDailyIndicatorService> logger,
           IMapper imapper,
           Func<string, IStockDailyIndicatorRepository> stockDailyIndicatorRepositoryFunc)
        {
            _logger = logger;
            _imapper = imapper;
            _stockDailyIndicatorRepositoryFunc = stockDailyIndicatorRepositoryFunc;
        }

        public override Task<Empty> SaveStockDailyIndicator(SaveStockDailyIndicatorRequest request, ServerCallContext context)
        {

            return Task.Run(() =>
             {

                 using (var stockDailyIndicatorRepository = _stockDailyIndicatorRepositoryFunc(request.TSCode))
                 {

                     var datas = _imapper.Map<IEnumerable<BFSE.StockDailyIndicator>>(request.Entities);

                     stockDailyIndicatorRepository.Save(datas);
                 }

                 return new Empty();
             });
        }

        public override Task BatchSaveStockDailyIndicator(BatchSaveStockDailyIndicatorRequest request, IServerStreamWriter<BatchSaveStockDailyIndicatorResponse> responseStream, ServerCallContext context)
        {
            return Task.Run(() => 
            {
                if (request.Entities != null && request.Entities.Count > 0)
                {
                    int complete = 0;
                    int count = request.Entities.Count;
                    foreach (var entity in request.Entities)
                    {
                        using (var stockDailyIndicatorRepository = _stockDailyIndicatorRepositoryFunc(entity.TSCode))
                        {

                            var data = _imapper.Map<BFSE.StockDailyIndicator>(entity.Entity);

                            stockDailyIndicatorRepository.Save(data);
                        }

                        complete++;

                        //第一个最后一个 每隔20通知
                        if (complete == 1 || complete % 20 == 0 || complete == count)
                        {
                            responseStream.WriteAsync(new  BatchSaveStockDailyIndicatorResponse { TotalCount = count, CompletCount = complete });

                        }
                    }
                }
                
            });
        }

        public override async Task GetStockDailyIndicator(GetStockDailyIndicatorRequest request, IServerStreamWriter<GetStockDailyIndicatorResponse> responseStream, ServerCallContext context)
        {

            using (var stockDailyIndicatorRepository = _stockDailyIndicatorRepositoryFunc(request.TSCode))
            {
                int pageSize = 5000;
                int allPage = 1;

                //返回所有数据
                if (request.StartDateTime == null && request.EndDateTime == null)
                {
                    int allCount = stockDailyIndicatorRepository.Entities.Count();
                    //每次返回5000条数据
                    allPage = (int)Math.Ceiling((double)allCount / 5000);

                    for (int i = 0; i < allPage; i++)
                    {
                        var data = stockDailyIndicatorRepository.Entities.OrderBy(p => p.TradeDate)
                            .Skip(pageSize * i).Take(pageSize);

                        GetStockDailyIndicatorResponse response = new GetStockDailyIndicatorResponse();
                        response.Entities.AddRange(_imapper.Map<IEnumerable<StockDailyIndicatorDto>>(data));
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
                        int allCount = stockDailyIndicatorRepository.Entities
                            .Where(p => p.TradeDate >= start && p.TradeDate <= end).Count();


                        //每次返回5000条数据

                        allPage = (int)Math.Ceiling((double)allCount / 5000);


                        for (int i = 0; i < allPage; i++)
                        {
                            var data = stockDailyIndicatorRepository.Entities.Where(p => p.TradeDate >= start && p.TradeDate <= end)
                                .OrderBy(p => p.TradeDate)
                                .Skip(pageSize * i).Take(pageSize);

                            GetStockDailyIndicatorResponse response = new GetStockDailyIndicatorResponse();
                            response.Entities.AddRange(_imapper.Map<IEnumerable<StockDailyIndicatorDto>>(data));
                            await responseStream.WriteAsync(response);
                        }
                    }
                }
            }

        }
    }
}
