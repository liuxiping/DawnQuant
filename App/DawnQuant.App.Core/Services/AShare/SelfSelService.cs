using Grpc.Core;
using AutoMapper;
using System.Collections.ObjectModel;
using DawnQuant.Passport;
using DawnQuant.App.Core.Models;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.App.Core.Models.AShare.UserProfile;

namespace DawnQuant.App.Core.Services.AShare
{
    /// <summary>
    /// 自选分类 自选股票服务
    /// </summary>
    public class SelfSelService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public SelfSelService(GrpcChannelSet grpcChannelSet,
          IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }
      

        /// <summary>
        /// 删除自选分类
        /// </summary>
        /// <param name="category"></param>
        public void DelSelfSelectCategory(SelfSelectStockCategory category)
        {

            var client = new SelfSelectStockCategoryApi.SelfSelectStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelStockCategory(new DelStockCategoryRequest { CategoryId = category.Id });
        }

        /// <summary>
        /// 删除自选股
        /// </summary>
        /// <param name="item"></param>
        public void DelSelfSelectStock(SelfSelectStock item)
        {

            var client = new SelfSelectStockApi.SelfSelectStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelSelfSelectStockById(new DelSelfSelectStockByIdRequest { Id = item.Id });
        }


        /// <summary>
        /// 获取相同行业股票
        /// </summary>
        /// <param name="tsCode"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> GetSameIndustryStocks(string tsCode)
        {
            ObservableCollection<SelfSelectStock> rdata = new ObservableCollection<SelfSelectStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSameIndustryStocks(new GetSameIndustryStocksRequest { TSCode = tsCode }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                rdata = _mapper.Map<ObservableCollection<SelfSelectStock>>(reponse.Entities);
            }
            return rdata;
        }


        /// <summary>
        /// 获取自选股分类
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStockCategory> GetSelfSelectStockCategories(long userid)
        {

            var client = new SelfSelectStockCategoryApi.SelfSelectStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetStockCategoriesByUser(new GetStockCategoriesByUserRequest { UserId = userid });

            return _mapper.Map<ObservableCollection<SelfSelectStockCategory>>(dtos.Entities.OrderBy(p => p.SortNum));

        }

        /// <summary>
        /// 根据分类获取自选股
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> GetSelfSelectStocksByCategory(long categoryId)
        {
            var client = new SelfSelectStockApi.SelfSelectStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSelfSelectStocksByCategory(new GetSelfSelectStocksByCategoryRequest { CategoryId = categoryId });
            return _mapper.Map<ObservableCollection<SelfSelectStock>>(dtos.Entities);
        }


        /// <summary>
        /// 根据用户获取自选股
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> GetSelfSelectStocksByUser(long userId)
        {
            var client = new SelfSelectStockApi.SelfSelectStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSelfSelectStocksByUser(new GetSelfSelectStocksByUserRequest { UserId = userId });
            return _mapper.Map<ObservableCollection<SelfSelectStock>>(dtos.Entities);
        }

        /// <summary>
        /// 获取提示股票
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> GetSuggestStocks(string pattern)
        {
            ObservableCollection<SelfSelectStock> rdata = new ObservableCollection<SelfSelectStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSuggestStocks(new GetSuggestStocksRequest { Pattern = pattern }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                if (reponse?.Entities?.Count > 0)
                {
                    rdata = _mapper.Map<ObservableCollection<SelfSelectStock>>(reponse.Entities);
                }
            }
            return rdata;
        }

        
        /// <summary>
        /// 保存自选股分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public SelfSelectStockCategory SaveSelfSelectCategory(SelfSelectStockCategory category)
        {
            var client = new SelfSelectStockCategoryApi.SelfSelectStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveStockCategoriesRequest request = new SaveStockCategoriesRequest();
            request.Entities.Add(_mapper.Map<SelfSelectStockCategoryDto>(category));

            var response = client.SaveStockCategories(request, meta);

            return _mapper.Map<IEnumerable<SelfSelectStockCategory>>(response.Entities).FirstOrDefault();
        }

        /// <summary>
        /// 保存自选股
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public SelfSelectStock SaveSelfSelectStock(SelfSelectStock item)
        {
            var client = new SelfSelectStockApi.SelfSelectStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveSelfSelectStocksRequest request = new SaveSelfSelectStocksRequest();
            request.Entities.Add(_mapper.Map<SelfSelectStockDto>(item));
            var response = client.SaveSelfSelectStocks(request, meta);
            return _mapper.Map<IEnumerable<SelfSelectStock>>(response.Entities).SingleOrDefault();
        }

        ///导入自选股
        public List<SelfSelectStock> ImportSelfStocks(long userId, List<string> stocksId, long categoryId)
        {
            var client = new SelfSelectStockApi.SelfSelectStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            ImportSelfStocksRequest request = new ImportSelfStocksRequest();
            request.CategoryId = categoryId;
            request.UserId = userId;
            request.StocksId.AddRange(stocksId);

            var response = client.ImportSelfStocks(request, meta);

            return _mapper.Map<IEnumerable<SelfSelectStock>>(response.Entities).ToList();


        }
    }
}
