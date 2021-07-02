using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Passport
{
    /// <summary>
    /// 认证相关信息
    /// </summary>
    public class AuthContext
    {
        public string IdentityUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}
