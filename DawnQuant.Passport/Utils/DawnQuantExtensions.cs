using DawnQuant.Passport.Data;
using DawnQuant.Passport.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;

namespace DawnQuant.Passport.Utils
{
    public static class DawnQuantExtensions
    {

        /// <summary>
        /// 注册配置 Identity 相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configration"></param>
        public static void AddDawnQuantIdentity(this IServiceCollection services, string strCon)
        {
            services.AddDbContext<DawnQuantIdentityDbContext>(options =>
            {

                options.UseMySql(strCon,
                    MySqlServerVersion.AutoDetect(strCon), builder =>
                    {

                    });
            });



            services.AddDefaultIdentity<DawnQuantIdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;


            })
            .AddEntityFrameworkStores<DawnQuantIdentityDbContext>();

        }



        /// <summary>
        /// 注册 Identity Server 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="strCon"></param>
        public static void AddDawnQuantIdentityServer(this IServiceCollection services, string strCon)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                // .AddDeveloperSigningCredential()
                //  .AddDataProtection()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseMySql(strCon, MySqlServerVersion.AutoDetect(strCon),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseMySql(strCon, MySqlServerVersion.AutoDetect(strCon),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<DawnQuantIdentityUser>();
            // .AddProfileService<ProfileService>();

        }
    }
}
