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
using Microsoft.EntityFrameworkCore;
using System.Transactions;

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

                //ɾ�������µ���ѡ��
                _subjectAndHotStockRepository.Delete(stocks);
                //ɾ������
                _subjectAndHotStockCategoryRepository.Delete(request.CategoryId);
                return new Empty();
            });
           
        }

        public override Task<Empty> MergeSubjectAndHotStockCategory(MergeSubjectAndHotStockCategoryRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SubjectAndHotStockCategory toCategory=_subjectAndHotStockCategoryRepository.Entities
                .Where(p=>p.Id==request.ToCategoryId && p.UserId== request.UserId).FirstOrDefault();

                if (toCategory != null)
                {
                    //��ӵ��ϲ��ķ�����
                    List<SubjectAndHotStock> ustocks = new List<SubjectAndHotStock>();

                    List<SubjectAndHotStock> allStocks = new List<SubjectAndHotStock>();
                    foreach (var cid in request.FromCategoriesId)
                    {
                        var stocks = _subjectAndHotStockRepository.Entities.Where(p => p.CategoryId == cid).ToList();
                        allStocks.AddRange(stocks);
                    }
                    //ȥ��
                    allStocks = allStocks.DistinctBy(p => p.TSCode).ToList();

                    foreach (var stock in allStocks)
                    {
                        //��������Ƿ���� ������������ʱ��
                        var self = _subjectAndHotStockRepository.Entities.Where(p => p.UserId == request.UserId &&
                            p.CategoryId == toCategory.Id && p.TSCode == stock.TSCode).AsNoTracking().FirstOrDefault();
                        if (self != null)
                        {
                            //�Ѿ����� ���´���ʱ��
                            self.CreateTime = DateTime.Now;
                            ustocks.Add(self);
                        }
                        else
                        {
                            //���ķ���
                            stock.CreateTime = DateTime.Now;
                            stock.CategoryId = toCategory.Id;
                            ustocks.Add(stock);
                        }

                        using (var scope = new TransactionScope())
                        {
                            //���浽�ϲ��ķ�����
                            _subjectAndHotStockRepository.Save(ustocks);

                            //ɾ������

                            foreach (var cid in request.FromCategoriesId)
                            {
                                _subjectAndHotStockCategoryRepository.Delete(cid);
                            }
                            scope.Complete();
                        }
                    }
                }
                return new Empty();
            });
        }
    }
}
