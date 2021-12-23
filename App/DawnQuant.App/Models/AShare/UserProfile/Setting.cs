using DawnQuant.App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.UserProfile
{
    public class Setting
    {
        /// <summary>
        /// 数据更新设置
        /// </summary>
        public DataUpdateSetting DataUpdateSetting { get; set; }


        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Setting Instance(string json)
        {
            Setting settings =null;
            if (!string.IsNullOrEmpty(json))
            {
                 settings = (Setting)JsonSerializer.Deserialize(json, typeof(Setting));
            }
            return settings;

        }

        /// <summary>
        /// Json 
        /// </summary>
        /// <returns></returns>
        public  string Serialize()
        {
          return  JsonSerializer.Serialize(this);
        }
    }

    public class DataUpdateSetting
    {
        /// <summary>
        /// 自选分类
        /// </summary>
        public List<long> SelfSelCategories { get; set; }


        /// <summary>
        /// 龙头股
        /// </summary>
        public bool UpdateBellwether { get; set; }

        /// <summary>
        /// 题材热点
        /// </summary>
        public bool UpdateSubjectAndHot { get; set; }
        /// <summary>
        /// 任务定时Cron
        /// </summary>
        public string TaskCron { get; set; }
    }


}
