using AutoMapper;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class SettingApiService:SettingApi.SettingApiBase
    {

        private readonly ILogger<SettingApiService> _logger;
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;


        public SettingApiService(ILogger<SettingApiService> logger, IMapper mapper,
        ISettingRepository settingRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _settingRepository = settingRepository;

        }

        public override Task<GetSettingByUserResponse> GetSettingByUser(GetSettingByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSettingByUserResponse response = new GetSettingByUserResponse();
                var s= _settingRepository.Entities.Where(p => p.UserId == request.UserId).FirstOrDefault();

                if(s != null && !string.IsNullOrEmpty(s.Content))
                {
                    response.Setting = s?.Content;
                }
               
               else
                {
                    response.Setting = "";
                }

                return response;
            });
        }

        public override Task<Empty> SaveSetting(SaveSettingRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                var s = new Setting();
                s.UserId = request.UserId;
                s.Content = request.Setting;
                _settingRepository.Save(s);
                return new Empty();

            });
        }
       
    }
}
