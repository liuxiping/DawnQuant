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
    public class BellwetherService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;



        public BellwetherService(GrpcChannelSet grpcChannelSet,
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
        public void DelBellwetherStockCategory(BellwetherStockCategory category)
        {
            var client = new BellwetherStockCategoryApi.BellwetherStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelBellwetherStockCategory(new DelBellwetherStockCategoryRequest { CategoryId = category.Id });
        }

        /// <summary>
        /// 删除龙头股
        /// </summary>
        /// <param name="item"></param>
        public void DelBellwetherStock(BellwetherStock item)
        {
            var client = new BellwetherStockApi.BellwetherStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelBellwetherStockById(new DelBellwetherStockByIdRequest { Id = item.Id });
        }


        /// <summary>
        /// 获取龙头股分类
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ObservableCollection<BellwetherStockCategory> GetBellwetherStockCategories(long userid)
        {

            var client = new BellwetherStockCategoryApi.BellwetherStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetBellwetherStockCategoriesByUser(new GetBellwetherStockCategoriesByUserRequest { UserId = userid });

            return _mapper.Map<ObservableCollection<BellwetherStockCategory>>(dtos.Entities.OrderBy(p => p.SortNum));

        }

        /// <summary>
        /// 根据龙头股分类获取相关龙头股
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ObservableCollection<BellwetherStock> GetBellwetherStocksByCategory(long categoryId)
        {
            var client = new BellwetherStockApi.BellwetherStockApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetBellwetherStocksByCategory(new GetBellwetherStocksByCategoryRequest { CategoryId = categoryId });
            return _mapper.Map<ObservableCollection<BellwetherStock>>(dtos.Entities);
        }


        /// <summary>
        /// 根据用户获取龙头股
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<BellwetherStock> GetBellwetherStocksByUser(long userId)
        {
            var client = new BellwetherStockApi.BellwetherStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetBellwetherStocksByUser(new GetBellwetherStocksByUserRequest { UserId = userId });
            return _mapper.Map<ObservableCollection<BellwetherStock>>(dtos.Entities);
        }

        

        
        /// <summary>
        /// 保存龙头股分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public BellwetherStockCategory SaveBellwetherCategory(BellwetherStockCategory category)
        {
            var client = new BellwetherStockCategoryApi.BellwetherStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveBellwetherStockCategoriesRequest request = new SaveBellwetherStockCategoriesRequest();
            request.Entities.Add(_mapper.Map<BellwetherStockCategoryDto>(category));

            var response = client.SaveBellwetherStockCategories(request, meta);

            return _mapper.Map<IEnumerable<BellwetherStockCategory>>(response.Entities).FirstOrDefault();
        }

        /// <summary>
        /// 保存龙头股
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public BellwetherStock SaveBellwetherStock(BellwetherStock item)
        {
            var client = new BellwetherStockApi.BellwetherStockApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            SaveBellwetherStocksRequest request = new SaveBellwetherStocksRequest();
            request.Entities.Add(_mapper.Map<BellwetherStockDto>(item));
            var response = client.SaveBellwetherStocks(request, meta);
            return _mapper.Map<IEnumerable<BellwetherStock>>(response.Entities).SingleOrDefault();
        }


        /// <summary>
        /// 获取提示股票
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public ObservableCollection<BellwetherStock> GetSuggestStocks(string pattern)
        {
            ObservableCollection<BellwetherStock> rdata = new ObservableCollection<BellwetherStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSuggestStocks(new GetSuggestStocksRequest { Pattern = pattern }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                if (reponse?.Entities?.Count > 0)
                {
                    rdata = _mapper.Map<ObservableCollection<BellwetherStock>>(reponse.Entities);
                }
            }
            return rdata;
        }


        /// <summary>
        /// 获取相同行业股票
        /// </summary>
        /// <param name="tsCode"></param>
        /// <returns></returns>
        public ObservableCollection<BellwetherStock> GetSameIndustryStocks(string tsCode)
        {
            ObservableCollection<BellwetherStock> rdata = new ObservableCollection<BellwetherStock>();

            var client = new BasicStockInfoApi.BasicStockInfoApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetSameIndustryStocks(new GetSameIndustryStocksRequest { TSCode = tsCode }, meta);

            if (reponse?.Entities?.Count > 0)
            {
                rdata = _mapper.Map<ObservableCollection<BellwetherStock>>(reponse.Entities);
            }
            return rdata;
        }
    }
}
