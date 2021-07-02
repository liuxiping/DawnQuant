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
            //        // ��־�����������ռ������ Microsoft ��ͷ��������־�����С����Ϊ Information
            //        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //        .Enrich.FromLogContext()
            //        // ������־���������̨
            //       // .WriteTo.Console()
            //        // ������־������ļ����ļ��������ǰ��Ŀ�� logs Ŀ¼��
            //        // �ռǵ���������Ϊÿ��
            //        .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day);
            //    // ���� logger
            //});
    }
}
