using AutoMapper;

using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class ReversionFactorService : ReversionFactorApi.ReversionFactorApiBase
    {
        private readonly ILogger<StockTradeDataService> _logger;
        private readonly IMapper _imapper;
        private readonly Func<string, KCycle, IStockTradeDataRepository>  _stdrFunc;
        private readonly Func<string, IReversionFactorRepository> _rfrFunc;

        private readonly IBasicStockInfoRepository _bsinfor;



        public ReversionFactorService(ILogger<StockTradeDataService> logger,
           IMapper imapper, Func<string, IReversionFactorRepository> rfrFunc,
           Func<string, KCycle, IStockTradeDataRepository> stdrFunc,
           IBasicStockInfoRepository bsinfor)
        {
            _logger = logger;
            _imapper = imapper;
            _stdrFunc = stdrFunc;
            _rfrFunc = rfrFunc;
            _bsinfor = bsinfor;
        }

        public override async  Task GetReversionFactor(GetReversionFactorRequest request, IServerStreamWriter<GetReversionFactorResponse> responseStream, ServerCallContext context)
        {

            IReversionFactorRepository  rfr = _rfrFunc(request.TSCode);
             
            int pageSize = 5000;
            int allPage = 1;

            //返回所有数据
            if (request.StartDate == null && request.EndDate == null)
            {
                int allCount = rfr.Entities.Count();

                //每次返回5000条数据
                allPage = (int)Math.Ceiling((double)allCount / 5000);
                for (int i = 0; i < allPage; i++)
                {
                    var datas = rfr.Entities.OrderBy(p => p.TradeDate)
                        .Skip(pageSize * i).Take(pageSize);

                    GetReversionFactorResponse response = new GetReversionFactorResponse();
                    response.Entities.AddRange(_imapper.Map<IEnumerable<ReversionFactorDto>>(datas));
                    await responseStream.WriteAsync(response);
                }
            }
            else if ((request.StartDate == null && request.EndDate != null) ||
                (request.StartDate != null && request.EndDate == null))
            {
                throw new Exception("开始时间和结束时间必须同时设置或者同时不设置");
            }
            else
            {
                DateTime start = request.StartDate.ToDateTime();
                DateTime end = request.EndDate.ToDateTime();

                if (end < start)
                {
                    throw new Exception("结束时间必须大于等于开始时间");
                }
                else
                {
                    int allCount = rfr.Entities
                        .Where(p => p.TradeDate >= start && p.TradeDate <= end).Count();
                    //每次返回5000条数据
                    allPage = (int)Math.Ceiling((double)allCount / 5000);

                    for (int i = 0; i < allPage; i++)
                    {
                        var tradeData = rfr.Entities.Where(p => p.TradeDate >= start && p.TradeDate <= end)
                            .OrderBy(p => p.TradeDate)
                            .Skip(pageSize * i).Take(pageSize);

                        GetReversionFactorResponse response = new GetReversionFactorResponse();
                        response.Entities.AddRange(_imapper.Map<IEnumerable<ReversionFactorDto>>(tradeData));
                        await responseStream.WriteAsync(response);
                    }
                }
            }
        }


        //public override Task<Empty> SaveReversionFactor(SaveReversionFactorRequest request, ServerCallContext context)
        //{
        //    return Task.Run(() =>
        //    {
        //        var rfr = _rfrFunc(request.TSCode);

        //        var datas = _imapper.Map<IEnumerable<ReversionFactor>>(request.Entities);

        //        rfr.Save(datas);

        //        return new Empty();
        //    });
        //}


        public override Task<Empty> CalculateReversionFactor(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                //计算复权因子

                //获取所有股票stockID
               List<string> tsCodes= _bsinfor.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing).Select(p => p.TSCode).ToList();

                foreach (var tscode in tsCodes)
                {
                    var stdr = _stdrFunc(tscode, KCycle.Day);
                    var datas= stdr.Entities.OrderBy(p => p.TradeDateTime).Select(p => new
                    {
                        TradeDateTime = p.TradeDateTime,
                        Close = p.Close,
                        PreClose = p.PreClose
                    }).ToList();
                     AdjustFactorCalculator.Calculate()
                }

                return new Empty();
            });
        }
    }
}
