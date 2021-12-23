using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using System.Collections.ObjectModel;
using DawnQuant.App.Core.Utils;
using DawnQuant.App.Core.Models.AShare.UserProfile;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class StrategyBasicInfoViewModel 
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StockStrategyService _stockStrategyService;

        public StrategyBasicInfoViewModel(IPassportProvider passportProvider,
           StockStrategyService stockStrategyService)
        {
            _passportProvider = passportProvider;

            _stockStrategyService = stockStrategyService;

            StockStrategyCategories = _stockStrategyService.GetStockStrategyCategoriesByUser(_passportProvider.UserId);

        }


        /// <summary>
        /// 策略名称
        /// </summary>
        public string Name { get; set; }
        

        /// <summary>
        /// 策略描述
        /// </summary>
        public string Desc { get; set; }
        

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        public StockStrategyCategory CurSelStockStrategyCategory { get; set; }
        

        
        /// <summary>
        /// 初始化策略分类
        /// </summary>
        /// <param name="Id"></param>
        public void InitCategory(long Id)
        {
            if(StockStrategyCategories!=null)
            {
                foreach(var c in StockStrategyCategories)
                {
                    if (c.Id == Id)
                    {
                        CurSelStockStrategyCategory = c;
                    }
                }
            }
        }

        /// <summary>
        /// 当前用户所有的策略分类
        /// </summary>
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories { get; set; }
       
        
    }
}
