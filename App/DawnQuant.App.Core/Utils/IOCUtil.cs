using DawnQuant.App.Core.Config;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.ViewModels;
using DawnQuant.App.Core.ViewModels.AShare.SelfSelStock;
using DawnQuant.Passport;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DawnQuant.App.Core.Utils
{
    /// <summary>
    /// 容器管理
    /// </summary>
    public static class IOCUtil
    {

        public static void AddDawnQuantAppServices(this ServiceCollection serviceCollection)
        {
            //配置信息
            serviceCollection.AddSingleton<AppConfig>((s)=>
            {
                IConfiguration configuration = s.GetService<IConfiguration>();

                AppConfig appConfig = new AppConfig();

                appConfig.AShareApiUrl = configuration["AShareApiUrl"];
                appConfig.IdentityUrl = configuration["IdentityUrl"];
                appConfig.ClientId = configuration["ClientId"];
                appConfig.ClientSecret = configuration["ClientSecret"];
                appConfig.Scope = configuration["Scope"];

                return appConfig;
            });

            //认证信息
            serviceCollection.AddSingleton<AuthContext>(s =>
            {
              var appConfig=  s.GetService<AppConfig>();
                AuthContext authContext = new AuthContext();
                authContext.IdentityUrl = appConfig.IdentityUrl;
                authContext.ClientId = appConfig.ClientId;
                authContext.Scope = appConfig.Scope;
                return authContext;
            });

            serviceCollection.AddSingleton<IPassportProvider>((s)=>
            {
                AuthContext c= s.GetService<AuthContext>();
                return new IdentityServerResourceOwnerPassword(c);
            });


            //注册ASahre服务
            serviceCollection.AddSingleton<AShareDataMaintainService>();
            serviceCollection.AddSingleton<SelfSelService>();
            serviceCollection.AddSingleton<BellwetherService>();
            serviceCollection.AddSingleton<SubjectAndHotService>();
            serviceCollection.AddSingleton<StockPlotDataService>();
            serviceCollection.AddSingleton<StockStrategyService>();
            serviceCollection.AddSingleton<StrategyExecutorService>();
            serviceCollection.AddSingleton<StrategyMetadataService>();
            serviceCollection.AddSingleton<StrategyScheduledTaskService>();
            serviceCollection.AddSingleton<SupportedCategoriesService>();


            //映射转换服务
            serviceCollection.AddAutoMapper();

            //Grpc
            serviceCollection.AddSingleton<GrpcChannelSet>((s) =>
            {
                string url = s.GetService<AppConfig>().AShareApiUrl;
                var channel = new GrpcChannelSet();
                channel.AShareGrpcChannel = GrpcChannel.ForAddress(url);
                return channel;
            });


            //定时任务消息中间件
            serviceCollection.AddSingleton<JobMessageUtil>();


            //注册ViewModel
            serviceCollection.AddSingleton<LoginViewModel>();
            serviceCollection.AddSingleton<SelfSelStockViewModel>();
           // serviceCollection.AddSingleton<SelfSelStockViewModel>();

        }

    }
}
