using AutoMapper;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using BFSE = DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DawnQuant.AShare.Repository.Abstract.EssentialData;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class TradingCalendarService: TradingCalendarApi.TradingCalendarApiBase
    {
        private readonly ILogger<TradingCalendarService> _logger;
        private readonly ITradingCalendarRepository _tradingCalendarRepository;
        private readonly IMapper _imapper;


        public TradingCalendarService(ILogger<TradingCalendarService> logger,
           ITradingCalendarRepository tradingCalendarRepository,
           IMapper imapper)
        {
            _logger = logger;
            _tradingCalendarRepository = tradingCalendarRepository;
            _imapper = imapper;
        }



        public override Task<Empty> SaveTradingCalendar(SaveTradingCalendarRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                var datas = _imapper.Map<IEnumerable<TradingCalendar >>(request.Entities);

                 _tradingCalendarRepository.Save(datas);

                return new Empty();

            });
        }


        public override Task<MarketIsOpenResponse> MarketIsOpen(MarketIsOpenRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var re = new MarketIsOpenResponse();

                var dateTime = request.Date.ToDateTime();

                var tradingCalendar = _tradingCalendarRepository.Entities.Where(p => p.Exchange == request.Exchange &&
                    p.Date == dateTime).Select(p => p).SingleOrDefault();

                if (tradingCalendar == null)
                {
                    re.IsOpen = false;
                }
                else
                {
                    re.IsOpen = tradingCalendar.IsOpen;
                }

                return re;
            });
        }
      

        
    }
}
