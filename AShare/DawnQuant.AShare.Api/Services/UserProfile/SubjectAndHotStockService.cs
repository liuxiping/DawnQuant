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
    public class SubjectAndHotStockService : SubjectAndHotStockApi.SubjectAndHotStockApiBase
    {
        private readonly ILogger<SubjectAndHotStockService> _logger;
        private readonly ISubjectAndHotStockRepository _subjectAndHotStockRepository;
        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IIndustryRepository _industryRepository;
        private readonly IMapper _mapper;

        public SubjectAndHotStockService(ILogger<SubjectAndHotStockService> logger, IMapper mapper,
        ISubjectAndHotStockRepository SubjectAndHotStockRepository,
        IBasicStockInfoRepository basicStockInfoRepository,
        IIndustryRepository industryRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _subjectAndHotStockRepository = SubjectAndHotStockRepository;
            _basicStockInfoRepository = basicStockInfoRepository;
            _industryRepository = industryRepository;
        }


        public override Task<GetSubjectAndHotStocksByCategoryResponse> GetSubjectAndHotStocksByCategory(GetSubjectAndHotStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSubjectAndHotStocksByCategoryResponse response = new GetSubjectAndHotStocksByCategoryResponse();
                var data = _subjectAndHotStockRepository.Entities.OrderByDescending(p => p.CreateTime).Where(p => p.CategoryId == request.CategoryId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<SubjectAndHotStockDto>>(data));

                return response;
            });
        }

        public override Task<GetSubjectAndHotStocksByUserResponse> GetSubjectAndHotStocksByUser(GetSubjectAndHotStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSubjectAndHotStocksByUserResponse response = new GetSubjectAndHotStocksByUserResponse();
                var data = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == request.UserId)
               .Select(p => p);

                response.Entities.AddRange(_mapper.Map<IEnumerable<SubjectAndHotStockDto>>(data));

                return response;
            });
        }

        public override Task<SaveSubjectAndHotStocksResponse> SaveSubjectAndHotStocks(SaveSubjectAndHotStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveSubjectAndHotStocksResponse response = new SaveSubjectAndHotStocksResponse();

                var data = _mapper.Map<IEnumerable<SubjectAndHotStock>>(request.Entities);


                foreach (var s in data)
                {
                    var cur = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == s.UserId &&
                      p.CategoryId == s.CategoryId && p.TSCode == s.TSCode).AsNoTracking().FirstOrDefault();

                    //更新
                    if (cur != null)
                    {
                        s.Id = cur.Id;
                    }

                }
                var rdata = _subjectAndHotStockRepository.Save(data);

                response.Entities.AddRange(_mapper.Map<IEnumerable<SubjectAndHotStockDto>>(rdata));

                return response;

            });
        }


        public override Task<Empty> DelSubjectAndHotStockById(DelSubjectAndHotStockByIdRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {

                 _subjectAndHotStockRepository.Delete(request.Id);

                 return new Empty();
             });
        }

        public override Task<Empty> DelSubjectAndHotStocksByCategory(DelSubjectAndHotStocksByCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {
                 var items = _subjectAndHotStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                 _subjectAndHotStockRepository.Delete(items);
                 return new Empty();
             });
        }

        public override Task<Empty> DelSubjectAndHotStocksByUser(DelSubjectAndHotStocksByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var items = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == request.UserId).ToList();

                _subjectAndHotStockRepository.Delete(items);
                return new Empty();
            });
        }


        public override Task<Empty> ImportSubjectAndHotStocks(ImportSubjectAndHotStocksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                if (request.StocksId != null && request.StocksId.Count > 0)
                {

                    //获取TScodes
                    List<SubjectAndHotStock> ustocks = new List<SubjectAndHotStock>();

                    foreach (var s in request.StocksId)
                    {
                        string tsCode = _basicStockInfoRepository.Entities.Where(p => p.TSCode.Contains(s)).Select(p => p.TSCode).FirstOrDefault();

                        if (!string.IsNullOrEmpty(tsCode))
                        {

                            //检测数据是否存在 如果存在则更新时间
                            var self = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == request.UserId &&
                                p.CategoryId == request.CategoryId && p.TSCode == tsCode).AsNoTracking().FirstOrDefault();
                            if (self != null)
                            {
                                //更新创建时间
                                self.CreateTime = DateTime.Now;
                                ustocks.Add(self);

                            }
                            else
                            {
                                SubjectAndHotStock subjectAndHotStock = new SubjectAndHotStock();
                                subjectAndHotStock.TSCode = tsCode;
                                subjectAndHotStock.UserId = request.UserId;
                                subjectAndHotStock.CategoryId = request.CategoryId;
                                subjectAndHotStock.CreateTime = DateTime.Now;

                                //名称
                                var basicInfo = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tsCode && !p.StockName.Contains("退")).FirstOrDefault();

                                string indus = _industryRepository.Entities
                                .Where(p => p.Id == basicInfo.IndustryId).Select(p => p.Name).SingleOrDefault();

                                subjectAndHotStock.Name = basicInfo.StockName;
                                subjectAndHotStock.Industry = indus;

                                ustocks.Add(subjectAndHotStock);
                            }

                        }
                    }

                    if (ustocks.Count > 0)
                    {
                        //保存
                        _subjectAndHotStockRepository.Save(ustocks);

                    }
                    
                }

                return new Empty();

            });

        }


    }
}
