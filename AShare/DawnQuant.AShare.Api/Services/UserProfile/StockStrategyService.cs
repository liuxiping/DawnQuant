using AutoMapper;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class StockStrategyService: StockStrategyApi.StockStrategyApiBase
    {
        private readonly ILogger<StockStrategyService> _logger;
        private readonly IStockStrategyRepository _repository;
        private readonly IMapper _mapper;

        public StockStrategyService(ILogger<StockStrategyService> logger, IMapper mapper,
        IStockStrategyRepository repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        public override Task<GetStockStrategiesByCategoryResponse> GetStockStrategiesByCategory(GetStockStrategiesByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetStockStrategiesByCategoryResponse response = new GetStockStrategiesByCategoryResponse();
                var data = _repository.Entities.Where(p => p.CategoryId == request.CategoryId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<StockStrategyDto>>(data));

                return response;
            });
        }

        public override Task<GetStockStrategyByUserResponse> GetStockStrategyByUser(GetStockStrategyByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetStockStrategyByUserResponse response = new GetStockStrategyByUserResponse();

                var data = _repository.Entities.Where(p => p.UserId == request.UserId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<StockStrategyDto>>(data));

                return response;
            });
        }

        public override Task<SaveStockStrategiesResponse> SaveStockStrategies(SaveStockStrategiesRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveStockStrategiesResponse response = new SaveStockStrategiesResponse();
                var s = _mapper.Map<IEnumerable<StockStrategy>>(request.Entities);
                var rs = _repository.Save(s);

                response.Entities.AddRange(_mapper.Map<IEnumerable<StockStrategyDto>>(rs));
                return response;
            });

        }

        public override Task<Empty> DelStockStrategyById(DelStockStrategyByIdRequest request, ServerCallContext context)
        {
           return Task.Run(() =>
            {
                _repository.Delete(request.Id);
                return new Empty();

            });
        }
    }
}
