using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;
using DevExpress.Mvvm;
using DawnQuant.Passport;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Models.AShare.Strategy.Descriptor;
using DawnQuant.App.Utils;
using Autofac;

namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StrategySetupWindowModel :ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StrategyMetadataService _strategyMetadataService;
        private readonly StockStrategyService _stockStrategyService;
        

        public StrategySetupWindowModel()
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _strategyMetadataService = IOCUtil.Container.Resolve<StrategyMetadataService>();
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();

            FinishCommand = new DelegateCommand(Finish);

            
        }

        UserProfile.StockStrategy  _strategy;
        public UserProfile.StockStrategy Strategy
        {
            get { return _strategy; }
            set
            {
                if (SetProperty(ref _strategy, value, nameof(Strategy)))
                {
                    InitStrategyExecutorInsDescriptor();
                }
            }

        }

        /// <summary>
        /// 策略参数初始化
        /// </summary>
        public void InitStrategyExecutorInsDescriptor()
        {

            //初始数据
            var scopeModel = new SelectScopeViewModel();
            var factorModel = new FactorViewModel();
            var basicInfoModel = new StrategyBasicInfoViewModel();

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

        SelectScopeViewModel _scopeViewModel;
        public SelectScopeViewModel ScopeViewModel
        {
            get { return _scopeViewModel; }
            set { SetProperty(ref _scopeViewModel, value, nameof(ScopeViewModel)); }

        }

        FactorViewModel _factorViewModel;
        public FactorViewModel FactorViewModel
        {
            get { return _factorViewModel; }
            set { SetProperty(ref _factorViewModel, value, nameof(_factorViewModel)); }

        }

        StrategyBasicInfoViewModel  _basicInfoViewModel;
        public StrategyBasicInfoViewModel BasicInfoViewModel
        {
            get { return _basicInfoViewModel; }
            set { SetProperty(ref _basicInfoViewModel, value, nameof(BasicInfoViewModel)); }

        }


        public  DelegateCommand FinishCommand { set; get; }

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
