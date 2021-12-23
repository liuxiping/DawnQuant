using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using UserProfile = DawnQuant.App.Core.Models.AShare.UserProfile;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class StrategyItemViewModel 
    {

        private readonly StockStrategyService _stockStrategyService;


        public StrategyItemViewModel(StockStrategyService stockStrategyService)
        {
            _stockStrategyService = stockStrategyService;
        }

        public UserProfile.StockStrategy Strategy { set; get; }
       

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
