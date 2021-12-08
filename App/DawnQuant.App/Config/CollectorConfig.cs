using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Config
{
    /// <summary>
    /// 采集器参数
    /// </summary>
    public class CollectorConfig
    {

        /// <summary>
        /// A股市场采集数据上传API
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
