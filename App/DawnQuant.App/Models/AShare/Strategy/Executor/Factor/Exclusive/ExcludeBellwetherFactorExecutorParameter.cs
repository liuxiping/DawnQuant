using Autofac;
using DawnQuant.App.Converter;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Factor
{
    public class ExcludeBellwetherFactorExecutorParameter : ExecutorParameter
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


        public override void Initialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                IPassportProvider passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
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
