using DawnQuant.App.Models.AShare.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    /// <summary>
    /// 任务消息
    /// </summary>
    public class MessageUtil
    {

        /// <summary>
        /// 客户端定时执行策略计划完成
        /// </summary>
        public event Action<StrategyScheduledTask> StrategyScheduledTaskCompleted;
        public void OnStrategyScheduledTaskCompleted(StrategyScheduledTask task)
        {
            StrategyScheduledTaskCompleted?.Invoke(task);
        }


        /// <summary>
        /// 下载交易数据进度
        /// </summary>
        public event Action<int,int> DownloadAShareDataProgress;
        public void OnDownloadAShareDataProgress(int complete,int total)
        {
            DownloadAShareDataProgress?.Invoke(complete, total);
        }

        /// <summary>
        /// 下载A股数据完成通知
        /// </summary>
        public event Action<bool> DownloadAShareDataComplete;
        public void OnDownloadAShareDataComplete(bool isDownlaodAllData)
        {
            DownloadAShareDataComplete?.Invoke(isDownlaodAllData);
        }


        //定时更新交易数据进度
        public event Action<int, int> DataUpdateScheduledTaskJobProgress;
        public void OnDataUpdateScheduledTaskJobProgress(int complete, int total)
        {
            DataUpdateScheduledTaskJobProgress?.Invoke(complete,total);
        }


        //定时更新交易数据完成
        public event Action DataUpdateScheduledTaskJobCompleted;
        public void OnDataUpdateScheduledTaskJobCompleted( )
        {
            DataUpdateScheduledTaskJobCompleted?.Invoke();
        }

    }

}
