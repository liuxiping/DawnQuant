using AutoMapper;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class IndustryService : IndustryApi.IndustryApiBase
    {
        private readonly ILogger<IndustryService> _logger;
        private readonly IIndustryRepository _industryRepository;
        private readonly IBasicStockInfoRepository _basicStockInfoRepository;

        private readonly IMapper _imapper;


        public IndustryService(ILogger<IndustryService> logger,
           IIndustryRepository industryRepository,
           IBasicStockInfoRepository basicStockInfoRepository,
           IMapper imapper)
        {
            _logger = logger;
            _industryRepository = industryRepository;
            _basicStockInfoRepository = basicStockInfoRepository;
            _imapper = imapper;
        }


        public override Task<SaveIndustryResponse> SaveIndustry(SaveIndustryRequest request, ServerCallContext context)
        {

            return Task.Run(() =>
             {
                 SaveIndustryResponse response = new SaveIndustryResponse();
                 var ins = _industryRepository.Save(_imapper.Map<Industry>(request.Entity));
                 response.Entity = _imapper.Map<IndustryDto>(ins);
                 return response;
             });


        }


        public override Task<ParseIndustryResponse> ParseIndustry(ParseIndustryRequest request, ServerCallContext context)
        {
            ParseIndustryResponse response = new ParseIndustryResponse();

            return Task.Run(() =>
            {
                var industry = _industryRepository.ParseIndustry((request.First, request.Second, request.Three));

                response.Entity = _imapper.Map<IndustryDto>(industry);

                return response;
            });
        }


        /// <summary>
        /// 获取第三级行业
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetThreeLevelIndustriesResponse> GetThreeLevelIndustries(Empty request, ServerCallContext context)
        {

            return Task.Run(() =>
             {
                 GetThreeLevelIndustriesResponse response = new GetThreeLevelIndustriesResponse();

                 var ins = _industryRepository.Entities.Where(p => p.Level == 3);

                 response.Entities.AddRange(_imapper.Map<IEnumerable<IndustryDto>>(ins));

                 return response;
             });
        }

        public override Task<GetAllIndustriesResponse> GetAllIndustries(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetAllIndustriesResponse response = new GetAllIndustriesResponse();

                var ins = _industryRepository.Entities;

                response.Entities.AddRange(_imapper.Map<IEnumerable<IndustryDto>>(ins));

                return response;
            });
        }

        /// <summary>
        /// 删除无效行业
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> ClearInvalidIndustry(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                foreach (var industry in _industryRepository.Entities.Where(p => p.Level == 3).AsNoTracking().ToList())
                {
                    if (!_basicStockInfoRepository.Entities.Where(
                      p => p.IndustryId == industry.Id).Any())
                    {
                        _industryRepository.Delete(industry);
                    }
                }
                return new Empty();
            });
        }
    } 
    
}
