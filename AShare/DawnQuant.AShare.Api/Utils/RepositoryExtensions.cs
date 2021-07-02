using DawnQuant.AShare.Repository.Abstract;

using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Repository.Impl.EssentialData;
using DawnQuant.AShare.Repository.Impl.StrategyMetadata;
using DawnQuant.AShare.Repository.Impl.UserProfile;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DawnQuant.AShare.Strategy.Executor;

namespace DawnQuant.AShare.Api.Utils
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configration"></param>
        public static void AddUserProfileRepository( this IServiceCollection services, string strCon)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //股票基本信息相关服务
            services.AddDbContext<UserProfileDbContext>(options =>
            {
                options.UseMySql(strCon, MySqlServerVersion.AutoDetect(strCon),b=>
                {
                    b.MigrationsAssembly(migrationsAssembly);
                });
            }, ServiceLifetime.Scoped);


            services.AddScoped<ISelfSelectStockCategoryRepository, EFSelfSelectStockCategoryRepository>();
            services.AddScoped<ISelfSelectStockRepository, EFSelfSelectStockRepository>();

            services.AddScoped<IStockStrategyCategoryRepository, EFStockStrategyCategoryRepository>();
            services.AddScoped<IStockStrategyRepository, EFStockStrategyRepository>();

            services.AddScoped<IStrategyScheduledTaskRepository, EFStrategyScheduledTaskRepository>();
          
        }


        /// <summary>
        /// 策略存储
        /// </summary>
        /// <param name="services"></param>
        /// <param name="strCon"></param>
        public static void AddStrategyRepository(this IServiceCollection services, string strCon)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            ///策略服务
            services.AddDbContext<StrategyDbContext>(options =>
            {
                options.UseMySql(strCon, MySqlServerVersion.AutoDetect(strCon), b =>
                {
                    b.MigrationsAssembly(migrationsAssembly);
                });
            });


            //股票策略数据
            services.AddScoped<ISelectScopeMetadataRepository, EFSelectScopeMetadataRepository>();
            services.AddScoped<ISelectScopeMetadataCategoryRepository, EFSelectScopeMetadataCategoryRepository>();

            services.AddScoped<IFactorMetadataRepository, EFFactorMetadataRepository>();
            services.AddScoped<IFactorMetadataCategoryRepository, EFFactorMetadataCategoryRepository>();

            
        }

        /// <summary>
        /// 基础数据
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configration"></param>
        public static void AddStockEDRepository(this IServiceCollection services, IConfiguration configuration)
        {
            string stock = configuration["ConnectionStrings:DawnQuant.AShare.Stock"];
            string dailytd = configuration["ConnectionStrings:DawnQuant.AShare.Stock.DailyTD"];
            string dailyindicator = configuration["ConnectionStrings:DawnQuant.AShare.Stock.DailyIndicator"];


            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //股票基本信息相关服务
            services.AddDbContext<StockEDDbContext>(options =>
            {
                options.UseMySql(stock, MySqlServerVersion.AutoDetect(stock), b =>
                {
                    b.MigrationsAssembly(migrationsAssembly);
                });
            });

            //股票基础数据
            services.AddScoped<IBasicStockInfoRepository, EFBasicStockInfoRepositoy>();
            services.AddScoped<ICompanyRepository, EFCompanyRepository>();
            services.AddScoped<IIndustryRepository, EFIndustryRepository>();
            services.AddScoped<IStockFormerNameRepository, EFStockFormerNameRepository>();
            services.AddScoped<ITradingCalendarRepository, EFTradingCalendarRepository>();

            //注册交易数据DBContext创建函数
            services.AddSingleton(provider =>
            {
                Func<string, KCycle, IStockTradeDataRepository> func = (ts_code, stockKCycle) =>
                {
                    DbContextOptionsBuilder<StockTradeDataDbContext> builder = new DbContextOptionsBuilder<StockTradeDataDbContext>();

                    if (stockKCycle == KCycle.Day)
                    {
                        builder.UseMySql(dailytd, MySqlServerVersion.AutoDetect(dailytd));
                    }
                    else
                    {
                        throw new NotSupportedException("原始交易数据只支持日线交易数据");
                    }

                    var dbContext = new StockTradeDataDbContext(builder.Options, ts_code, stockKCycle);

                    return new EFStockTradeDataRepository(dbContext, configuration);
                };
                return func;
            });

            //注册每日指标DBContext创建函数
            services.AddSingleton(provider =>
            {
                Func<string, IStockDailyIndicatorRepository> func = ts_code =>
                {
                    DbContextOptionsBuilder<StockDailyIndicatorDbContext> builder = new DbContextOptionsBuilder<StockDailyIndicatorDbContext>();
                    builder.UseMySql(dailyindicator, MySqlServerVersion.AutoDetect(dailyindicator));

                    var dbContext = new StockDailyIndicatorDbContext(builder.Options, ts_code);

                    return new EFStockDailyIndicatorRepository(dbContext, configuration);
                };
                return func;
            });

        }

    }
}
