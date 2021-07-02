using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;


namespace DawnQuant.Passport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

            //.UseSerilog((context, configuration) =>
            //{
            //    configuration
            //        .MinimumLevel.Information()
            //        // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
            //        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //        .Enrich.FromLogContext()
            //        // 配置日志输出到控制台
            //       // .WriteTo.Console()
            //        // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
            //        // 日记的生成周期为每天
            //        .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day);
            //    // 创建 logger
            //});
    }
}
