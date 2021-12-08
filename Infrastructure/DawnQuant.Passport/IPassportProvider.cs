using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Passport
{
    /// <summary>
    /// 认证
    /// </summary>
    public interface IPassportProvider
    {
        string AccessToken { get; }
        IEnumerable<Claim> Claims { get; }
        bool Login(string name, string pwd);
        long UserId { get; }
        string UserName { get; }
        string Error { get; }


    }
}
