using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserProfile = DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.Passport;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Models.AShare.Strategy.Descriptor;
using DawnQuant.App.Core.Utils;

namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class StrategySetupWindowModel 
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly StockStrategyService _stockStrategyService;
        

        public StrategySetupWindowModel(IPassportProvider passportProvider,
           StrategyMetadataService strategyMetadataService,
          StockStrategyService stockStrategyService)
        {
            _passportProvider = passportProvider;
            _strategyMetadataService = strategyMetadataService;
            _stockStrategyService = stockStrategyService;

            
        }

        UserProfile.StockStrategy  _strategy;
        public UserProfile.StockStrategy Strategy
        {
            get { return _strategy; }
            set
            {
                _strategy = value;
                InitStrategyExecutorInsDescriptor();

            }

        }

        /// <summary>
        /// 策略参数初始化
        /// </summary>
        public void InitStrategyExecutorInsDescriptor()
        {

            //初始数据
            var scopeModel = new SelectScopeViewModel(_strategyMetadataService,_passportProvider);
            var factorModel = new FactorViewModel(_strategyMetadataService, _passportProvider);
            var basicInfoModel = new StrategyBasicInfoViewModel(_passportProvider,_stockStrategyService);

            //参数不为空 编辑状态
            if (!string.IsNullOrEmpty(_strategy?.StockStragyContent))
            {
                var instance = JsonSerializer.Deserialize<StrategyExecutorInsDescriptor>(_strategy.StockStragyContent);

                scopeModel.InitSelectScopeIns(instance.SelectScopeInsDescriptors);
                factorModel.InitFactorIns(instance.FactorInsDescriptors);

                basicInfoModel.Name = _strategy.Name;
                basicInfoModel.Desc = _strategy.Desc;
                basicInfoModel.InitCategory(_strategy.CategoryId);
            }

            ScopeViewModel = scopeModel;
            FactorViewModel = factorModel;
            BasicInfoViewModel = basicInfoModel;

        }

        public SelectScopeViewModel ScopeViewModel { get; set; }
        

        public FactorViewModel FactorViewModel { get; set; }
        

        public StrategyBasicInfoViewModel BasicInfoViewModel { get; set; }
        



        /// <summary>
        /// 保存用户策略数据
        /// </summary>
        private void Finish()
        {
            StrategyExecutorInsDescriptor strategyInsDescriptor = new StrategyExecutorInsDescriptor();
            strategyInsDescriptor.SelectScopeInsDescriptors.AddRange(ScopeViewModel.GetStockSelectScopeInsDescriptors());
            strategyInsDescriptor.FactorInsDescriptors.AddRange(FactorViewModel.GetFactorExecutorInsDescriptors());

            //保存策略
            UserProfile.StockStrategy stockStrategy = new UserProfile.StockStrategy();
            stockStrategy.UserId = _passportProvider.UserId;
            stockStrategy.Name = BasicInfoViewModel.Name;
            stockStrategy.Desc = BasicInfoViewModel.Desc;
            stockStrategy.CategoryId = BasicInfoViewModel.CurSelStockStrategyCategory.Id;
            stockStrategy.CreateTime = DateTime.Now;
            stockStrategy.StockStragyContent = JsonSerializer.Serialize(strategyInsDescriptor);
            if (_strategy != null)
            {
                stockStrategy.Id = _strategy.Id;
            }
            var ss = _stockStrategyService.SaveStockStrategy(stockStrategy);
        }
    }
}
