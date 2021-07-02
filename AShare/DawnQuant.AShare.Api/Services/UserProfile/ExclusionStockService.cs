using AutoMapper;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class ExclusionStockService: ExclusionStockApi.ExclusionStockApiBase
    {

        private readonly ILogger<SelfSelectStockCategoryService> _logger;
        private readonly IExclusionStockRepository  _exclusionStockRepository;

        private readonly IMapper _mapper;

        public ExclusionStockService(ILogger<SelfSelectStockCategoryService> logger, IMapper mapper,
        IExclusionStockRepository exclusionStockRepository )
        {
            _logger = logger;
            _mapper = mapper;
            _exclusionStockRepository = exclusionStockRepository;
        }

        public override Task<SaveExclusionStocksResponse> SaveExclusionStocks(SaveExclusionStocksRequest request, ServerCallContext context)
        {
            SaveExclusionStocksResponse response = new SaveExclusionStocksResponse();

            return Task.Run(() =>
            {
                var data = _mapper.Map<IEnumerable<ExclusionStock>>(request.Entities);

                var rdata = _exclusionStockRepository.Save(data);
                response.Entities.AddRange(_mapper.Map<IEnumerable<ExclusionStockDto>>(rdata));
                return response;

            });
        }
    }
}
