using Autofac;
using DawnQuant.DataCollector.Collectors.AShare;
using DawnQuant.DataCollector.Config;
using DawnQuant.Passport;
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

namespace DawnQuant.DataCollector.Utils
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

            //消息服务
            builder.RegisterInstance(new JobMessageUtil()).SingleInstance();


            //配置信息
            CollectorConfig config = new CollectorConfig();
            config.TushareToken = ConfigurationManager.AppSettings["TushareToken"];
            config.TushareUrl = ConfigurationManager.AppSettings["TushareUrl"];
            config.DailyStockTradeDataCollectorThreadCount = int.Parse(ConfigurationManager.AppSettings["DailyStockTradeDataCollectorThreadCount"]);
            config.StockDailyIndicatorCollectorThreadCount = int.Parse(ConfigurationManager.AppSettings["StockDailyIndicatorCollectorThreadCount"]);
            config.IndustryCollectorThreadCount =int.Parse( ConfigurationManager.AppSettings["IndustryCollectorThreadCount"]);
            config.AShareApiUrl = ConfigurationManager.AppSettings["AShareApiUrl"];

            config.IdentityUrl = ConfigurationManager.AppSettings["IdentityUrl"];
            config.ClientId = ConfigurationManager.AppSettings["ClientId"];
            config.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            config.Scope = ConfigurationManager.AppSettings["Scope"];

            config.DailyTradeDataTaskCron = ConfigurationManager.AppSettings["DailyTradeDataTaskCron"];
            config.DailyTradeDataTask230Cron = ConfigurationManager.AppSettings["230DailyTradeDataTaskCron"];

            config.StockDailyIndicatorTaskCron = ConfigurationManager.AppSettings["StockDailyIndicatorTaskCron"];
            builder.RegisterInstance(config).SingleInstance();

            //A股数据采集器
            builder.RegisterType<BasicStockInfoCollector>().SingleInstance();
            builder.RegisterType<CompanyCollector>().SingleInstance();
            builder.RegisterType<DailyStockTradeDataCollector>().SingleInstance();
            builder.RegisterType<IncrementalDataCollector>().SingleInstance();
            builder.RegisterType<IndustryCollector>().SingleInstance();
            builder.RegisterType<StockDailyIndicatorCollector>().SingleInstance();
            builder.RegisterType<StockFormerNameCollector>().SingleInstance();
            builder.RegisterType<TradingCalendarCollector>().SingleInstance();


            //认证信息
            AuthContext authContext = new AuthContext();
            authContext.IdentityUrl = config.IdentityUrl;
            authContext.ClientId = config.ClientId;
            authContext.ClientSecret = config.ClientSecret;
            authContext.Scope = config.Scope;
            builder.RegisterInstance(authContext).SingleInstance();
            builder.RegisterType<IdentityServerClientCredentials>().As<IPassportProvider>().SingleInstance();

            Container = builder.Build();



        }

    }
}
