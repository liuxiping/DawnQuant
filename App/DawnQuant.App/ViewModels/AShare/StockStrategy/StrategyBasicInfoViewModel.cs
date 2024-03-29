﻿using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.App.Utils;
using DawnQuant.App.Models.AShare.UserProfile;

namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StrategyBasicInfoViewModel : ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly StockStrategyService _stockStrategyService;

        public StrategyBasicInfoViewModel(Tuple<string,string ,long,int> basicInfo)
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();

            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();


            if (basicInfo!=null)
            {
                Name = basicInfo.Item1;
                Desc = basicInfo.Item2;
                SortNum= basicInfo.Item4;
            }
            Initialize( basicInfo);

        }


        private async void Initialize(Tuple<string, string, long,int> basicInfo)
        {
            ObservableCollection<StockStrategyCategory> categories = null;
            await Task.Run(() =>
            {
                categories = _stockStrategyService.GetStockStrategyCategoriesByUser(_passportProvider.UserId);
            }).ConfigureAwait(true);

            StockStrategyCategories = categories;

            if(StockStrategyCategories!=null && StockStrategyCategories.Count>0)
            {
                if (basicInfo != null)
                {
                    InitCategory(basicInfo.Item3);
                }
            }

        }

       

        /// <summary>
        /// 策略名称
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, nameof(Name)); }
        }


        /// <summary>
        /// 排序
        /// </summary>
        private int _sortNum=0;
        public int SortNum
        {
            get { return _sortNum; }
            set { SetProperty(ref _sortNum, value, nameof(SortNum)); }
        }

        /// <summary>
        /// 策略描述
        /// </summary>
        private string _desc;
        public string Desc
        {
            get { return _desc; }
            set { SetProperty(ref _desc, value, nameof(Desc)); }
        }

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        StockStrategyCategory _curSelStockStrategyCategory;
        public StockStrategyCategory CurSelStockStrategyCategory
        {
            get { return _curSelStockStrategyCategory; }
            set { SetProperty(ref _curSelStockStrategyCategory, value, nameof(CurSelStockStrategyCategory)); }
        }

        
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
        ObservableCollection<StockStrategyCategory> _stockStrategyCategories;
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories
        {
            get { return _stockStrategyCategories; }
            set { SetProperty(ref _stockStrategyCategories, value, nameof(StockStrategyCategories)); }
        }
        
    }
}
