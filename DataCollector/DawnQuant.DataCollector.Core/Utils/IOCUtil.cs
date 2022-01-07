using DawnQuant.DataCollector.Core.Collectors.AShare;
using DawnQuant.DataCollector.Core.Config;
using DawnQuant.DataCollector.Core.ViewModels.AShare;
using DawnQuant.Passport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DawnQuant.DataCollector.Core.Utils
{
    /// <summary>
    /// 容器管理
    /// </summary>
    public static class IOCUtil
    {

        public static void AddDawnQuantDataCollectorServices(this ServiceCollection serviceCollection)
        {

            //消息服务
            serviceCollection.AddSingleton<JobMessageUtil>();


            serviceCollection.AddSingleton<CollectorConfig>((s) =>
            {

                var configuration = s.GetService<IConfiguration>();

                //配置信息
                CollectorConfig config = new CollectorConfig();
                config.TushareToken = configuration["TushareToken"];
                config.TushareUrl = configuration["TushareUrl"];
                config.DailyStockTradeDataCollectorThreadCount = int.Parse(configuration["DailyStockTradeDataCollectorThreadCount"]);
                config.StockDailyIndicatorCollectorThreadCount = int.Parse(configuration["StockDailyIndicatorCollectorThreadCount"]);
                config.IndustryCollectorThreadCount = int.Parse(configuration["IndustryCollectorThreadCount"]);
                config.AShareApiUrl = configuration["AShareApiUrl"];

                config.IdentityUrl = configuration["IdentityUrl"];
                config.ClientId = configuration["ClientId"];
                config.ClientSecret = configuration["ClientSecret"];
                config.Scope = configuration["Scope"];

                config.InDataFromTushareTaskCron = configuration["InDataFromTushareTaskCron"];
                config.InDTDFromSinaTaskCron = configuration["InDTDFromSinaTaskCron"];


                //龙头股关键词
                config.BellwetherKeyWords = new List<string>();
                var keys=configuration["BellwetherKeyWords"].Split(",").ToList(); 
                foreach (var key in keys)
                {
                    config.BellwetherKeyWords.Add(key.Trim());
                }

                return config;

            });


            //A股数据采集器
            serviceCollection.AddSingleton<BasicStockInfoCollector>();
            serviceCollection.AddSingleton<CompanyCollector>();
            serviceCollection.AddSingleton<IndustryCollector>();
            serviceCollection.AddSingleton<HolderCollector>();
            serviceCollection.AddSingleton<TradingCalendarCollector>();
            serviceCollection.AddSingleton<BellwetherAnalyseCollector>();
            serviceCollection.AddSingleton<PerformanceForecastCollector>();


            serviceCollection.AddSingleton<DailyStockTradeDataCollector>();
            serviceCollection.AddSingleton<StockDailyIndicatorCollector>();
            serviceCollection.AddSingleton<IncrementalDataCollector>();


            serviceCollection.AddSingleton<SubjectAndHotCollector>();


            //认证信息


            serviceCollection.AddSingleton<AuthContext>((s) =>
            {
                var config = s.GetService<CollectorConfig>();
                AuthContext authContext = new AuthContext();
                authContext.IdentityUrl = config.IdentityUrl;
                authContext.ClientId = config.ClientId;
                authContext.ClientSecret = config.ClientSecret;
                authContext.Scope = config.Scope;

                return authContext;
            });
            serviceCollection.AddSingleton<IPassportProvider, IdentityServerClientCredentials>();

            //注册ViewModel
            serviceCollection.AddSingleton<HistoryDataViewModel>();
            serviceCollection.AddSingleton<IncrementDataViewModel>();
            serviceCollection.AddSingleton<SubjectAndHotViewModel>();

            
        }

    }
}
