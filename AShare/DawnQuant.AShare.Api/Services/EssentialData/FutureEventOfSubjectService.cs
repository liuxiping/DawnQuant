using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class FutureEventOfSubjectService : FutureEventOfSubjectApi.FutureEventOfSubjectApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly IFutureEventOfSubjectRepository  _sRepository;

        private readonly IMapper _imapper;


        public FutureEventOfSubjectService(ILogger<HolderService> logger,
          IFutureEventOfSubjectRepository futureEventOfSubjectRepository,
           IMapper imapper)
        {
            _logger = logger;
            _sRepository = futureEventOfSubjectRepository;
            _imapper = imapper;
        }


        public override Task<Empty> SaveFutureEventsOfSubject(SaveFutureEventsOfSubjectRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request.Entities.Count > 0)
                {
                    var datas = _imapper.Map<IEnumerable<FutureEventOfSubject>>(request.Entities);

                    _sRepository.Save(datas);
                }

                return new Empty();
            });
        }
    }
}
