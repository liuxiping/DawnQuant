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
    public class FactorMetadataCategoryService: FactorMetadataCategoryApi.FactorMetadataCategoryApiBase
    {
        IFactorMetadataCategoryRepository _categoryRepository;
        IMapper _imapper;
        public FactorMetadataCategoryService(
        IFactorMetadataCategoryRepository categoryRepository,
            IMapper imapper)
        {
            _categoryRepository = categoryRepository;
            _imapper = imapper;
        }

        public override Task<GetFactorMetadataCategoriesIncludeItemsResponse> GetFactorMetadataCategoriesIncludeItems(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetFactorMetadataCategoriesIncludeItemsResponse response = new GetFactorMetadataCategoriesIncludeItemsResponse();

                var categories = _categoryRepository.Entities.OrderBy(p => p.SortNum)
                .Include(p => p.FactorMetadatas.OrderBy(sp => sp.SortNum)).ToList();

                foreach (var c in categories)
                {
                   response.Entities.Add(  _imapper.Map<FactorMetadataCategoryDto>(c));
                }
                return response;
            });
        }
    }
}
