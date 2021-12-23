using DawnQuant.App.Core.ViewModels;
using DawnQuant.Passport;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Core.Views
{
    public partial class LoginView
    {

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //读取配置文件信息
            string configFile = Path.Combine(Environment.CurrentDirectory, "Config\\user.config");
            if (File.Exists(configFile))
            {
                //base64 解码
                string base64UserInfo = File.ReadAllText(configFile);
                byte[] bytes = Convert.FromBase64String(base64UserInfo);
                string jsonUserInfo = Encoding.UTF8.GetString(bytes);

                LoginViewModel model = JsonSerializer.Deserialize<LoginViewModel>(jsonUserInfo);

                if (model.Remember)
                {
                    ViewModel.Name = model.Name;
                    ViewModel.Password = model.Password;
                    ViewModel.Remember = model.Remember;
                    ViewModel.AutoLogin = model.AutoLogin;
                    ViewModel.Password = model.Password;
                }
                //自动登录
                if (model.AutoLogin)
                {
                    //延时登录
                    await Task.Delay(1000);
                    if (ViewModel.AutoLogin)
                    {
                        Login();
                    }
                }
            }
        }

       

        const string login = "登  录";

        /// <summary>
        /// 按钮文字
        /// </summary>
        string btnMessage { get; set; } = login;

        /// <summary>
        /// 登录状态
        /// </summary>
        bool loginDisabled = false;

        [Inject]
        private LoginViewModel ViewModel { get; set; }

        [Inject]
        IPassportProvider PassportProvider { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private void Login()
        {
            try
            {
                if (string.IsNullOrEmpty(ViewModel.Name) || string.IsNullOrEmpty(ViewModel.Password))
                {
                    return;
                }

                btnMessage = "正在登录...";
                loginDisabled = true;
                bool isSuccess = false;
                string name = ViewModel.Name;
                string pwd = ViewModel.Password;
                isSuccess = PassportProvider.Login(name, pwd);

                //保存配置信息
                if (isSuccess)
                {
                    //保存登录信息
                    if (ViewModel.Remember)
                    {
                        //base 编码
                        string jsonUserInfo = JsonSerializer.Serialize(ViewModel);
                        byte[] bytes = Encoding.UTF8.GetBytes(jsonUserInfo);
                        string base64UserInfo = Convert.ToBase64String(bytes);

                        //保存信息
                        string configPath = Path.Combine(Environment.CurrentDirectory, "Config");
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }
                        string configFile = Path.Combine(Environment.CurrentDirectory, "Config\\user.config");
                        File.WriteAllText(configFile, base64UserInfo);
                    }
                    //登录成功 更新数据
                    NavigationManager.NavigateTo("/DownloadData");
                }
                else
                {
                    //登录失败
                    btnMessage = login;
                    loginDisabled = false;
                    //ModelState.
                }
            }
            catch (Exception ex)
            {
                loginDisabled = false;
            }

        }
    }
}
    
