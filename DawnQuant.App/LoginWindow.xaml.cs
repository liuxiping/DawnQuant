﻿using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using DawnQuant.Passport;
using DawnQuant.App.Utils;
using Autofac;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginWindowModel();
        }

        LoginWindowModel Model
        {
            get { return (LoginWindowModel)DataContext; }
        }

        private void _btnLogin_Click(object sender, RoutedEventArgs e)
        {
          
            try
            {
                _btnLogin.IsEnabled = false;

                if (string.IsNullOrEmpty(Model.Name) || string.IsNullOrEmpty(_txtPassword.Password))
                {
                    ThemedMessageBox.Show("温馨提示", "用户名和密码不能为空！", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Model.Password = _txtPassword.Password;
                IPassportProvider _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
                if (_passportProvider.Login(Model.Name, Model.Password))
                {
                    //保存配置信息
                    if (Model.Remember)
                    {
                        //base 编码
                        string jsonUserInfo = JsonSerializer.Serialize(Model);
                        byte[] bytes = Encoding.UTF8.GetBytes(jsonUserInfo);
                        string base64UserInfo = Convert.ToBase64String(bytes);

                        //保存信息
                        string configPath = Path.Combine(Environment.CurrentDirectory, "\\Config");
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }
                        string configFile = Path.Combine(Environment.CurrentDirectory, "\\Config\\user.config");
                        File.WriteAllText(configFile, base64UserInfo);
                    }

                    //登录成功 更新数据
                    Visibility = Visibility.Collapsed;

                    DownloadDataWindow loadDataWindow = new DownloadDataWindow();
                    loadDataWindow.ShowDialog();

                    this.Close();

                }
                else
                {
                    _btnLogin.IsEnabled = true;
                    ThemedMessageBox.Show("温馨提示", "用户名或密码错误", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                _btnLogin.IsEnabled = true;
                ThemedMessageBox.Show("温馨提示", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void _btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);

        }
        private void _btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        class LoginWindowModel : ViewModelBase
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value, nameof(Name)); }
            }


            private string _password;
            public string Password
            {
                get { return _password; }
                set { SetProperty(ref _password, value, nameof(Password)); }
            }


            private bool _remember = false;
            public bool Remember
            {
                get { return _remember; }
                set { SetProperty(ref _remember, value, nameof(Remember)); }
            }


            private bool _autoLogin = false;
            public bool AutoLogin
            {
                get { return _autoLogin; }
                set { SetProperty(ref _autoLogin, value, nameof(AutoLogin)); }
            }

        }

        private void _loginWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();

            }
        }


        private async void _loginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //读取配置文件信息
            string configFile = Path.Combine(Environment.CurrentDirectory, "\\Config\\user.config");
            if (File.Exists(configFile))
            {
                //base64 解码
                string base64UserInfo = File.ReadAllText(configFile);
                byte[] bytes = Convert.FromBase64String(base64UserInfo);
                string jsonUserInfo = Encoding.UTF8.GetString(bytes);

                LoginWindowModel model = JsonSerializer.Deserialize<LoginWindowModel>(jsonUserInfo);

                if (model.Remember)
                {
                    Model.Name = model.Name;
                    Model.Password = model.Password;
                    Model.Remember = model.Remember;
                    Model.AutoLogin = model.AutoLogin;
                    _txtPassword.Password = model.Password;
                }
                //自动登录
                if (model.AutoLogin)
                {
                    _btnLogin.IsEnabled = false;
                    //延时2后登录
                    await Task.Delay(2000);
                    if (chkAutoLogin.IsChecked == true)
                    {
                        _btnLogin_Click(null, null);
                    }
                    else
                    {
                        _btnLogin.IsEnabled = true;
                    }
                }
            }
        }

        private void chkAutoLogin_Checked(object sender, RoutedEventArgs e)
        {
            chkRemember.IsChecked = true;
        }
    }
}
