using Grpc.Core;
using AutoMapper;
using System.Collections.ObjectModel;
using DawnQuant.Passport;
using DawnQuant.App.Core.Models;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Utils;

namespace DawnQuant.App.Core.Services.AShare
{
    /// <summary>
    /// 龙头股相关接口
    /// </summary>
    public class SubjectAndHotService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public SubjectAndHotService(GrpcChannelSet grpcChannelSet,
         IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
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
        public ObservableCollection<SubjectAndHotStockCategory> GetSubjectAndHotStockCategories(long userid)
        {

            var client = new SubjectAndHotStockCategoryApi.SubjectAndHotStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSubjectAndHotStockCategoriesByUser(new GetSubjectAndHotStockCategoriesByUserRequest { UserId = userid });

            return _mapper.Map<ObservableCollection<SubjectAndHotStockCategory>>(dtos.Entities.OrderBy(p => p.SortNum));

        }

        /// <summary>
        /// 根据龙头股分类获取相关龙头股
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
        /// 根据用户获取龙头股
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
        /// 保存龙头股分类
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
        /// 保存龙头股
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
    }
}
