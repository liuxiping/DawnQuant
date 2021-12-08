using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Models.AShare.StrategyMetadata;
using DawnQuant.Passport;
using Grpc.Core;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.Utils;
using DawnQuant.AShare.Api.StrategyMetadata;

namespace DawnQuant.App.Core.Services.AShare
{
    public class StrategyMetadataService 
    {

        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StrategyMetadataService(GrpcChannelSet grpcChannelSet,
           IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }


        /// <summary>
        /// 获取选股因子分类和选股因子元数据
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FactorMetadataCategory> GetFactorMetadataCategoriesIncludeItems()
        {
            var client = new FactorMetadataCategoryApi.FactorMetadataCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetFactorMetadataCategoriesIncludeItems(new Google.Protobuf.WellKnownTypes.Empty(), meta);

            return _mapper.Map<ObservableCollection<FactorMetadataCategory>>(dtos.Entities);
        }

        /// <summary>
        /// 获取选股范围分类和选股范围元数据
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<SelectScopeMetadataCategory> GetSelectScopeMetadataCategoriesIncludeItems()
        {
            var client = new SelectScopeMetadataCategoryApi.SelectScopeMetadataCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetSelectScopeMetadataCategoriesIncludeItems(new  Google.Protobuf.WellKnownTypes.Empty(),meta);

           return _mapper.Map<ObservableCollection<SelectScopeMetadataCategory>>(dtos.Entities);
           
        }

       
    }
}
