using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Repository.Abstract;
using AutoMapper;
using DawnQuant.AShare.Entities;
using Google.Protobuf.WellKnownTypes;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Entities.UserProfile;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class SubjectAndHotStockCategoryService : SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiBase
    {
        private readonly ILogger<SubjectAndHotStockCategoryService> _logger;
        private readonly ISubjectAndHotStockCategoryRepository _subjectAndHotStockCategoryRepository;
        private readonly ISubjectAndHotStockRepository  _subjectAndHotStockRepository;

        private readonly IMapper _mapper;

        public SubjectAndHotStockCategoryService(ILogger<SubjectAndHotStockCategoryService> logger, IMapper mapper,
        ISubjectAndHotStockCategoryRepository SubjectAndHotStockCategoryRepository,
        ISubjectAndHotStockRepository SubjectAndHotStockRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _subjectAndHotStockCategoryRepository = SubjectAndHotStockCategoryRepository;
            _subjectAndHotStockRepository = SubjectAndHotStockRepository;
        }

        public override Task<GetSubjectAndHotStockCategoriesByUserResponse> GetSubjectAndHotStockCategoriesByUser(GetSubjectAndHotStockCategoriesByUserRequest request, ServerCallContext context)
        {

            return Task.Run(() =>
            {
                GetSubjectAndHotStockCategoriesByUserResponse response = new GetSubjectAndHotStockCategoriesByUserResponse();
                var data = _subjectAndHotStockCategoryRepository.Entities.Where(p => p.UserId == request.UserId).Select(p => p);
                response.Entities.AddRange(_mapper.Map<IEnumerable<SubjectAndHotStockCategoryDto>>(data));

                return response;
            });
           
        }


        public override Task<SaveSubjectAndHotStockCategoriesResponse> SaveSubjectAndHotStockCategories(SaveSubjectAndHotStockCategoriesRequest request, ServerCallContext context)
        {
            SaveSubjectAndHotStockCategoriesResponse response = new SaveSubjectAndHotStockCategoriesResponse();

            return Task.Run(() =>
            {
                var data = _mapper.Map<IEnumerable<SubjectAndHotStockCategory>>(request.Entities);

                var rdata = _subjectAndHotStockCategoryRepository.Save(data);
                response.Entities.AddRange(_mapper.Map<IEnumerable<SubjectAndHotStockCategoryDto>>(rdata));
                return response;

            });
        }


        public override Task<Empty> DelSubjectAndHotStockCategory(DelSubjectAndHotStockCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
               var stocks= _subjectAndHotStockRepository.Entities.Where(p => p.CategoryId == request.CategoryId).ToList();

                //删除分类下的自选股
                _subjectAndHotStockRepository.Delete(stocks);
                //删除分类
                _subjectAndHotStockCategoryRepository.Delete(request.CategoryId);
                return new Empty();
            });
           
        }
    }
}
