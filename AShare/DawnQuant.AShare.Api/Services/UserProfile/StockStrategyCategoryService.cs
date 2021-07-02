using AutoMapper;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class StockStrategyCategoryService: StockStrategyCategoryApi.StockStrategyCategoryApiBase
    {

        private readonly ILogger<StockStrategyCategoryService> _logger;
        private readonly IStockStrategyCategoryRepository  _screpository;
        private readonly IStockStrategyRepository _srepository;
        private readonly IMapper _mapper;

        public StockStrategyCategoryService(ILogger<StockStrategyCategoryService> logger, IMapper mapper,
        IStockStrategyCategoryRepository screpository, IStockStrategyRepository srepository)
        {
            _logger = logger;
            _mapper = mapper;
            _screpository = screpository;
            _srepository = srepository;
        }

        public override Task<GetStrategyCategoriesByUserResponse> GetStrategyCategoriesByUser(GetStrategyCategoriesByUserRequest request, ServerCallContext context)
        {

           return Task.Run(() =>
            {
                GetStrategyCategoriesByUserResponse response = new GetStrategyCategoriesByUserResponse();

                var category = _screpository.Entities.OrderBy(p => p.SortNum).Where(p => p.UserId == request.UserId);
                response.Entities.AddRange(_mapper.Map<IEnumerable<StockStrategyCategoryDto>>(category));
                return response;
            });
           
        }

        public override Task<SaveStrategyCategoriesResponse> SaveStrategyCategories(SaveStrategyCategoriesRequest request, ServerCallContext context)
        {
            SaveStrategyCategoriesResponse response = new SaveStrategyCategoriesResponse();

            return Task.Run(() =>
            {
                var data = _mapper.Map<IEnumerable<StockStrategyCategory>>(request.Entities);

                var rdata = _screpository.Save(data);
                response.Entities.AddRange(_mapper.Map<IEnumerable<StockStrategyCategoryDto>>(rdata));
                return response;

            });
        }

        public override Task<GetCategoriesIncludeStrategiesByUserResponse> GetCategoriesIncludeStrategiesByUser(GetCategoriesIncludeStrategiesByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetCategoriesIncludeStrategiesByUserResponse response = new GetCategoriesIncludeStrategiesByUserResponse();

                var categories = _screpository.Entities.OrderBy(p=>p.SortNum).Include(p=>p.StockStrategies)
                .Where(p => p.UserId == request.UserId);

                foreach (var c in categories)
                {
                    response.Entities.Add(_mapper.Map<StockStrategyCategoryDto>(c));
                }
                return response;
            });

        }

        public override Task<Empty> DelStrategyCategory(DelStrategyCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var s = _srepository.Entities.Where(p => p.CategoryId == request.Id).ToList();
                _srepository.Delete(s);
                _screpository.Delete(request.Id);
                return new Empty();
            });


        }
    }
}
