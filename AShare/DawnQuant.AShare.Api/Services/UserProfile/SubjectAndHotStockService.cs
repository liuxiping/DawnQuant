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
        private readonly IMapper _mapper;

        public SubjectAndHotStockService(ILogger<SubjectAndHotStockService> logger, IMapper mapper,
        ISubjectAndHotStockRepository SubjectAndHotStockRepository,
        IBasicStockInfoRepository basicStockInfoRepository )
        {
            _logger = logger;
            _mapper = mapper;
            _subjectAndHotStockRepository = SubjectAndHotStockRepository;
            _basicStockInfoRepository = basicStockInfoRepository;
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


        

    }
}
