using AutoMapper;

using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.StrategyMetadata
{
    public class FactorMetadataService : FactorMetadataApi.FactorMetadataApiBase
    {
        IFactorMetadataRepository _FactorRepository;
        IMapper _imapper;

        public FactorMetadataService(IFactorMetadataRepository FactorRepository,
         IMapper imapper)
        {
            _FactorRepository = FactorRepository;
            _imapper = imapper;
        }


        public override Task<GetFactorMetadatasResponse> GetFactorMetadatas(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetFactorMetadatasResponse response = new GetFactorMetadatasResponse();
                response.Entities.AddRange(_imapper.Map<IEnumerable<FactorMetadataDto>>(_FactorRepository.Entities));
                return response;

            });
        }
    }
}
