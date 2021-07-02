using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace DawnQuant.Passport.Data
{
    public static class DawnQuantIdentityServerConfig
    {

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),

            };

        public static IEnumerable<ApiScope> ApiScopes =>
         new List<ApiScope>
         {
             //A股市场
            new ApiScope("DawnQuant.AShare.DataApi", "DawnQuant.AShare.DataApi"),
            new ApiScope("DawnQuant.AShare.AppApi", "DawnQuant.AShare.AppApi"),
         };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                //数据采集程序
                new Client
                {
                    ClientId = "DawnQuant.DataCollector",


                    AllowedGrantTypes = GrantTypes.ClientCredentials,


                    ClientSecrets =
                    {
                        new Secret("lxp48218736.".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "DawnQuant.AShare.DataApi" }
                },
           

                //业务程序
                new Client
                {
                    ClientId = "DawnQuant.App",
                    ClientSecrets = { new Secret("lxp48218736.".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    RequireClientSecret=false,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "DawnQuant.AShare.DataApi",
                        "DawnQuant.AShare.AppApi",
                       
                    }
                }


            };
    }
}