using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Models;

namespace DawnQuant.App.ViewModels.AShare
{
    public class AShareMainViewModel : ViewModelBase
    {
        //private readonly IPassportProvider _passportProvider;
        AShareDataMaintainService _aShareDataMaintainService;
        MessageUtil _msgNotify;

        public AShareMainViewModel()
        {
            //_passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _msgNotify = IOCUtil.Container.Resolve<MessageUtil>();


            Initialize();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            //计划任务执行消息

            _msgNotify.DataUpdateScheduledTaskJobCompleted += Notify_DataUpdateScheduledTaskJobCompleted;

            _msgNotify.StrategyScheduledTaskCompleted += Notify_StrategyScheduledTaskCompleted;


            //定时任务
            Task.Run(() =>
            {
                //开启客户端执行策略任务
                TaskUtil.StartStrategyScheduledTask();

                //开启后台自动更新数据
                TaskUtil.StartDataUpdateScheduledTask();

            });

            //后台更新数据
            Task.Run(async () =>
            {
                //延时20秒后下载数据
                await Task.Delay(1000 * 20);
                
                DateTime inDataUpdateTime = new DateTime(DateTime.Now.Year,
                    DateTime.Now.Month, DateTime.Now.Day, 17, 20, 0);

                if (_aShareDataMaintainService.IsOpen() && !App.IsUpdateAllAShareData &&
                (AppLocalConfig.Instance.LastUpdateAllDataDateTime == null || AppLocalConfig.Instance.LastUpdateAllDataDateTime <= inDataUpdateTime))
                {

                    if (!App.IsDataUpdateScheduledTaskJob)
                    {
                     //   DownloadAllAShareData();
                    }
                }
                
            });

        }

        /// <summary>
        ///下载交易数据
        /// </summary>
        private void DownloadAllAShareData()
        {
            _aShareDataMaintainService.DownLoadStockDataProgress += (complete, total) =>
            {
                _msgNotify.OnDownloadAShareDataProgress(complete, total);
            };

            App.IsUpdateAllAShareData = true;


            _aShareDataMaintainService.DownLoadStockData(1);

            App.IsUpdateAllAShareData = false;

            AppLocalConfig.Instance.LastUpdateAllDataDateTime = DateTime.Now;

            //保存配置
            AppLocalConfig.Instance.Save();

            _msgNotify.OnDownloadAShareDataComplete(true);

            Notify_DataUpdateScheduledTaskJobCompleted();

        }


        /// <summary>
        /// 自动更新数据完成 通知用户
        /// </summary>
        private void Notify_DataUpdateScheduledTaskJobCompleted()
        {
            DispatcherService.Invoke(() =>
            {
                ShowDataUpdateScheduledTaskCompletedNofify();
            });
            

        }

        IDispatcherService DispatcherService
        {
            get
            {
                //return ServiceContainer.Default.GetService<IDispatcherService>();

                return GetService<IDispatcherService>();
            }
        }

        /// <summary>
        /// 计划任务执行完成 通知用户
        /// </summary>
        /// <param name="obj"></param>
        private void Notify_StrategyScheduledTaskCompleted(Models.AShare.UserProfile.StrategyScheduledTask task)
        {

            DispatcherService.Invoke(() =>
            {
                ShowStrategyScheduledTaskCompletedNofify(task);
            });


        }


        #region notification
        public INotificationService NotificationService
        {
            get
            {
                return GetService<INotificationService>();
            }
        }


        /// <summary>
        /// 显示计划任务完成通知
        /// </summary>
        public void ShowStrategyScheduledTaskCompletedNofify(Models.AShare.UserProfile.StrategyScheduledTask task)
        {
            ImageSource image = new BitmapImage(new Uri("pack://application:,,,/DawnQuant.App;component/Assets/Images/sun.png", UriKind.Absolute));
            string text1 = $"{task.Name}任务计划已经成功执行完毕！";
            string text2 = null;
            string text3 = null;
            INotification notification = NotificationService.CreatePredefinedNotification(text1, text2, text3, image);
            Show(notification);
           
        }

        public void ShowDataUpdateScheduledTaskCompletedNofify()
        {
            ImageSource image = new BitmapImage(new Uri("pack://application:,,,/DawnQuant.App;component/Assets/Images/sun.png", UriKind.Absolute));
            string text1 = $"更新交易数据已完成！";
            string text2 = null;
            string text3 = null;
            INotification notification = NotificationService.CreatePredefinedNotification(text1, text2, text3, image);
            Show(notification);
        }

        void Show(INotification notification)
        {
            notification.ShowAsync().ContinueWith(OnNotificationShown, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void OnNotificationShown(Task<NotificationResult> task)
        {
            try
            {
                switch (task.Result)
                {
                    case NotificationResult.Activated:
                        break;
                    case NotificationResult.TimedOut:
                        break;
                    case NotificationResult.UserCanceled:
                        break;
                    case NotificationResult.Dropped:
                        break;
                }
            }
            catch 
            {
            }
        }
     
    }
    #endregion

}
