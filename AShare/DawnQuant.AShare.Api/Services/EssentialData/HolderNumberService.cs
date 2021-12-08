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
    public class HolderNumberService: HolderNumberApi.HolderNumberApiBase
    {
        private readonly ILogger<HolderNumberService> _logger;
        private readonly IHolderNumberRepository _holderNumberRepository;
        private readonly IMapper _imapper;


        public HolderNumberService(ILogger<HolderNumberService> logger,
           IHolderNumberRepository holderNumberRepository,
           IMapper imapper)
        {
            _logger = logger;
            _holderNumberRepository = holderNumberRepository;
            _imapper = imapper;
        }

        public override Task<GetHolderNumberResponse> GetHolderNumber(GetHolderNumberRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 GetHolderNumberResponse response = new GetHolderNumberResponse();

                 var data = _holderNumberRepository.Entities.Where(p => p.TSCode == request.TSCode &&
                  p.ReportingPeriod >= _imapper.Map<DateTime>(request.SartDate) &&
                  p.ReportingPeriod <= _imapper.Map<DateTime>(request.EndDate));

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
                     var data = _imapper.Map<IEnumerable<HolderNumber>>(request.Entities);
                    var rdata= _holderNumberRepository.Save(data);

                     response.Entities.AddRange(_imapper.Map<IEnumerable<HolderNumberDto>>(rdata));
                 }

                 return response;
             });
        }
    }
}
