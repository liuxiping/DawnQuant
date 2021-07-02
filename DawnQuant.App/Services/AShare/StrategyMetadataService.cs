using AutoMapper;
using DawnQuant.App.Models;
using DawnQuant.App.Models.AShare.Strategy;
using DawnQuant.App.Models.AShare.StrategyMetadata;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.App.Utils;
using DawnQuant.AShare.Api.StrategyMetadata;

namespace DawnQuant.App.Services.AShare
{
    public class StrategyMetadataService 
    {

        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StrategyMetadataService()
        {

            _grpcChannelSet = IOCUtil.Container.Resolve<GrpcChannelSet>(); 
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>(); 
            _mapper = IOCUtil.Container.Resolve<IMapper>(); 
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
