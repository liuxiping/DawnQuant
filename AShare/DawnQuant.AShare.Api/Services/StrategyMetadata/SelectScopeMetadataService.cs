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
    public class SelectScopeMetadataService: SelectScopeMetadataApi.SelectScopeMetadataApiBase
    {
        ISelectScopeMetadataRepository _stockSelectScopeRepository;
        IMapper _imapper;

        public SelectScopeMetadataService( 
        ISelectScopeMetadataRepository stockSelectScopeRepository,
            IMapper imapper)
        {
            _stockSelectScopeRepository = stockSelectScopeRepository;
            _imapper = imapper;
        }


        public override Task<GetSelectScopeMetadatasResponse> GetSelectScopeMetadatas(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSelectScopeMetadatasResponse response = new GetSelectScopeMetadatasResponse();

                response.Entities.AddRange(_imapper.Map<IEnumerable<SelectScopeMetadataDto>>(_stockSelectScopeRepository.Entities));

                return response;

            });
        }

    }
}
