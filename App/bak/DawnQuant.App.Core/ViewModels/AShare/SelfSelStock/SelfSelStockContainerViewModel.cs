using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using Microsoft.Extensions.Logging;

namespace DawnQuant.App.Core.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockContainerViewModel
    {

        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;

        public SelfSelStockContainerViewModel(
               SelfSelService selfSelService,
               ILogger<SelfSelStockContainerViewModel> logger,
               IPassportProvider passportProvider)
        {
            _selfSelService = selfSelService;
            _passportProvider = passportProvider;
        }

        public IEnumerable<SelfSelectStockCategory> SelfSelCategorys
        {
            get
            {
                return _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            }
        }
    }
}
