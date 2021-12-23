using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.SelectScope
{
    public class BellwetherExecutorParameter : ExecutorParameter
    {

        /// <summary>
        /// 用户ID
        /// </summary>
        [DisplayName("用户ID")]
        [ReadOnly(true)]
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        [ReadOnly(true)]
        
        public string UserName { get; set; }


        public override void Initialize(string json, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(json))
            {
                IPassportProvider passportProvider = serviceProvider.GetService<IPassportProvider>();
                UserId = passportProvider.UserId;
                UserName = passportProvider.UserName;
            }
            else
            {
                base.Initialize(json);
            }

        }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
