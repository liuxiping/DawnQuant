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

namespace DawnQuant.App.ViewModels.AShare.Setting
{
    internal class DataUpdateSettingViewModel:ViewModelBase
    {
        private readonly IPassportProvider _passportProvider;
        private readonly SelfSelService _selfSelService;

        private readonly SettingService _settingService ;


        public DataUpdateSettingViewModel()
        {
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();

            _settingService = IOCUtil.Container.Resolve<SettingService>();

            //初始化

            if (App.AShareSetting!=null && App.AShareSetting.DataUpdateSetting!=null)
            {
                UpdateBellwether = App.AShareSetting.DataUpdateSetting.UpdateBellwether;
                UpdateSubjectAndHot = App.AShareSetting.DataUpdateSetting.UpdateSubjectAndHot;

                TaskCron = App.AShareSetting.DataUpdateSetting.TaskCron;
            }

            LoadCategories();
        }

        public event Action CategoriesLoaded;
        public void OnCategoriesUpdated()
        {
            CategoriesLoaded?.Invoke();
        }

        //所有分类
        ObservableCollection<SelfSelectStockCategory> _categories;
        public ObservableCollection<SelfSelectStockCategory> Categories
        {
            set
            {
                SetProperty(ref _categories, value, nameof(Categories));
                OnCategoriesUpdated();
            }
            get
            { return _categories; }
        }

        public async void LoadCategories()
        {
            //刷新自选股分类
            ObservableCollection<SelfSelectStockCategory> categories = null;
            await Task.Run(() => 
            {
                categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            }).ConfigureAwait(true);
            Categories = categories;
        }

        public void Save()
        {

            if (App.AShareSetting == null)
            { 
                App.AShareSetting = new Models.AShare.UserProfile.Setting();
            }

            var s = App.AShareSetting;

            if(s.DataUpdateSetting==null)
            {
                s.DataUpdateSetting=new DataUpdateSetting();
            }

            s.DataUpdateSetting.SelfSelCategories = UpdateSelfSelStockCategoryIds;
            s.DataUpdateSetting.UpdateBellwether = UpdateBellwether;
            s.DataUpdateSetting.UpdateSubjectAndHot = UpdateSubjectAndHot;
            s.DataUpdateSetting.TaskCron = TaskCron;

            Task.Run(() => 
            {
                _settingService.SaveSetting(App.AShareSetting.Serialize()); 

            }).ConfigureAwait(true);
         

        }

        bool _updateBellwether=false;
        public bool UpdateBellwether
        {
            set
            {
                SetProperty(ref _updateBellwether, value, nameof(UpdateBellwether));
                OnCategoriesUpdated();
            }
            get
            { return _updateBellwether; }
        }

        bool _updateSubjectAndHot=false;
        public bool UpdateSubjectAndHot
        {
            set
            {
                SetProperty(ref _updateSubjectAndHot, value, nameof(UpdateSubjectAndHot));
                OnCategoriesUpdated();
            }
            get
            { return _updateSubjectAndHot; }
        }

        List<long> _updateSelfSelStockCategoryIds;
        public List<long> UpdateSelfSelStockCategoryIds
        {
            set
            {
                SetProperty(ref _updateSelfSelStockCategoryIds, value, nameof(UpdateSelfSelStockCategoryIds));
                OnCategoriesUpdated();
            }
            get
            { return _updateSelfSelStockCategoryIds; }
        }


        string _taskCron ;
        public string TaskCron
        {
            set
            {
                SetProperty(ref _taskCron, value, nameof(TaskCron));
                OnCategoriesUpdated();
            }
            get
            { return _taskCron; }
        }
    }
}
