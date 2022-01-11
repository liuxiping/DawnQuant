using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Collections.ObjectModel;
using DawnQuant.Passport;
using DawnQuant.App.Models;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using Google.Protobuf.WellKnownTypes;

namespace DawnQuant.App.Services.AShare
{
    /// <summary>
    /// 龙头股相关接口
    /// </summary>
    public class SubjectAndHotService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;

        public SubjectAndHotService( )
        {

            _grpcChannelSet=IOCUtil.Container.Resolve<GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>(); 
            _mapper = IOCUtil.Container.Resolve<IMapper>(); ;
        }


        /// <summary>
        /// 删除龙头股分类
        /// </summary>
        /// <param name="category"></param>
        public void DelSubjectAndHotStockCategory(SubjectAndHotStockCategory category)
        {
            var client = new SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelSubjectAndHotStockCategory(new DelSubjectAndHotStockCategoryRequest { CategoryId = category.Id });
        }

        /// <summary>
        /// 删除龙头股
        /// </summary>
        /// <param name="item"></param>
        public void DelSubjectAndHotStock(SubjectAndHotStock item)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelSubjectAndHotStockById(new DelSubjectAndHotStockByIdRequest { Id = item.Id });
        }


        /// <summary>
        /// 获取龙头股分类
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ObservableCollection<SubjectAndHotStockCategory> GetSubjectAndHotStockCategories( )
        {

            var client = new SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSubjectAndHotStockCategoriesByUser(new GetSubjectAndHotStockCategoriesByUserRequest { UserId = _passportProvider.UserId });

            return _mapper.Map<ObservableCollection<SubjectAndHotStockCategory>>(dtos.Entities.OrderBy(p => p.SortNum));

        }

        /// <summary>
        /// 根据题材热点分类获取相关题材热点股
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ObservableCollection<SubjectAndHotStock> GetSubjectAndHotStocksByCategory(long categoryId)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSubjectAndHotStocksByCategory(new GetSubjectAndHotStocksByCategoryRequest { CategoryId = categoryId });
            return _mapper.Map<ObservableCollection<SubjectAndHotStock>>(dtos.Entities);
        }


        /// <summary>
        /// 根据用户获取题材热点股
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<SubjectAndHotStock> GetSubjectAndHotStocksByUser(long userId)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSubjectAndHotStocksByUser(new GetSubjectAndHotStocksByUserRequest { UserId = userId });
            return _mapper.Map<ObservableCollection<SubjectAndHotStock>>(dtos.Entities);
        }




        /// <summary>
        /// 保存题材热点分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public SubjectAndHotStockCategory SaveSubjectAndHotCategory(SubjectAndHotStockCategory category)
        {
            var client = new SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveSubjectAndHotStockCategoriesRequest request = new SaveSubjectAndHotStockCategoriesRequest();
            request.Entities.Add(_mapper.Map<SubjectAndHotStockCategoryDto>(category));

            var response = client.SaveSubjectAndHotStockCategories(request, meta);

            return _mapper.Map<IEnumerable<SubjectAndHotStockCategory>>(response.Entities).FirstOrDefault();
        }

        /// <summary>
        /// 保存题材热点
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public SubjectAndHotStock SaveSubjectAndHotStock(SubjectAndHotStock item)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveSubjectAndHotStocksRequest request = new SaveSubjectAndHotStocksRequest();
            request.Entities.Add(_mapper.Map<SubjectAndHotStockDto>(item));
            var response = client.SaveSubjectAndHotStocks(request, meta);
            return _mapper.Map<IEnumerable<SubjectAndHotStock>>(response.Entities).SingleOrDefault();
        }


        /// <summary>
        /// 获取提示股票
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public ObservableCollection<SubjectAndHotStock> GetSuggestStocks(string pattern)
        {
            ObservableCollection<SubjectAndHotStock> rdata = new ObservableCollection<SubjectAndHotStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSuggestStocks(new GetSuggestStocksRequest { Pattern = pattern }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                if (reponse?.Entities?.Count > 0)
                {
                    rdata = _mapper.Map<ObservableCollection<SubjectAndHotStock>>(reponse.Entities);
                }
            }
            return rdata;
        }


        /// <summary>
        /// 获取相同行业股票
        /// </summary>
        /// <param name="tsCode"></param>
        /// <returns></returns>
        public ObservableCollection<SubjectAndHotStock> GetSameIndustryStocks(string tsCode)
        {
            ObservableCollection<SubjectAndHotStock> rdata = new ObservableCollection<SubjectAndHotStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSameIndustryStocks(new GetSameIndustryStocksRequest { TSCode = tsCode }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                rdata = _mapper.Map<ObservableCollection<SubjectAndHotStock>>(reponse.Entities);
            }
            return rdata;
        }


        /// <summary>
        /// 导入题材热点股票
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="stocksId"></param>
        public void ImportSubjectAndHotStocks(long categoryId, List<string> stocksId)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            ImportSubjectAndHotStocksRequest request = new ImportSubjectAndHotStocksRequest();
            request.CategoryId = categoryId;
            request.UserId = _passportProvider.UserId;
            request.StocksId.AddRange(stocksId);
            client.ImportSubjectAndHotStocks(request, meta);
        }


        /// <summary>
        /// 获取行业分类
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Industry> GetAllInustries()
        {
            var client = new IndustryApi.IndustryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var response = client.GetAllIndustries(new Empty(), meta);

            ObservableCollection<Industry> industries = new ObservableCollection<Industry>();

            //顶级
            foreach (var f in response.Entities.Where(p => p.ParentId==0 && p.Level == 1))
            {
                Industry fi = new Industry()
                {
                    Id = f.Id,
                    Name = f.Name,
                    Level = f.Level,
                    ParentId = f.ParentId,
                    SubIndustries = new List<Industry>() 
                };

                industries.Add(fi);

                //第二级别
                foreach (var s in response.Entities.Where(p => p.ParentId == fi.Id &&
                p.Level == 2))
                {

                    Industry si = new Industry()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Level = s.Level,
                        ParentId = s.Id,
                        SubIndustries = new List<Industry>()
                    };

                    fi.SubIndustries.Add(si);

                    //第三级别

                    foreach (var t in response.Entities.Where(p => p.ParentId == si.Id &&
                    p.Level == 3))  
                    {
                        Industry ti = new Industry()
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Level = t.Level,
                            ParentId = si.Id,
                            SubIndustries = null
                        };
                        si.SubIndustries.Add(ti);
                    }
                }

            }
            return industries;

        }


        /// <summary>
        /// 根据行业导入题材热点股票
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="stocksId"></param>
        public void ImportSubjectAndHotStocksByIndustries(long categoryId, List<int> industries)
        {
            var client = new SubjectAndHotStockApi.SubjectAndHotStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            ImportSubjectAndHotStocksByIndustriesRequest request = new ImportSubjectAndHotStocksByIndustriesRequest();
            request.CategoryId = categoryId;
            request.UserId = _passportProvider.UserId;
            request.Industries.AddRange(industries);
            client.ImportSubjectAndHotStocksByIndustries(request, meta);
        }

        /// <summary>
        /// 合并分类
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public void MergeSubjectAndHotStockCategory(long to, List<long> from)
        {
            var client = new SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            MergeSubjectAndHotStockCategoryRequest request = new MergeSubjectAndHotStockCategoryRequest();
            request.ToCategoryId = to;
            request.UserId = _passportProvider.UserId;
            request.FromCategoriesId.AddRange(from);
            client.MergeSubjectAndHotStockCategory(request, meta);
        }
    }
}
