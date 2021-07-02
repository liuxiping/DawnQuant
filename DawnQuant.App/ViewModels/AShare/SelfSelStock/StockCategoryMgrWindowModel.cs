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

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class StockCategoryMgrWindowModel : ViewModelBase
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public StockCategoryMgrWindowModel()
        {

            _stockPlotDataService = IOCUtil.Container.Resolve<StockPlotDataService>(); ;
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();


            NewCommand = new DelegateCommand(New);
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);

            Initialize();
        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public Task Initialize()
        {
            //加载分类数据
            var t = Task.Run(() =>
            {
                LoadCategories();
            });
            return t;
        }

        /// <summary>
        /// 加载分类
        /// </summary>
        private void LoadCategories()
        {
            Categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);

        }

       

        //所有分类
        ObservableCollection<SelfSelectStockCategory> _categories;
        public ObservableCollection<SelfSelectStockCategory> Categories
        {
            set
            { SetProperty(ref _categories, value, nameof(Categories)); }
            get
            { return _categories; }
        }

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        SelfSelectStockCategory _curSelCategory;
        public SelfSelectStockCategory CurSelCategory
        {
            set
            {
                if (SetProperty(ref _curSelCategory, value, nameof(CurSelCategory)))
                {
                    OnCurSelCategoryChange(value);
                }
                
            }
            get
            { return _curSelCategory; }
        }

        private void OnCurSelCategoryChange(SelfSelectStockCategory category)
        {
           if(category==null)
            {
                Name = null;
                Desc = null;
                Sort = 0;
                IsGroup = false;
            }
           else
            {
                Name = category.Name;
                Desc = category.Desc;
                Sort = category.SortNum;
                IsGroup = category.IsGroupByIndustry;
            }
        }

        string _name;
        public string  Name
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

        bool _isGroup;
        public bool IsGroup
        {
            set
            { SetProperty(ref _isGroup, value, nameof(IsGroup)); }
            get
            { return _isGroup; }
        }


        public DelegateCommand SaveCommand { set; get; }
        private void Save()
        {
            SelfSelectStockCategory c = new SelfSelectStockCategory();
            c.Name = Name;
            c.Desc = Desc;
            c.SortNum = Sort;
            c.IsGroupByIndustry = IsGroup;
            c.UserId = _passportProvider.UserId;
            c.CreateTime = DateTime.Now;
            if (CurSelCategory != null)
            {
                c.Id = CurSelCategory.Id;
            }
            var uc = _selfSelService.SaveSelfSelectCategory(c);

            LoadCategories();

            CurSelCategory = Categories.Where(p => p.Id == uc.Id).FirstOrDefault();


        }

        public DelegateCommand NewCommand { set; get; }
        private void New()
        {
            CurSelCategory = null;
        }

        public DelegateCommand DeleteCommand { set; get; }
        private void Delete()
        {
            if (CurSelCategory != null)
            {
                _selfSelService.DelSelfSelectCategory(CurSelCategory);
                LoadCategories();

            }
        }

    }
}
