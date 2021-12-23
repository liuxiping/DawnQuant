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

namespace DawnQuant.App.ViewModels.AShare
{
    public class AShareMainViewModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        public AShareMainViewModel()
        {
            _passportProvider=IOCUtil.Container.Resolve<IPassportProvider>();
        }


        #region notification
        [ServiceProperty(Key = "_notificationService")]
        public virtual INotificationService NotificationService { get { return null; } }


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
