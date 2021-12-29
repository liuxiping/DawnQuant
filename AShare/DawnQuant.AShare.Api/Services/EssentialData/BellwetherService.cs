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
    public class BellwetherService:BellwetherApi.BellwetherApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly IBellwetherRepository _bellwetherRepository;

        private readonly IMapper _imapper;


        public BellwetherService(ILogger<HolderService> logger,
          IBellwetherRepository bellwetherRepository,
           IMapper imapper)
        {
            _logger = logger;
            _bellwetherRepository = bellwetherRepository;
            _imapper = imapper;
        }


        public override Task<Empty> SaveBellwethers(SaveBellwethersRequest request, ServerCallContext context)
        {

          return  Task.Run(() => 
          {
              if (request.Entities.Count > 0)
              {
                  
                  var datas = _imapper.Map<IEnumerable<Bellwether>>(request.Entities);

                  _bellwetherRepository.Save(datas);
              }

              return new Empty();
          });
            
        }
    }
}
