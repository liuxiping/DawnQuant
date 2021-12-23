using Autofac;
using DawnQuant.App.Models;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.Passport;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Services.AShare
{
    internal class SettingService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
       


        public SettingService()
        {

            _grpcChannelSet = IOCUtil.Container.Resolve<GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
           
        }

        public Setting GetSetting()
        {
            var client = new SettingApi.SettingApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

           var response= client.GetSettingByUser(new GetSettingByUserRequest { UserId = _passportProvider.UserId }, meta);

            if(!string.IsNullOrEmpty(response.Setting))
            {
                return Setting.Instance(response.Setting);
            }
            else
            {
                return null;
            }


        }

        public void SaveSetting(string setting)
        {
            var client = new SettingApi.SettingApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            client.SaveSetting(new SaveSettingRequest
            {
                UserId = _passportProvider.UserId,
                Setting = setting
            }, meta);

        }
    }
}
