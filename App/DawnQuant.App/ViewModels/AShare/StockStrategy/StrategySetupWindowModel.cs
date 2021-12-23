﻿using System;
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
    public class StrategySetupWindowModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StockStrategyService _stockStrategyService;


        public StrategySetupWindowModel()
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();

            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();

            FinishCommand = new DelegateCommand(Finish);


        }


        UserProfile.StockStrategy _strategy;
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
            StrategyExecutorInsDescriptor instance = null;

            if (!string.IsNullOrEmpty(_strategy?.StockStragyContent))
            {
                instance = JsonSerializer.Deserialize<StrategyExecutorInsDescriptor>(_strategy.StockStragyContent);
            }

            var scopeModel = new SelectScopeViewModel(instance?.SelectScopeInsDescriptors);
            var factorModel = new FactorViewModel(instance?.FactorInsDescriptors);

            ScopeViewModel = scopeModel;
            FactorViewModel = factorModel;


            if (_strategy != null)
            {
                var basicInfoModel = new StrategyBasicInfoViewModel(
                    new Tuple<string, string, long,int>(_strategy.Name, _strategy.Desc,
                    _strategy.CategoryId, _strategy.SortNum));

                BasicInfoViewModel = basicInfoModel;

            }
            else
            {
                BasicInfoViewModel=new StrategyBasicInfoViewModel(null);
            }

           
          

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

        StrategyBasicInfoViewModel _basicInfoViewModel;
        public StrategyBasicInfoViewModel BasicInfoViewModel
        {
            get { return _basicInfoViewModel; }
            set { SetProperty(ref _basicInfoViewModel, value, nameof(BasicInfoViewModel)); }

        }


        public DelegateCommand FinishCommand { set; get; }

        /// <summary>
        /// 保存用户策略数据
        /// </summary>
        private async void Finish()
        {
            StrategyExecutorInsDescriptor strategyInsDescriptor = new StrategyExecutorInsDescriptor();
            strategyInsDescriptor.SelectScopeInsDescriptors.AddRange(ScopeViewModel.GetStockSelectScopeInsDescriptors());
            strategyInsDescriptor.FactorInsDescriptors.AddRange(FactorViewModel.GetFactorExecutorInsDescriptors());

            //保存策略
            UserProfile.StockStrategy stockStrategy = new UserProfile.StockStrategy();
            stockStrategy.UserId = _passportProvider.UserId;
            stockStrategy.Name = BasicInfoViewModel.Name;
            stockStrategy.Desc = BasicInfoViewModel.Desc;
            stockStrategy.SortNum = BasicInfoViewModel.SortNum;
            stockStrategy.CategoryId = BasicInfoViewModel.CurSelStockStrategyCategory.Id;
            stockStrategy.CreateTime = DateTime.Now;
            stockStrategy.StockStragyContent = JsonSerializer.Serialize(strategyInsDescriptor);
            if (_strategy != null)
            {
                stockStrategy.Id = _strategy.Id;
            }

            await Task.Run(() =>
             {
                 var ss = _stockStrategyService.SaveStockStrategy(stockStrategy);
             }).ConfigureAwait(true);

        }
    }
}
