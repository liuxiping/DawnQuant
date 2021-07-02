using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })

            .ConfigureAppConfiguration(builder =>
            {
                builder.AddXmlFile("Config/FinancialDbSql.xml");
            });
            //.UseSerilog((context, configuration) =>
            //{
            //    configuration
            //        .MinimumLevel.Information()
            //         ��־�����������ռ������ Microsoft ��ͷ��������־�����С����Ϊ Information

            //        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)

            //        .Enrich.FromLogContext()
            //         ������־���������̨
            //        .WriteTo.Console()
            //         ������־������ļ����ļ��������ǰ��Ŀ�� logs Ŀ¼��
            //         �ռǵ���������Ϊÿ��
            //        .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day);
            //});


    }
}
