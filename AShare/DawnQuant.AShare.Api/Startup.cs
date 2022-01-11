using DawnQuant.AShare.Api.Data;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.AShare.Api.StrategyMetadata;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Api.Utilities;
using DawnQuant.AShare.Api.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string cronExpression = Configuration["TaskJobCron"];
            string userProfileCon = Configuration["ConnectionStrings:DawnQuant.AShare.App"];
            string strategyCon = Configuration["ConnectionStrings:DawnQuant.AShare.App"];

            //定时任务
            services.AddQuartzJob(cronExpression);

            //对象映射
            services.AddMapper();

            //基础数据
            services.AddStockEDRepository(Configuration);
            ///用户相关数据
            services.AddUserProfileRepository(userProfileCon);

            //策略相关
            services.AddStrategyRepository(strategyCon);
            
            //策略相关对象
            services.AddStrategyExecutorContext();

            //认证
            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = Configuration["IdentityUrl"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,

                };
            });

            //授权数据策略
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DataApi", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "DawnQuant.AShare.DataApi");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AppApi", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "DawnQuant.AShare.AppApi");
                });
            });

            //设置GRPC消息大小
            services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = 1024 * 1024 * 50;
                options.MaxSendMessageSize = 1024 * 1024 * 50;
            });

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {

                //基础数据服务
                endpoints.MapGrpcService<BasicStockInfoService>();
                endpoints.MapGrpcService<CompanyService>();
                endpoints.MapGrpcService<TradingCalendarService>();
                endpoints.MapGrpcService<IndustryService>();
                endpoints.MapGrpcService<StockTradeDataService>();
                endpoints.MapGrpcService<StockDailyIndicatorService>();
                endpoints.MapGrpcService<HolderService>();
                endpoints.MapGrpcService<BellwetherService>();
                endpoints.MapGrpcService<SubjectAndHotService>();
                endpoints.MapGrpcService<PerformanceForecastService>();
                endpoints.MapGrpcService<FutureEventOfSubjectService>();

                //同花顺指数
                endpoints.MapGrpcService<THSIndexService>();
                endpoints.MapGrpcService<THSIndexMemberService>();
                endpoints.MapGrpcService<THSIndexTradeDataService>();



                //策略服务基础数据
                endpoints.MapGrpcService<SelectScopeMetadataService>();
                endpoints.MapGrpcService<SelectScopeMetadataCategoryService>();
                endpoints.MapGrpcService<FactorMetadataService>();
                endpoints.MapGrpcService<FactorMetadataCategoryService>();

                //执行策略
                 endpoints.MapGrpcService<StrategyExecutorService>();


                //用户配置相关服务
                endpoints.MapGrpcService<SelfSelectStockCategoryService>();
                endpoints.MapGrpcService<SelfSelectStockService>();
                endpoints.MapGrpcService<BellwetherStockCategoryService>();
                endpoints.MapGrpcService<BellwetherStockService>();
                endpoints.MapGrpcService<SubjectAndHotStockCategoryService>();
                endpoints.MapGrpcService<SubjectAndHotStockService>();
                endpoints.MapGrpcService<StockStrategyCategoryService>();
                endpoints.MapGrpcService<StrategyScheduledTaskService>();
                endpoints.MapGrpcService<StockStrategyService>();
                endpoints.MapGrpcService<SettingApiService>();


                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            }).UseAuthorization();


            //初始化基础数据
            SeedData.InitStockStrategyMetadata(app);
        }
    }
}
