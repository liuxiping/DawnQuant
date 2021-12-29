using AutoMapper;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class HolderService: HolderApi.HolderApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly IHolderNumberRepository _holderNumberRepository;
        private readonly ITop10FloatHolderRepository _top10FloatHolderRepository;

        private readonly IMapper _imapper;


        public HolderService(ILogger<HolderService> logger,
           IHolderNumberRepository holderNumberRepository,
           ITop10FloatHolderRepository top10FloatHolderRepository,
           IMapper imapper)
        {
            _logger = logger;
            _holderNumberRepository = holderNumberRepository;
            _top10FloatHolderRepository = top10FloatHolderRepository;
            _imapper = imapper;
        }

        public override Task<GetHolderNumberResponse> GetHolderNumber(GetHolderNumberRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 GetHolderNumberResponse response = new GetHolderNumberResponse();

                 var data = _holderNumberRepository.Entities.Where(p => p.TSCode == request.TSCode &&
                  p.EndDate >= _imapper.Map<DateTime>(request.SartDate) &&
                  p.EndDate <= _imapper.Map<DateTime>(request.EndDate));

                 if(response!=null && response.Entities.Count>0)
                 {
                     response.Entities.AddRange(_imapper.Map<IEnumerable<HolderNumberDto>>(data));
                 }
                 return response;
             });

        }

        public override Task<SaveHolderNumberResponse> SaveHolderNumber(SaveHolderNumberRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 SaveHolderNumberResponse response = new SaveHolderNumberResponse();

                 if(request.Entities!=null && request.Entities.Count>0)
                 {
                     //先清空老数据

                    var od = _holderNumberRepository.Entities.Where(p => p.TSCode == request.Entities[0].TSCode);
                     if (od != null && od.Count() > 0)
                     {
                         _holderNumberRepository.Delete(od);
                     }

                    var data = _imapper.Map<IEnumerable<HolderNumber>>(request.Entities);

                    var rdata= _holderNumberRepository.Save(data);

                    response.Entities.AddRange(_imapper.Map<IEnumerable<HolderNumberDto>>(rdata));
                 }

                 return response;
             });
        }


        public override Task<GetTop10FloatHolderResponse> GetTop10FloatHolder(GetTop10FloatHolderRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetTop10FloatHolderResponse response = new GetTop10FloatHolderResponse();

                var data = _top10FloatHolderRepository.Entities.Where(p => p.TSCode == request.TSCode &&
                 p.EndDate >= _imapper.Map<DateTime>(request.SartDate) &&
                 p.EndDate <= _imapper.Map<DateTime>(request.EndDate));

                if (response != null && response.Entities.Count > 0)
                {
                    response.Entities.AddRange(_imapper.Map<IEnumerable<Top10FloatHolderDto>>(data));
                }
                return response;
            });
        }

        public override Task<SaveTop10FloatHolderResponse> SaveTop10FloatHolder(SaveTop10FloatHolderRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveTop10FloatHolderResponse response = new SaveTop10FloatHolderResponse();

                if (request.Entities != null && request.Entities.Count > 0)
                {
                    //先清空老数据
                    var od = _top10FloatHolderRepository.Entities.Where(p => p.TSCode == request.Entities[0].TSCode);
                    if (od != null && od.Count() > 0)
                    {
                        _top10FloatHolderRepository.Delete(od);
                    }

                    var data = _imapper.Map<IEnumerable<Top10FloatHolder>>(request.Entities);
                    var rdata = _top10FloatHolderRepository.Save(data);

                    response.Entities.AddRange(_imapper.Map<IEnumerable<Top10FloatHolderDto>>(rdata));
                }

                return response;
            });
        }
    }
}
