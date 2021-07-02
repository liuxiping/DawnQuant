using DawnQuant.Passport;
using DawnQuant.Passport.Data;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using DawnQuant.Passport.Entities;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;


namespace DawnQuant.Passport.Data
{
    public class SeedData
    {
        public static void EnsureIdentitySeedData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                ILogger< SeedData> logger = scope.ServiceProvider.GetService<ILoggerFactory>()
                    .CreateLogger<SeedData>();

                var context = scope.ServiceProvider.GetService<DawnQuantIdentityDbContext>();

                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<DawnQuantIdentityUser>>();
                var user = userMgr.FindByNameAsync("liuxiping").Result;
                if (user == null)
                {
                    user = new DawnQuantIdentityUser
                    {
                        UserName = "liuxiping",
                        Email = "liuxiping.cn@163.com",
                        EmailConfirmed = true,
                        PhoneNumber = "18374068853",
                        PhoneNumberConfirmed = true,
                        NickName = "轻松一笑"
                        

                    };
                    var result = userMgr.CreateAsync(user, "lxp48218736.").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(user, new Claim[]{
                            new Claim("AShare","AShare")
                        }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    logger.LogInformation("liuxiping created");
                }
                else
                {
                    logger.LogInformation("liuxiping already exists");
                }
               
            }
        }


        public static void InitIdentityServerSeedData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                ILogger<SeedData> logger = scope.ServiceProvider.GetService<ILoggerFactory>()
                  .CreateLogger<SeedData>();

                scope.ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in DawnQuantIdentityServerConfig.Clients)
                    {
                        context.Clients.Add(client.ToEntity());

                        logger.LogInformation($"{client.ClientName} created ");
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in DawnQuantIdentityServerConfig.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                        logger.LogInformation($"{resource.Name} created ");
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var apiScope in DawnQuantIdentityServerConfig.ApiScopes)
                    {
                        context.ApiScopes.Add(apiScope.ToEntity());
                        logger.LogInformation($"{apiScope.Name} created ");
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}

