
using DawnQuant.AShare.Api.Quartz;
using DawnQuant.AShare.Api.Quartz.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.Utilities
{
    public static class QuartzJobExtensions
    {
        public static void AddQuartzJob(this IServiceCollection services, string cronExpression)
        {


            services.AddSingleton<IJobFactory, SingletonJobFactory>();

            services.AddTransient<StrategyScheduledTaskJob>();

            services.AddSingleton(new JobSchedule(
                jobType: typeof(StrategyScheduledTaskJob),
                cronExpression: cronExpression));


            //services.AddSingleton(new JobSchedule(
            //   jobType: typeof(StrategyScheduledTaskJob),
            //   cronExpression: "0/5 * * * * ?"));

            //"0/5 * * * * ?"
            //0 00 23 ? * MON-FRI

            services.AddHostedService<QuartzHostedService>();
        }
    }
}
