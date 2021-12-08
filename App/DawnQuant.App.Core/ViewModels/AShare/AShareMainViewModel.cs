using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;


namespace DawnQuant.App.Core.ViewModels.AShare
{
    public class AShareMainViewModel
    {
        private readonly IPassportProvider _passportProvider;
        public AShareMainViewModel(IPassportProvider passportProvider)
        {
            _passportProvider = passportProvider;
        }

    }

}
