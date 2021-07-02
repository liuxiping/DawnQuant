using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class BasicStockInfoService : BasicStockInfoApi.BasicStockInfoApiBase
    {
        private readonly ILogger<BasicStockInfoService> _logger;
        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IIndustryRepository  _industryRepository;

        private readonly IMapper _imapper;

        public BasicStockInfoService(ILogger<BasicStockInfoService> logger,
            IBasicStockInfoRepository basicStockInfoRepository,
            IIndustryRepository industryRepository,
            IMapper imapper)
        {
            _logger = logger;
            _basicStockInfoRepository = basicStockInfoRepository;
            _industryRepository = industryRepository;
            _imapper = imapper;
        }

        public override Task<GetAllTSCodesResponse> GetAllTSCodes(Empty request, ServerCallContext context)
        {

            GetAllTSCodesResponse response = new GetAllTSCodesResponse();

            return Task.Run(() =>
             {
                 response.TSCodes.AddRange(_basicStockInfoRepository.Entities.Select(p => p.TSCode));
                 return response;
             });

        }

        public override Task<Empty> SaveBasicStockInfo(SaveBasicStockInfoRequest request, ServerCallContext context)
        {
            var datas = _imapper.Map<IEnumerable<BasicStockInfo>>(request.Entities);

            return  Task.Run(() =>
            {
                _basicStockInfoRepository.Save(datas);
                return new Empty();
            });

        }

      

        public override Task<Empty> UpdateIndustry(UpdateIndustryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
           {
               var entity = _basicStockInfoRepository.Entities.Where(p => p.TSCode == request.TSCode).SingleOrDefault();
               if (entity != null)
               {
                   entity.IndustryId = request.IndustryId;
                   _basicStockInfoRepository.Update(entity);
               }

               return new Empty();
           });

        }

        public override Task<GetBasicStockInfoResponse> GetBasicStockInfo(GetBasicStockInfoRequest request, ServerCallContext context)
        {
            GetBasicStockInfoResponse response = new GetBasicStockInfoResponse();

            return Task.Run(() =>
            {
                foreach (var tscode in request.TSCodes)
                {
                    var entity = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tscode).SingleOrDefault();
                    BasicStockInfoDto entityDto = null;
                    if (entity != null)
                    {
                        entityDto = _imapper.Map<BasicStockInfoDto>(entity);
                        response.Entities.Add(entityDto);
                    }
                }
                return response;
            });
        }



        public override Task<GetSuggestStocksResponse> GetSuggestStocks(GetSuggestStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSuggestStocksResponse response = new GetSuggestStocksResponse();

                var entities = _basicStockInfoRepository.Entities.Where(p => (p.TSCode.Contains(request.Pattern)||
                p.StockName.Contains(request.Pattern)) && p.ListedStatus==StockEssentialDataConst.Listing).Take(20);
                if (entities != null)
                {
                    foreach (var e in entities.ToList())
                    {
                        string indus = _industryRepository.Entities.Where(p => p.Id == e.IndustryId).Select(p => p.Name).SingleOrDefault();
                        response.Entities.Add(new  RelatedStockItem { TSCode=e.TSCode, Name= e.StockName , Industry=indus??""});
                    }
                }

                return response;
            });

        }


        public override Task<GetSameIndustryStocksResponse> GetSameIndustryStocks(GetSameIndustryStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSameIndustryStocksResponse response = new GetSameIndustryStocksResponse();

                var binfo = _basicStockInfoRepository.Entities.Where(p => p.TSCode == request.TSCode).FirstOrDefault();
                if(binfo!=null)
                {
                   var stocks= _basicStockInfoRepository.Entities.Where(p => p.IndustryId == binfo.IndustryId);

                    foreach (var e in stocks.ToList())
                    {
                        string indus = _industryRepository.Entities.Where(p => p.Id == e.IndustryId).Select(p => p.Name).SingleOrDefault();
                        response.Entities.Add(new RelatedStockItem { TSCode = e.TSCode, Name = e.StockName, Industry = indus ?? "" });
                    }
                }
                return response;
            });
            
        }





    }
}
