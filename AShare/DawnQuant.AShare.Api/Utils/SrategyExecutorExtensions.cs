using DawnQuant.AShare.Strategy.Executor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.Utils
{
    public static class SrategyExecutorExtensions
    {

        /// <summary>
        /// 注册选股范围与策略执行器类型
        /// </summary>
        /// <param name="services"></param>
        public static void AddStrategyExecutorContext(this IServiceCollection services)
        {
            
            var dir = Environment.CurrentDirectory;
            Assembly asm = Assembly.LoadFrom("DawnQuant.AShare.Strategy.dll");
            foreach (var t in asm.GetTypes())
            {

                if (t != typeof(ISelectScopeExecutor) && t != typeof(IFactorExecutor) &&
                    (t.IsAssignableTo(typeof(IFactorExecutor)) ||
                    t.IsAssignableTo(typeof(ISelectScopeExecutor)) )
                    )
                {
                    services.AddScoped(t);
                }
            }
        }
    }
}
