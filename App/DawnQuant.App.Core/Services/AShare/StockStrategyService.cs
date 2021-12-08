using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Grpc.Core;
using System.Collections.ObjectModel;

namespace DawnQuant.App.Core.Services.AShare
{
    public class StockStrategyService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StockStrategyService(GrpcChannelSet grpcChannelSet,
           IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据用户获取策略分类以及策略
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<StockStrategyCategory> GetCategoriesIncludeStrategiesByUser(long userId)
        {
            var client = new StockStrategyCategoryApi.StockStrategyCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetCategoriesIncludeStrategiesByUser(new GetCategoriesIncludeStrategiesByUserRequest { UserId = userId }, meta);
            ObservableCollection<StockStrategyCategory> cs = new ObservableCollection<StockStrategyCategory>();
            foreach (var item in dtos.Entities)
            {
                cs.Add(_mapper.Map<StockStrategyCategory>(item));
            }

            return cs;
        }

        
        /// <summary>
        /// 根据用户获取策略
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ObservableCollection<StockStrategyCategory> GetStockStrategyCategoriesByUser(long userId)
        {
            var client = new StockStrategyCategoryApi.StockStrategyCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetStrategyCategoriesByUser(new GetStrategyCategoriesByUserRequest { UserId = userId }, meta);

            return _mapper.Map<ObservableCollection<StockStrategyCategory>>(dtos.Entities);
        }


        /// <summary>
        /// 保存策略
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns></returns>
        public StockStrategy SaveStockStrategy(StockStrategy strategy)
        {
            var client = new StockStrategyApi.StockStrategyApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            SaveStockStrategiesRequest request = new SaveStockStrategiesRequest();
            request.Entities.Add(_mapper.Map<StockStrategyDto>(strategy));
            var dtos = client.SaveStockStrategies(request, meta);
            return _mapper.Map<IEnumerable<StockStrategy>>(dtos.Entities).FirstOrDefault();
        }


        /// <summary>
        /// 删除策略
        /// </summary>
        /// <param name="id"></param>
        public void DelStockStrategy(long id)
        {
            var client = new StockStrategyApi.StockStrategyApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelStockStrategyById(new DelStockStrategyByIdRequest { Id = id }, meta);
        }


        /// <summary>
        /// 保存策略分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public StockStrategyCategory SaveStockStrategyCategory(StockStrategyCategory category)
        {
            var client = new StockStrategyCategoryApi.StockStrategyCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var request = new SaveStrategyCategoriesRequest();
            request.Entities.Add(_mapper.Map<StockStrategyCategoryDto>(category));
            var r = client.SaveStrategyCategories(request, meta);

            return _mapper.Map<IEnumerable<StockStrategyCategory>>(r.Entities).FirstOrDefault();
        }


        /// <summary>
        /// 删除策略分类
        /// </summary>
        /// <param name="category"></param>
        public void DelStockStrategyCategory(StockStrategyCategory category)
        {
            var client = new StockStrategyCategoryApi.StockStrategyCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            client.DelStrategyCategory(new DelStrategyCategoryRequest { Id = category.Id }, meta);
        }

    }
}
