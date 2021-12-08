using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using AutoMapper;
using DawnQuant.AShare.Entities;
using Google.Protobuf.WellKnownTypes;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Entities.UserProfile;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class BellwetherStockCategoryService : BellwetherStockCategoryApi.BellwetherStockCategoryApiBase
    {
        private readonly ILogger<BellwetherStockCategoryService> _logger;
        private readonly IBellwetherStockCategoryRepository _bellwetherStockCategoryRepository;
        private readonly IBellwetherStockRepository  _bellwetherStockRepository;

        private readonly IMapper _mapper;

        public BellwetherStockCategoryService(ILogger<BellwetherStockCategoryService> logger, IMapper mapper,
        IBellwetherStockCategoryRepository bellwetherStockCategoryRepository,
        IBellwetherStockRepository bellwetherStockRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _bellwetherStockCategoryRepository = bellwetherStockCategoryRepository;
            _bellwetherStockRepository = bellwetherStockRepository;
        }

        public override Task<GetBellwetherStockCategoriesByUserResponse> GetBellwetherStockCategoriesByUser(GetBellwetherStockCategoriesByUserRequest request, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                GetBellwetherStockCategoriesByUserResponse response = new GetBellwetherStockCategoriesByUserResponse();
                var data = _bellwetherStockCategoryRepository.Entities.Where(p => p.UserId == request.UserId).Select(p => p);
                response.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockCategoryDto>>(data));

                return response;
            });
           
        }


        public override Task<SaveBellwetherStockCategoriesResponse> SaveBellwetherStockCategories(SaveBellwetherStockCategoriesRequest request, ServerCallContext context)
        {
            SaveBellwetherStockCategoriesResponse response = new SaveBellwetherStockCategoriesResponse();

            return Task.Run(() =>
            {
                var data = _mapper.Map<IEnumerable<BellwetherStockCategory>>(request.Entities);

                var rdata = _bellwetherStockCategoryRepository.Save(data);
                response.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockCategoryDto>>(rdata));
                return response;

            });
        }


        public override Task<Empty> DelBellwetherStockCategory(DelBellwetherStockCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
               var stocks= _bellwetherStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                //删除分类下的自选股
                _bellwetherStockRepository.Delete(stocks);
                //删除分类
                _bellwetherStockCategoryRepository.Delete(request.CategoryId);
                return new Empty();
            });
           
        }
    }
}
