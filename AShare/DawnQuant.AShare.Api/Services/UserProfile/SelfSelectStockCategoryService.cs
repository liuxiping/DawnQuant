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
    public class SelfSelectStockCategoryService : SelfSelectStockCategoryApi.SelfSelectStockCategoryApiBase
    {
        private readonly ILogger<SelfSelectStockCategoryService> _logger;
        private readonly ISelfSelectStockCategoryRepository _selfSelStockCategoryRepository;
        private readonly ISelfSelectStockRepository  _selfSelectStockRepository;

        private readonly IMapper _mapper;

        public SelfSelectStockCategoryService(ILogger<SelfSelectStockCategoryService> logger, IMapper mapper,
        ISelfSelectStockCategoryRepository selfSelStockCategoryRepository,
        ISelfSelectStockRepository selfSelectStockRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _selfSelStockCategoryRepository = selfSelStockCategoryRepository;
            _selfSelectStockRepository = selfSelectStockRepository;
        }

        public override Task<GetStockCategoriesByUserResponse> GetStockCategoriesByUser(GetStockCategoriesByUserRequest request, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                GetStockCategoriesByUserResponse response = new GetStockCategoriesByUserResponse();
                var data = _selfSelStockCategoryRepository.Entities.Where(p => p.UserId == request.UserId).Select(p => p);
                response.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockCategoryDto>>(data));

                return response;
            });
           
        }


        public override Task<SaveStockCategoriesResponse> SaveStockCategories(SaveStockCategoriesRequest request, ServerCallContext context)
        {
            SaveStockCategoriesResponse response = new SaveStockCategoriesResponse();

            return Task.Run(() =>
            {
                var data = _mapper.Map<IEnumerable<SelfSelectStockCategory>>(request.Entities);

                var rdata = _selfSelStockCategoryRepository.Save(data);
                response.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockCategoryDto>>(rdata));
                return response;

            });
        }


        public override Task<Empty> DelStockCategory(DelStockCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
               var stocks= _selfSelectStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                //删除分类下的自选股
                _selfSelectStockRepository.Delete(stocks);
                //删除分类
                _selfSelStockCategoryRepository.Delete(request.CategoryId);
                return new Empty();
            });
           
        }
    }
}
