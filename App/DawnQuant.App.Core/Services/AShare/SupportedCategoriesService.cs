using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Models.AShare.Strategy.Executor.Common;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Grpc.Core;

namespace DawnQuant.App.Core.Services.AShare
{
    public class SupportedCategoriesService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;
        public SupportedCategoriesService(GrpcChannelSet grpcChannelSet,
       IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取当前用户的自选分类
        /// </summary>
        /// <returns></returns>
        public List<SupportedCategory> GetSupportedCategories()
        {
            List<SupportedCategory> categories = new List<SupportedCategory>();
            var client = new SelfSelectStockCategoryApi.SelfSelectStockCategoryApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);
            var dtos = client.GetStockCategoriesByUser(new GetStockCategoriesByUserRequest { UserId = _passportProvider.UserId }, meta);
            foreach (var c in dtos.Entities)
            {
                categories.Add(new SupportedCategory { CategoryId = c.Id, Name = c.Name });
            }
            return categories;
        }
    }
}
