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

        public override Task<GetSettingsByUserResponse> GetSettingsByUser(GetSettingsByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSettingsByUserResponse response = new GetSettingsByUserResponse();
                var data = _mapper.Map<IEnumerable<SettingDto>>(_settingRepository.Entities.Where(p => p.UserId == request.UserId));
                response.Entities.AddRange(data);
                return response;
            });
        }

        public override Task<SaveSettingsResponse> SaveSettings(SaveSettingsRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveSettingsResponse response = new SaveSettingsResponse();
                var data = _mapper.Map<IEnumerable<Setting>>(request.Entities);
                var rdata = _mapper.Map<IEnumerable<SettingDto>>(_settingRepository.Save(data));
                response.Entities.AddRange(rdata);
                return response;
            });
        }

        public override Task<Empty> DelSettingsByUser(DelSettingsByUserRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                var data = _settingRepository.Entities.Where(p => p.UserId == request.UserId);
                _settingRepository.Delete(data);
                return new Empty();
            });

        }



        public override Task<GetSpecifySettingResponse> GetSpecifySetting(GetSpecifySettingRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetSpecifySettingResponse response = new GetSpecifySettingResponse();
                var s = _mapper.Map<SettingDto>(_settingRepository.Entities.
                    Where(p => p.UserId == request.UserId && p.Key == request.Key)
                    .FirstOrDefault());


                response.Setting = s;


                return response;
            });
        }

        public override Task<SaveSpecifySettingResponse> SaveSpecifySetting(SaveSpecifySettingRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveSpecifySettingResponse response = new SaveSpecifySettingResponse();
                var s = _mapper.Map<Setting>(request.Setting);
                var rs = _mapper.Map<SettingDto>(_settingRepository.Save(s));
                response.Setting = rs;
                return response;
            });
        }

        public override Task<Empty> DelSpecifySetting(DelSpecifySettingRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                var data = _settingRepository.Entities.Where(p => p.UserId == request.UserId &&
                p.Key==request.Key);
                _settingRepository.Delete(data);
                return new Empty();
            });
        }
    }
}
