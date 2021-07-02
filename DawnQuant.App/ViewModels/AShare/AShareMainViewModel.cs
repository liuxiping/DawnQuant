using DawnQuant.Passport;
using DevExpress.Mvvm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.AShare
{
    public class AShareMainViewModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;


        public AShareMainViewModel( IPassportProvider passportProvider)
        {
            _passportProvider = passportProvider;
        }
    }
}
