using AutoMapper;
using DawnQuant.AShare.Entities;
using BFSE = DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DawnQuant.AShare.Api.EssentialData
{
    public class CompanyService: CompanyApi.CompanyApiBase
    {
        private readonly ILogger<CompanyService> _logger;
        private readonly  ICompanyRepository _companyRepository;
        private readonly IMapper _imapper;


        public CompanyService(ILogger<CompanyService> logger,
           ICompanyRepository companyRepository,
           IMapper imapper)
        {
            _logger = logger;
            _companyRepository = companyRepository;
            _imapper = imapper;
        }


        public override Task<Empty> SaveCompanys(SaveCompanysRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var datas = _imapper.Map<IEnumerable<BFSE.Company>>(request.Entities);

                 _companyRepository.Save(datas);

                return new Empty();

            });
           
        }

        
    }
}
