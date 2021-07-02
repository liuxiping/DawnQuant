using AutoMapper;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class SelfSelStockContainerViewModel : ViewModelBase
    {

        private readonly ILogger<SelfSelStockContainerViewModel> _logger;
        private readonly IPassportProvider _passportProvider;

        //自定义选股类别
        List<SelfSelectStockCategory> _selfSelCategorys = new List<SelfSelectStockCategory>();

        public IEnumerable<SelfSelectStockCategory> SelfSelCategorys
        {
            get
            {

                long userId = long.Parse(_passportProvider.Claims.Where(p => p.Type == "sub").SingleOrDefault().Value);

                return _selfSelService.GetSelfSelectStockCategories(userId);
            }
        }

        SelfSelService _selfSelService;

        public SelfSelStockContainerViewModel(
               SelfSelService selfSelService,
               ILogger<SelfSelStockContainerViewModel> logger,
          IPassportProvider passportProvider)
        {
            _selfSelService = selfSelService;
            _logger = logger;
            _passportProvider = passportProvider;
        }
        
    }
}
