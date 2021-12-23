using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using DawnQuant.App.Core.Utils;

namespace DawnQuant.App.WinFormHost
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            niMain.ContextMenuStrip = cmsNotifyIcon;
            serviceCollection = new ServiceCollection();

            serviceCollection.AddBlazorWebView();
            serviceCollection.AddDawnQuantAppServices();

            //配置服务
            serviceCollection.AddSingleton<IConfiguration>((s) =>
            {
                var configurationbuilder = new ConfigurationBuilder()
               .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true, true);
                IConfiguration configuration = configurationbuilder.Build();
                return configuration;
            });

            //日志服务
            string SerilogOutputTemplate = "{NewLine}{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" + new string('-', 100);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("Logs\\log.log", rollingInterval: RollingInterval.Day,
                outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 10)
                .CreateLogger();
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
            serviceCollection.AddSingleton<ILoggerFactory>((c) =>
            {
                return loggerFactory;
            });
            serviceCollection.AddTransient<Microsoft.Extensions.Logging.ILogger>((c) =>
            {
                return loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().ReflectedType.Name);
            });


            //缓存服务
            serviceCollection.AddSingleton<IMemoryCache>((s) =>
            {
                return new MemoryCache(new MemoryCacheOptions { });
            });

            blazorWebView = new BlazorWebView()
            {
                Dock = DockStyle.Fill,
                HostPage = "wwwroot/index.html",

                Services = serviceCollection.BuildServiceProvider(),
               
            };
            blazorWebView.RootComponents.Add<DawnQuant.App.Core.App>("#app");
            Controls.Add(blazorWebView);

            InitializeAsync();


        }

        private BlazorWebView blazorWebView;
        private ServiceCollection serviceCollection;


        private async void InitializeAsync()
        {
            await blazorWebView.WebView.EnsureCoreWebView2Async(null);
            blazorWebView.WebView.WebMessageReceived += WebView_WebMessageReceived;
        }

        private void WebView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.WebMessageAsJson.Contains("MaximizeWindow"))
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Maximized;
            }
            else if(e.WebMessageAsJson.Contains("ExitApp"))
            {
                Environment.Exit(0);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;

        }

        private void miShowMain_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Show();
            this.Focus();
            this.BringToFront();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ShowInTaskbar = true;
                this.Show();
            }
        }
    }
}