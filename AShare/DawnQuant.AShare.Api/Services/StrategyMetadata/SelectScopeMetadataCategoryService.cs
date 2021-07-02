using AutoMapper;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.StrategyMetadata
{
    public class SelectScopeMetadataCategoryService: SelectScopeMetadataCategoryApi.SelectScopeMetadataCategoryApiBase
    {

        ISelectScopeMetadataCategoryRepository _categoryRepository;
        IMapper _imapper;
        public SelectScopeMetadataCategoryService(
        ISelectScopeMetadataCategoryRepository categoryRepository,
            IMapper imapper)
        {
            _categoryRepository = categoryRepository;
            _imapper = imapper;
        }
        public override Task<GetSelectScopeMetadataCategoriesIncludeItemsResponse> GetSelectScopeMetadataCategoriesIncludeItems(Empty request, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                GetSelectScopeMetadataCategoriesIncludeItemsResponse response = new GetSelectScopeMetadataCategoriesIncludeItemsResponse();

                var categories = _categoryRepository.Entities.Include(p => p.SelectScopeMetadatas).ToList();
                response.Entities.AddRange(_imapper.Map<IEnumerable<SelectScopeMetadataCategoryDto>>(categories));
                return response;
            });

          
        }

       
    }
}
