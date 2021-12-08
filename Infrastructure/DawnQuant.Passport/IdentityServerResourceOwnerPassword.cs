using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;


namespace DawnQuant.Passport
{
    public class IdentityServerResourceOwnerPassword : IPassportProvider
    {
        public IdentityServerResourceOwnerPassword(AuthContext authContext)
        {

            _identityUrl = authContext.IdentityUrl;
            _clientId = authContext.ClientId;
            _scope = authContext.Scope;
        }
        string _name;
        string _pwd;

        private readonly string _identityUrl;
        private readonly string _clientId;
        private readonly string _scope;

        TokenResponse _tokenResponse;
        UserInfoResponse _userInfoResponse;
        DateTime _acquisitionDateTime = DateTime.Now;

        public bool Login(string name, string pwd )
        {
            _name = name;
            _pwd = pwd;

            return Login();
          
        }

        private bool Login(  )
        {
            var client = new HttpClient();
            var discTask =   client.GetDiscoveryDocumentAsync(_identityUrl);
            discTask.Wait();
            var disc = discTask.Result;

            if (disc.IsError)
            {
                Error+= disc.Error;
                return false;
                
            }

            var tokenResponseTask = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disc.TokenEndpoint,
                ClientId = _clientId,
                UserName = _name,
                Password = _pwd,
                Scope = _scope
            });
            tokenResponseTask.Wait();
            _tokenResponse = tokenResponseTask.Result;

            if (_tokenResponse.IsError)
            {
                Error += _tokenResponse.ErrorDescription;
                return false;
            }
            _acquisitionDateTime = DateTime.Now;

            //获取用户信息
            var uClient = new HttpClient();
            var userInfoTask = uClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disc.UserInfoEndpoint,
                Token = _tokenResponse.AccessToken
            }); 
            userInfoTask.Wait();

            _userInfoResponse = userInfoTask.Result;
            if (_userInfoResponse.IsError)
            {
                Error += _userInfoResponse.Error;
                return false;
            }

            UserName = _name;
            return true;
        }

       

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (_tokenResponse == null || _tokenResponse.IsError)
                {
                    return null;
                }

                //过期时间
                DateTime expiresDateTime = _acquisitionDateTime.AddSeconds(_tokenResponse.ExpiresIn);

                //提前5分钟更新
                if (DateTime.Now.AddMinutes(5) >= expiresDateTime)
                {
                    Login();
                    
                }

                return _tokenResponse.AccessToken;

            }
            
        }
        
        /// <summary>
        /// 用户信息
        /// </summary>
        public IEnumerable<Claim> Claims
        {
            get { return _userInfoResponse?.Claims; }
           
        }

        /// <summary>
        /// User Id
        /// </summary>
        public long UserId {
            get
            {
                if (Claims == null || !Claims.Any())
                {
                    throw new Exception("请先登录系统");
                }
                return long.Parse(Claims.Where(p => p.Type == "sub").SingleOrDefault().Value);

            }
        }

        public string UserName { private set; get; }

        public string Error  { private set; get; }
}
}

