using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Passport
{
    public class IdentityServerClientCredentials : IPassportProvider
    {
        public IEnumerable<Claim> Claims => throw new NotSupportedException();

        public bool Login(string name, string pwd)
        {
            throw new NotSupportedException();
        }

        public IdentityServerClientCredentials( AuthContext authContext)
        {
          
            _identityUrl = authContext.IdentityUrl;
            _clientId = authContext.ClientId;
            _clientSecret = authContext.ClientSecret;
            _scope = authContext.Scope;
        }



        string _identityUrl;
        string _clientId;
        string _clientSecret;
        string _scope;

        TokenResponse _tokenResponse;
        DateTime _acquisitionDateTime = DateTime.Now;
        DiscoveryDocumentResponse _discoveryDocumentResponse;

        public string AccessToken
        {
            get
            {
                if (_discoveryDocumentResponse == null)
                {
                    var discclient = new HttpClient();
                    var discTask = discclient.GetDiscoveryDocumentAsync(_identityUrl);
                    discTask.Wait();

                    if (discTask.Result.IsError)
                    {
                        throw discTask.Result.Exception;
                    }
                    _discoveryDocumentResponse = discTask.Result;
                }

                DateTime expiresDateTime = DateTime.Now;

                if (_tokenResponse != null)
                {
                    expiresDateTime = _acquisitionDateTime.AddSeconds(_tokenResponse.ExpiresIn);
                }
                

                //提前5分钟更新
                if (_tokenResponse == null || _tokenResponse.IsError ||
                    DateTime.Now.AddMinutes(5) >= expiresDateTime)
                {

                    var client = new HttpClient();
                    var tokenResponseTask = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = _discoveryDocumentResponse.TokenEndpoint,
                        ClientId = _clientId,
                        ClientSecret = _clientSecret,
                        Scope = _scope
                    }); ;

                    tokenResponseTask.Wait();
                    _tokenResponse = tokenResponseTask.Result;
                    if (_tokenResponse.IsError)
                    {
                        throw _tokenResponse.Exception;
                    }
                    _acquisitionDateTime = DateTime.Now;
                }

               
                return _tokenResponse.AccessToken;
            }
        }

        public long UserId => throw new NotImplementedException();
    }
}

