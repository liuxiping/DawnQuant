using Autofac;
using DawnQuant.App.Config;
using DawnQuant.App.Models;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace DawnQuant.App.Utils
{
    /// <summary>
    /// 容器管理
    /// </summary>
    public static class IOCUtil
    {
        public static IContainer Container { get; set; }

        static IOCUtil()
        {
            var builder = new ContainerBuilder();

            //日志服务
            string SerilogOutputTemplate = "{NewLine}{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" + new string('-', 100);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("Logs\\log.log", rollingInterval: RollingInterval.Day,
                outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 10)
                .CreateLogger();

            builder.Register<ILoggerFactory>((c) =>
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog();
                });
                return loggerFactory;
            }).SingleInstance();

            builder.Register<ILogger>((c) =>
            {
                ILoggerFactory loggerFactory = IOCUtil.Container.Resolve<ILoggerFactory>();
                return loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().ReflectedType.Name);

            });

            builder.RegisterGeneric(typeof(LoggerUtil<>)).As(typeof(ILogger<>)).SingleInstance();

            //配置信息
            CollectorConfig config = new CollectorConfig();
            config.AShareApiUrl = ConfigurationManager.AppSettings["AShareApiUrl"];
            config.IdentityUrl = ConfigurationManager.AppSettings["IdentityUrl"];
            config.ClientId = ConfigurationManager.AppSettings["ClientId"];
            config.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            config.Scope = ConfigurationManager.AppSettings["Scope"];
            builder.RegisterInstance(config).SingleInstance();

            
            //认证信息
            AuthContext authContext = new AuthContext();
            authContext.IdentityUrl = config.IdentityUrl;
            authContext.ClientId = config.ClientId;
            authContext.Scope = config.Scope;
            builder.RegisterInstance(authContext).SingleInstance();
            builder.RegisterType<IdentityServerResourceOwnerPassword>().As<IPassportProvider>().SingleInstance();


            //注册ASahre服务
            builder.RegisterType<AShareDataMaintainService>().SingleInstance();
            builder.RegisterType<SelfSelService>().SingleInstance();
            builder.RegisterType<BellwetherService>().SingleInstance();
            builder.RegisterType<SubjectAndHotService>().SingleInstance();
            builder.RegisterType<PlotDataService>().SingleInstance();
            builder.RegisterType<StockStrategyService>().SingleInstance();
            builder.RegisterType<StrategyExecutorService>().SingleInstance();
            builder.RegisterType<StrategyMetadataService>().SingleInstance();
            builder.RegisterType<StrategyScheduledTaskService>().SingleInstance();
            builder.RegisterType<SupportedCategoriesService>().SingleInstance();

            builder.RegisterType<SettingService>().SingleInstance();

            builder.RegisterType<THSIndexService>().SingleInstance();

            

            //映射转换服务
            builder.AddAutoMapper();

            //Grpc
            builder.Register<GrpcChannelSet>((c) =>
            {
                var channel = new GrpcChannelSet();
                channel.AShareGrpcChannel = GrpcChannel.ForAddress(config.AShareApiUrl);
                return channel;
            }).SingleInstance();

            //缓存服务
            builder.Register<IMemoryCache>((c) =>
            {
                return new MemoryCache(new MemoryCacheOptions { });
            }).SingleInstance();


            //消息中间件
            builder.RegisterType<MessageUtil>().SingleInstance();


            Container = builder.Build();



        }

    }
}
