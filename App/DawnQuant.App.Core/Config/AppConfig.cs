
namespace DawnQuant.App.Core.Config
{
    /// <summary>
    /// 应用程序参数
    /// </summary>
    public class AppConfig
    {

        /// <summary>
        /// Api 地址
        /// </summary>
        public string AShareApiUrl { get; set; }


        /// <summary>
        /// 资源认证服务器Url
        /// </summary>
        public string IdentityUrl { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 权限作用域
        /// </summary>
        public string Scope { get; set; }

    }
}
