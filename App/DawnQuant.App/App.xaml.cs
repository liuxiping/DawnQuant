using DawnQuant.App.Utils;
using DevExpress.Xpf.Core;
using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Microsoft.Extensions.Logging;
using System.Text;
using DawnQuant.App.Models.AShare.UserProfile;

namespace DawnQuant.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;

            _logger = IOCUtil.Container.Resolve<ILogger<App>>();

        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public static Setting AShareSetting { get; set; } = null;

        ILogger _logger = null;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
         
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                StringBuilder sbEx = new StringBuilder();
                if (e.IsTerminating)
                {
                    sbEx.Append("非UI线程发生致命错误\r\n");
                }
                sbEx.Append("非UI线程异常：");
                if (e.ExceptionObject is Exception)
                {
                    sbEx.Append(((Exception)e.ExceptionObject).Message);
                }
                else
                {
                    sbEx.Append(e.ExceptionObject);
                }

                string error=sbEx.ToString();

                _logger.LogError(error);
                DXMessageBox.Show(error, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception ex)
            {
                string error = $"应用程序发生致命错误！：{ex.Message}";
                DXMessageBox.Show(error, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Task 线程未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            try
            {
                //设置该异常已察觉（这样处理后就不会引起程序崩溃）
                e.SetObserved();
                string error = $"Task线程异常：{e.Exception.Message}";
                _logger.LogError(error);
                DXMessageBox.Show(error, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);


            }
            catch (Exception ex)
            {
                string error = $"应用程序发生致命错误！：{ex.Message}";
                DXMessageBox.Show(error, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// UI线程没有捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                string error = $"UI线程发生致命错误！：{e.Exception.Message}";
                _logger.LogError(error);
                DXMessageBox.Show(error,"温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                string error = $"应用程序发生致命错误！：{ex.Message}";
                DXMessageBox.Show(error, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

      
    }
}
