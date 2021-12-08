using DawnQuant.DataCollector.Core;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using DawnQuant.DataCollector.Core.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DawnQuant.DataCollector.WinFormHost
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            niMain.ContextMenuStrip = cmsNotifyIcon;

            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddBlazorWebView();
            _serviceCollection.AddDawnQuantDataCollectorServices();

            //配置服务
            _serviceCollection.AddSingleton<IConfiguration>((s) =>
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
            _serviceCollection.AddSingleton<ILoggerFactory>((c) =>
            {
                return loggerFactory;
            });
            _serviceCollection.AddTransient<Microsoft.Extensions.Logging.ILogger>((c) =>
            {
                return loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().ReflectedType.Name);
            });

            _blazorWebView = new BlazorWebView()
            {
                Dock = DockStyle.Fill,
                HostPage = "wwwroot/index.html",

                Services = _serviceCollection.BuildServiceProvider(),
            };

            _blazorWebView.RootComponents.Add<App>("#app");
         
            Controls.Add(_blazorWebView);
           
        }

        private BlazorWebView _blazorWebView;
        private ServiceCollection _serviceCollection;

        private FormWindowState _formWindowState = FormWindowState.Normal;

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _formWindowState = this.WindowState;
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;

        }

        private void miShowMain_Click(object sender, EventArgs e)
        {
            this.WindowState = _formWindowState;
            this.ShowInTaskbar = true;
            this.Show();
           
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.WindowState = _formWindowState;
                this.ShowInTaskbar = true;
                this.Show();
            }
        }
    }
}