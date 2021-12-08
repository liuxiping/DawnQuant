using AutoMapper;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
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
    public class BellwetherStockService : BellwetherStockApi.BellwetherStockApiBase
    {
        private readonly ILogger<BellwetherStockService> _logger;
        private readonly IBellwetherStockRepository _bellwetherStockRepository;
        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IMapper _mapper;

        public BellwetherStockService(ILogger<BellwetherStockService> logger, IMapper mapper,
        IBellwetherStockRepository bellwetherStockRepository,
        IBasicStockInfoRepository basicStockInfoRepository )
        {
            _logger = logger;
            _mapper = mapper;
            _bellwetherStockRepository = bellwetherStockRepository;
            _basicStockInfoRepository = basicStockInfoRepository;
        }


        public override Task<GetBellwetherStocksByCategoryResponse> GetBellwetherStocksByCategory(GetBellwetherStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetBellwetherStocksByCategoryResponse response = new GetBellwetherStocksByCategoryResponse();
                var data = _bellwetherStockRepository.Entities.OrderByDescending(p => p.CreateTime).Where(p => p.CategoryId == request.CategoryId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockDto>>(data));

                return response;
            });
        }

        public override Task<GetBellwetherStocksByUserResponse> GetBellwetherStocksByUser(GetBellwetherStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetBellwetherStocksByUserResponse response = new GetBellwetherStocksByUserResponse();
                var data = _bellwetherStockRepository.Entities.Where(p => p.UserId == request.UserId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockDto>>(data));

                return response;
            });
        }

        public override Task<SaveBellwetherStocksResponse> SaveBellwetherStocks(SaveBellwetherStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveBellwetherStocksResponse response = new SaveBellwetherStocksResponse();

                var data = _mapper.Map<IEnumerable<BellwetherStock>>(request.Entities);


                foreach (var s in data)
                {
                    var cur = _bellwetherStockRepository.Entities.Where(p => p.UserId == s.UserId &&
                      p.CategoryId == s.CategoryId && p.TSCode == s.TSCode).AsNoTracking().FirstOrDefault();

                    //更新
                    if (cur != null)
                    {
                        s.Id = cur.Id;
                    }

                }
                var rdata = _bellwetherStockRepository.Save(data);

                response.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockDto>>(rdata));

                return response;

            });
        }


        public override Task<Empty> DelBellwetherStockById(DelBellwetherStockByIdRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {

                 _bellwetherStockRepository.Delete(request.Id);

                 return new Empty();
             });
        }

        public override Task<Empty> DelBellwetherStocksByCategory(DelBellwetherStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 var items = _bellwetherStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                 _bellwetherStockRepository.Delete(items);
                 return new Empty();
             });
        }

        public override Task<Empty> DelBellwetherStocksByUser(DelBellwetherStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var items = _bellwetherStockRepository.Entities.Where(p => p.UserId == request.UserId).ToList();

                _bellwetherStockRepository.Delete(items);
                return new Empty();
            });
        }


        /// <summary>
        /// 导入自选股
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ImportBellwetherStocksResponse> ImportBellwetherStocks(ImportBellwetherStocksRequest request, ServerCallContext context)
        {
            //return Task.Run(() =>
            // {
            //     ImportSelfStocksResponse repsonse = new ImportSelfStocksResponse();

            //     if (request.StocksId != null && request.StocksId.Count > 0)
            //     {

            //         获取TScodes
            //         List<BellwetherStock> ustocks = new List<BellwetherStock>();

            //         foreach (var s in request.StocksId)
            //         {
            //             string tsCode = _basicStockInfoRepository.Entities.Where(p => p.TSCode.Contains(s)).Select(p => p.TSCode).FirstOrDefault();

            //             if (!string.IsNullOrEmpty(tsCode))
            //             {

            //                 检测数据是否存在 如果存在则更新时间
            //                 var self = _bellwetherStockRepository.Entities.Where(p => p.UserId == request.UserId &&
            //                     p.CategoryId == request.CategoryId && p.TSCode == s).FirstOrDefault();
            //                 if (self != null)
            //                 {
            //                     更新创建时间
            //                     self.CreateTime = DateTime.Now;
            //                     ustocks.Add(self);

            //                 }
            //                 else
            //                 {
            //                     BellwetherStock BellwetherStock = new BellwetherStock();
            //                     BellwetherStock.TSCode = tsCode;
            //                     BellwetherStock.UserId = request.UserId;
            //                     BellwetherStock.CategoryId = request.CategoryId;
            //                     BellwetherStock.CreateTime = DateTime.Now;

            //                     获取行业和名称
            //                     var basicInfo = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tsCode).FirstOrDefault();
            //                     string indus = _industryRepository.Entities
            //                      .Where(p => p.Id == basicInfo.IndustryId).Select(p => p.Name).SingleOrDefault();
            //                     BellwetherStock.Name = basicInfo.StockName;
            //                     BellwetherStock.Industry = indus;

            //                     ustocks.Add(BellwetherStock);
            //                 }

            //             }
            //         }

            //         if (ustocks.Count > 0)
            //         {
            //             保存自选股
            //             _bellwetherStockRepository.Save(ustocks);

            //             repsonse.Entities.AddRange(_mapper.Map<IEnumerable<BellwetherStockDto>>(ustocks));

            //         }

            //     }

            //     return repsonse;
            // });

            return null;
        }


    }
}
