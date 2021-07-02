using AutoMapper;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Grpc.Core;
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
        private readonly IMapper _imapper;


        public IndustryService(ILogger<IndustryService> logger,
           IIndustryRepository industryRepository,
           IMapper imapper)
        {
            _logger = logger;
            _industryRepository = industryRepository;
            _imapper = imapper;
        }


        public override Task<SaveIndustryResponse> SaveIndustry(SaveIndustryRequest request, ServerCallContext context)
        {
            SaveIndustryResponse response = new SaveIndustryResponse();

            Task.Run(() =>
            {
                var ins = _industryRepository.Save(_imapper.Map<Industry>(request.Entity));
                response.Entity = _imapper.Map<IndustryDto>(ins);
                return response;
            });

            return base.SaveIndustry(request, context);
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

    } 
    
}
