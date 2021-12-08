using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Core.ViewModels
{
    internal class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string Name { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        public bool Remember { get; set; }

        bool _autoLogin;
        public bool AutoLogin
        {
            get
            {
                return _autoLogin;
            }
            set
            {

                _autoLogin = value;
                if(value)
                {
                    Remember=true;
                }
            }
        }


       

    }
}
