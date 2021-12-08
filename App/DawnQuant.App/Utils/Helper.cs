using DawnQuant.App.Models.AShare;
using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
   public class Helper
    {
       
        public static string GetStockTradeDataFileName(string tscode, KCycle kCycle)
        {
            //日线
            if (kCycle == KCycle.Day)
            {
                string dailyTDPath = Path.Combine(Environment.CurrentDirectory, "Data\\DailyTD\\");

                //确保目录已经创建

                if (!Directory.Exists(dailyTDPath))
                {
                    Directory.CreateDirectory(dailyTDPath);
                }


                return Path.Combine(dailyTDPath, tscode + "_" + kCycle.ToString() + ".bin");
            }
            else
            {
                throw new NotSupportedException("只支持日线和五分钟数据");
            }
        }
    }
}
