using DawnQuant.App.Services.AShare;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.App.Utils;
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;

namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StrategyItemViewModel : ViewModelBase
    {

        private readonly StockStrategyService _stockStrategyService;


        public StrategyItemViewModel()
        {
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();
            DelStrategyCommand = new AsyncCommand(DelStrategy);
        }

        UserProfile.StockStrategy _strategy;
        public UserProfile.StockStrategy Strategy 
        {
            get { return _strategy; }
            set { SetProperty(ref _strategy, value, nameof(Strategy)); } 
        }

        public AsyncCommand DelStrategyCommand { set; get; }
        private Task DelStrategy()
        {
            //删除策略

            return Task.Run(() =>
            {
                _stockStrategyService.DelStockStrategy(Strategy.Id);

            });
           
        }
    }

}
