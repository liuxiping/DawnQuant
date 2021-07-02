﻿
using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;


namespace DawnQuant.App.ViewModels.AShare.StockStrategy
{
    public class StrategyCategoryMgrWindowModel:ViewModelBase
    {
        private readonly StockStrategyService  _stockStrategyService;
        private readonly IPassportProvider _passportProvider;


        public StrategyCategoryMgrWindowModel()
        {
            _stockStrategyService = IOCUtil.Container.Resolve<StockStrategyService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();

            NewCommand = new DelegateCommand(New);
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);

            Task.Run(() =>
            {
                LoadCategories();
            });
        }

        private void LoadCategories()
        {
            StockStrategyCategories = _stockStrategyService.GetStockStrategyCategoriesByUser(_passportProvider.UserId);

        }

        //所有分类
        ObservableCollection<StockStrategyCategory> _stockStrategyCategories;
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories
        {
            set
            { SetProperty(ref _stockStrategyCategories, value, nameof(StockStrategyCategories)); }
            get
            { return _stockStrategyCategories; }
        }

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        StockStrategyCategory _curSelStockStrategyCategory;
        public StockStrategyCategory CurSelStockStrategyCategory
        {
            set
            {
                SetProperty(ref _curSelStockStrategyCategory, value, nameof(CurSelStockStrategyCategory));
                OnCurSelCategoryChange(value);
            }
            get
            { return _curSelStockStrategyCategory; }
        }

        private void OnCurSelCategoryChange(StockStrategyCategory category)
        {
            if (category == null)
            {
                Name = null;
                Desc = null;
                Sort = 0;
            }
            else
            {
                Name = category.Name;
                Desc = category.Desc;
                Sort = category.SortNum;
            }
        }

        string _name;
        public string Name
        {
            set
            { SetProperty(ref _name, value, nameof(Name)); }
            get
            { return _name; }
        }

        string _desc;
        public string Desc
        {
            set
            { SetProperty(ref _desc, value, nameof(Desc)); }
            get
            { return _desc; }
        }

        int _sort;
        public int Sort
        {
            set
            { SetProperty(ref _sort, value, nameof(Sort)); }
            get
            { return _sort; }
        }

        public DelegateCommand SaveCommand { set; get; }
        private void Save()
        {
            StockStrategyCategory c = new StockStrategyCategory();
            c.Name = Name;
            c.Desc = Desc;
            c.SortNum = Sort;
            c.UserId = _passportProvider.UserId;
            c.CreateTime = DateTime.Now;
            if (CurSelStockStrategyCategory != null)
            {
                c.Id = CurSelStockStrategyCategory.Id;
            }
            var uc = _stockStrategyService.SaveStockStrategyCategory(c);

            LoadCategories();

            CurSelStockStrategyCategory = StockStrategyCategories.Where(p => p.Id == uc.Id).FirstOrDefault();


        }

        public DelegateCommand NewCommand { set; get; }
        private void New()
        {
            CurSelStockStrategyCategory = null;
        }

        public DelegateCommand DeleteCommand { set; get; }
        private void Delete()
        {
            if (CurSelStockStrategyCategory != null)
            {
                _stockStrategyService.DelStockStrategyCategory(CurSelStockStrategyCategory);
                LoadCategories();
            }
        }
    }
}
