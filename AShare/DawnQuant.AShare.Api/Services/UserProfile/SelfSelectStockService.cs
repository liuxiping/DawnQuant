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
    public class SelfSelectStockService : SelfSelectStockApi.SelfSelectStockApiBase
    {
        private readonly ILogger<SelfSelectStockService> _logger;
        private readonly ISelfSelectStockRepository _selfSelectStockRepository;
        private readonly ISelfSelectStockCategoryRepository _selfSelectStockCategoryRepository;

        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IIndustryRepository _industryRepository;
        private readonly IMapper _mapper;

        public SelfSelectStockService(ILogger<SelfSelectStockService> logger, IMapper mapper,
        ISelfSelectStockRepository selfSelfSelStockItemRepository,
        ISelfSelectStockCategoryRepository selfSelectStockCategoryRepository,
        IBasicStockInfoRepository basicStockInfoRepository,
         IIndustryRepository industryRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _selfSelectStockRepository = selfSelfSelStockItemRepository;
            _selfSelectStockCategoryRepository = selfSelectStockCategoryRepository;
            _basicStockInfoRepository = basicStockInfoRepository;
            _industryRepository = industryRepository;
        }


        public override Task<GetSelfSelectStocksByCategoryResponse> GetSelfSelectStocksByCategory(GetSelfSelectStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSelfSelectStocksByCategoryResponse response = new GetSelfSelectStocksByCategoryResponse();

                //获取排序字段
                var category = _selfSelectStockCategoryRepository.Entities.Where(p => p.Id == request.CategoryId).FirstOrDefault();

                IQueryable<SelfSelectStock> data = null;

                if (category != null)
                {
                    //加入时间
                    if (category.StockSortFiled == 1)
                    {
                        data = _selfSelectStockRepository.Entities.OrderByDescending(p => p.CreateTime).Where(p => p.CategoryId == request.CategoryId)
                                   .Select(p => p);
                    }
                    //行业
                    else if (category.StockSortFiled == 2)
                    {
                        data = _selfSelectStockRepository.Entities.OrderByDescending(p => p.Industry).Where(p => p.CategoryId == request.CategoryId)
                                   .Select(p => p);
                    }

                }

                response.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockDto>>(data));

                return response;
            });
        }

        public override Task<GetSelfSelectStocksByUserResponse> GetSelfSelectStocksByUser(GetSelfSelectStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSelfSelectStocksByUserResponse response = new GetSelfSelectStocksByUserResponse();
                var data = _selfSelectStockRepository.Entities.Where(p => p.UserId == request.UserId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockDto>>(data));

                return response;
            });
        }

        public override Task<SaveSelfSelectStocksResponse> SaveSelfSelectStocks(SaveSelfSelectStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveSelfSelectStocksResponse response = new SaveSelfSelectStocksResponse();

                var data = _mapper.Map<IEnumerable<SelfSelectStock>>(request.Entities);


                foreach (var s in data)
                {
                    var cur = _selfSelectStockRepository.Entities.Where(p => p.UserId == s.UserId &&
                      p.CategoryId == s.CategoryId && p.TSCode == s.TSCode).AsNoTracking().FirstOrDefault();

                    //更新
                    if (cur != null)
                    {
                        s.Id = cur.Id;
                    }

                }
                var rdata = _selfSelectStockRepository.Save(data);

                response.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockDto>>(rdata));

                return response;

            });
        }


        public override Task<Empty> DelSelfSelectStockById(DelSelfSelectStockByIdRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {

                 _selfSelectStockRepository.Delete(request.Id);

                 return new Empty();
             });
        }

        public override Task<Empty> DelSelfSelectStocksByCategory(DelSelfSelectStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 var items = _selfSelectStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                 _selfSelectStockRepository.Delete(items);
                 return new Empty();
             });
        }

        public override Task<Empty> DelSelfSelectStocksByUser(DelSelfSelectStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var items = _selfSelectStockRepository.Entities.Where(p => p.UserId == request.UserId).ToList();

                _selfSelectStockRepository.Delete(items);
                return new Empty();
            });
        }


        /// <summary>
        /// 导入自选股
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ImportSelfStocksResponse> ImportSelfStocks(ImportSelfStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 ImportSelfStocksResponse repsonse = new ImportSelfStocksResponse();

                 if (request.StocksId != null && request.StocksId.Count > 0)
                 {

                     //获取TScodes
                     List<SelfSelectStock> ustocks = new List<SelfSelectStock>();

                     foreach (var s in request.StocksId)
                     {
                         string tsCode = _basicStockInfoRepository.Entities.Where(p => p.TSCode.Contains(s)).Select(p => p.TSCode).FirstOrDefault();

                         if (!string.IsNullOrEmpty(tsCode))
                         {

                             //检测数据是否存在 如果存在则更新时间
                             var self = _selfSelectStockRepository.Entities.Where(p => p.UserId == request.UserId &&
                                 p.CategoryId == request.CategoryId && p.TSCode == tsCode).FirstOrDefault();
                             if (self != null)
                             {
                                 //更新创建时间
                                 self.CreateTime = DateTime.Now;
                                 ustocks.Add(self);

                             }
                             else
                             {
                                 SelfSelectStock selfSelectStock = new SelfSelectStock();
                                 selfSelectStock.TSCode = tsCode;
                                 selfSelectStock.UserId = request.UserId;
                                 selfSelectStock.CategoryId = request.CategoryId;
                                 selfSelectStock.CreateTime = DateTime.Now;

                                 //获取行业和名称
                                 var basicInfo = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tsCode  &&!p.StockName.Contains("退")).FirstOrDefault();
                                 string indus = _industryRepository.Entities
                                  .Where(p => p.Id == basicInfo.IndustryId).Select(p => p.Name).SingleOrDefault();
                                 selfSelectStock.Name = basicInfo.StockName;
                                 selfSelectStock.Industry = indus;

                                 ustocks.Add(selfSelectStock);
                             }

                         }
                     }

                     if (ustocks.Count > 0)
                     {
                         //保存自选股
                         _selfSelectStockRepository.Save(ustocks);

                         repsonse.Entities.AddRange(_mapper.Map<IEnumerable<SelfSelectStockDto>>(ustocks));

                     }

                 }

                 return repsonse;
             });


        }


    }
}
