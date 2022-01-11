using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class THSIndexService : THSIndexApi.THSIndexApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly ITHSIndexRepository _indexRepository;
        private readonly ITHSIndexMemberRepository _memberRepository;

        private readonly ISubjectAndHotStockCategoryRepository _subjectAndHotStockCategoryRepository;
        private readonly ISubjectAndHotStockRepository _subjectAndHotStockRepository;

        private readonly IMapper _imapper;


        public THSIndexService(ILogger<HolderService> logger,
          ITHSIndexRepository thsIndexRepository, IMapper imapper,
          ITHSIndexMemberRepository memberRepository,
          ISubjectAndHotStockCategoryRepository subjectAndHotStockCategoryRepository,
          ISubjectAndHotStockRepository subjectAndHotStockRepository)
        {
            _logger = logger;
            _indexRepository = thsIndexRepository;
            _memberRepository = memberRepository;
            _subjectAndHotStockCategoryRepository = subjectAndHotStockCategoryRepository;
            _subjectAndHotStockRepository = subjectAndHotStockRepository;
            _imapper = imapper;
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SaveTHSIndexes(SaveTHSIndexesRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request.Entities.Count > 0)
                {

                    var datas = _imapper.Map<IEnumerable<THSIndex>>(request.Entities);

                    _indexRepository.Save(datas);
                }

                return new Empty();
            });

        }


        public override Task<GetAllTHSIndexesReponse> GetAllTHSIndexes(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetAllTHSIndexesReponse reponse = new GetAllTHSIndexesReponse();

                var datas = _indexRepository.Entities.ToList();

                if (datas != null && datas.Count > 0)
                {
                    var dtos = _imapper.Map<IEnumerable<THSIndexDto>>(datas);
                    reponse.Entities.AddRange(dtos);
                }
                return reponse;

            });
        }


        public override Task<Empty> AddToSubjectAndHot(AddToSubjectAndHotRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
           {

               //获取指数信息
               var index = _indexRepository.Entities.Where(p => p.TSCode == request.TSCode).FirstOrDefault();

               if (index != null)
               {
                   //获取成分股
                   var members = _memberRepository.Entities.Where(
                         p => p.TSCode == request.TSCode).ToList();

                   //查询题材热点是否已经存在 检查名称 同名即为存在
                   var category = _subjectAndHotStockCategoryRepository.Entities.Where(
                       p => p.Name == index.Name).FirstOrDefault();


                   List<SubjectAndHotStock> subjectAndHotStocks = new List<SubjectAndHotStock>();
                   //存在
                   if (category != null)
                   {
                       //查询分类中已经存在的股票

                       var existTSCodes = _subjectAndHotStockRepository.Entities
                       .Where(p => p.CategoryId == category.Id && p.UserId == request.UserId)
                       .Select(p => p.TSCode).ToList();

                       //添加成分股
                       foreach (var member in members)
                       {
                           if (!existTSCodes.Contains(member.Code))
                           {
                               SubjectAndHotStock subjectAndHotStock = new SubjectAndHotStock();
                               subjectAndHotStock.CategoryId = category.Id;
                               subjectAndHotStock.Industry = category.Name;
                               subjectAndHotStock.Name = member.Name;
                               subjectAndHotStock.TSCode = member.Code;
                               subjectAndHotStock.IsFocus = false;
                               subjectAndHotStock.UserId = request.UserId;
                               subjectAndHotStock.CreateTime = System.DateTime.Now;

                               subjectAndHotStocks.Add(subjectAndHotStock);
                           }
                       }
                       _subjectAndHotStockRepository.Save(subjectAndHotStocks);
                   }
                   else
                   {
                       using (var scope = new TransactionScope())
                       {
                           var c = new SubjectAndHotStockCategory();
                           c.CreateTime = System.DateTime.Now;
                           c.Name = index.Name;
                           c.UserId = request.UserId;
                           c.SortNum = _subjectAndHotStockCategoryRepository.Entities.
                            Select(p => p.SortNum).Min() - 1;
                           c.Desc = index.Name;

                           c = _subjectAndHotStockCategoryRepository.Save(c);

                           //添加成分股
                           foreach (var member in members)
                           {
                               SubjectAndHotStock subjectAndHotStock = new SubjectAndHotStock();

                               subjectAndHotStock.CategoryId = c.Id;
                               subjectAndHotStock.Industry = c.Name;
                               subjectAndHotStock.Name = member.Name;
                               subjectAndHotStock.TSCode = member.Code;
                               subjectAndHotStock.IsFocus = false;
                               subjectAndHotStock.UserId = request.UserId;
                               subjectAndHotStock.CreateTime = System.DateTime.Now;

                               subjectAndHotStocks.Add(subjectAndHotStock);
                           }
                           _subjectAndHotStockRepository.Save(subjectAndHotStocks);

                           scope.Complete();
                       }
                   }

               }
               return new Empty();
           });
        }

    }
}
