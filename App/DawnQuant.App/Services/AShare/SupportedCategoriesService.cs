using AutoMapper;
using DawnQuant.App.Models;
using DawnQuant.App.Models.AShare.Strategy.Executor.Common;
using DawnQuant.App.Models.AShare.Strategy.Executor.SelectScope;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Services.AShare
{
    public class SupportedCategoriesService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;

        public SupportedCategoriesService(GrpcChannelSet grpcChannelSet, IPassportProvider passportProvider)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;

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
