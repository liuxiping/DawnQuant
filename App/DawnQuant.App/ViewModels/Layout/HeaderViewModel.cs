
using Autofac;
using DawnQuant.App.Models;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.Layout
{
    public class HeaderViewModel : ViewModelBase
    {
        AShareDataMaintainService _aShareDataMaintainService;
        SettingService _settingService;
        MessageUtil _messageNotify;


        public HeaderViewModel()
        {
            _aShareDataMaintainService = IOCUtil.Container.Resolve<AShareDataMaintainService>();
            _settingService = IOCUtil.Container.Resolve<SettingService>();
            _messageNotify= IOCUtil.Container.Resolve<MessageUtil>();

            DownloadAllDataCommand = new DelegateCommand(DownloadAllData, CanExecuteDownloadAllData);


        }



        public DelegateCommand DownloadAllDataCommand { get; set; }

        /// <summary>
        /// 下载交易数据
        /// </summary>
        /// <returns></returns>
        private void DownloadAllData()
        {
            if (!App.IsUpdateAllAShareData && !App.IsDataUpdateScheduledTaskJob)
            {
                Task.Run(() =>
               {
                   var notify = IOCUtil.Container.Resolve<MessageUtil>();

                   App.IsUpdateAllAShareData = true;

                   //删除数据
                   string dataPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Data");
                   Directory.Delete(dataPath, true);

                   _aShareDataMaintainService.DownLoadAShareStockDataProgress += (complete, total) =>
                     {
                         _messageNotify.OnDownloadAllAShareDataProgress(complete, total);
                     };
                   _aShareDataMaintainService.DownLoadStockData(1);

                   AppLocalConfig.Instance.LastUpdateAllDataDateTime = DateTime.Now;

                   AppLocalConfig.Instance.Save();

                   App.IsUpdateAllAShareData = false;

                   //数据下载全部完成，通知
                   notify.OnDownloadAShareDataComplete();
                 

               });

            }
        }

        private bool CanExecuteDownloadAllData()
        {
            return  !App.IsUpdateAllAShareData;
        }

    }
}
