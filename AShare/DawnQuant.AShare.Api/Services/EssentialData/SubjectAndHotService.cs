using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class SubjectAndHotService: SubjectAndHotApi.SubjectAndHotApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly ISubjectAndHotRepository  _subjectAndHotRepository;

        private readonly IMapper _imapper;


        public SubjectAndHotService(ILogger<HolderService> logger,
          ISubjectAndHotRepository subjectAndHotRepository,
           IMapper imapper)
        {
            _logger = logger;
            _subjectAndHotRepository = subjectAndHotRepository;
            _imapper = imapper;
        }


        public override Task<Empty> SaveSubjectAndHots(SaveSubjectAndHotsRequest request, ServerCallContext context)
        {
            return Task.Run(()=>
                {
                if (request.Entities.Count > 0)
                {
                    var datas = _imapper.Map<IEnumerable< SubjectAndHot>>(request.Entities);

                    _subjectAndHotRepository.Save(datas);
                }

                return new Empty();
            });
        }
      
    }
}
