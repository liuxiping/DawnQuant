using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models
{
    /// <summary>
    /// 本地配置文件
    /// </summary>
    public class AppLocalConfig
    {
        private AppLocalConfig()
        {

        }

        /// <summary>
        /// 上一次数据更新时间
        /// </summary>
        public DateTime? LastUpdateAllDataDateTime { get; set; }

        public void Save()
        {
            //base 编码
            string config = JsonSerializer.Serialize(this);
            byte[] bytes = Encoding.UTF8.GetBytes(config);
            string base64Config = Convert.ToBase64String(bytes);

            //保存信息
            string configPath = Path.Combine(Environment.CurrentDirectory, "Config");
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            string configFile = Path.Combine(Environment.CurrentDirectory, "Config\\app.config");
            File.WriteAllText(configFile, base64Config);
        }

        private static AppLocalConfig _config = null;
        public static AppLocalConfig Instance
        {
            get
            {
                if (_config == null)
                {
                    

                    string configFile = Path.Combine(Environment.CurrentDirectory, "Config\\app.config");
                    if (File.Exists(configFile))
                    {
                        _config = new AppLocalConfig();

                        //base64 解码
                        string base64UserInfo = File.ReadAllText(configFile);
                        byte[] bytes = Convert.FromBase64String(base64UserInfo);
                        string config = Encoding.UTF8.GetString(bytes);

                        var doc =  JsonDocument.Parse(config);
                        try
                        {
                            JsonElement lastUpdateAllDataDateTime = doc.RootElement.GetProperty("LastUpdateAllDataDateTime");
                            _config.LastUpdateAllDataDateTime = lastUpdateAllDataDateTime.GetDateTime(); ;
                        }
                        catch (KeyNotFoundException ex)
                        {
                            //没有为空
                        }
                        catch(Exception)
                        {
                            throw;
                        }
                       
                    }
                    
                    
                }

                return _config;
            }
        }
    }
}
