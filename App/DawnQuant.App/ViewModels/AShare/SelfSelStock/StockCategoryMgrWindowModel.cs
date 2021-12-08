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
using System.Windows.Threading;

namespace DawnQuant.App.ViewModels.AShare.SelfSelStock
{
    public class StockCategoryMgrWindowModel : ViewModelBase
    {

       
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public StockCategoryMgrWindowModel( )
        {
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
        public void Initialize()
        {
            //  1 加入时间   2 行业
            StockSortFields = new ObservableCollection<StockSortField>();
            StockSortFields.Add(new StockSortField { Id = 1, Name = "加入时间" });
            StockSortFields.Add(new StockSortField { Id = 2, Name = "行业" });

            CurSelStockSortField = StockSortFields.First();

            //加载分类数据
            LoadCategories();
           
        }

        /// <summary>
        /// 加载分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<SelfSelectStockCategory> categories = null;

            await Task.Run(() => {
                categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);
            }).ConfigureAwait(true);

            Categories = categories;

            if (CurSelCategory == null && Categories != null && Categories.Count > 0)
            {
                CurSelCategory = Categories[0];
            }
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
                CurSelStockSortField = StockSortFields[0];
            }
           else
            {
                Name = category.Name;
                Desc = category.Desc;
                Sort = category.SortNum;
                IsGroup = category.IsGroupByIndustry;
                if(category.StockSortFiled==1)
                {
                    CurSelStockSortField = StockSortFields[0];
                }
                else if (category.StockSortFiled == 2)
                {
                    CurSelStockSortField = StockSortFields[1];
                }
                   
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

        int _sort=1;
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
        private async void Save()
        {
            SelfSelectStockCategory c = new SelfSelectStockCategory();
            c.Name = Name;
            c.Desc = Desc;
            c.SortNum = Sort;
            c.IsGroupByIndustry = IsGroup;
            c.UserId = _passportProvider.UserId;
            c.CreateTime = DateTime.Now;
            c.StockSortFiled = CurSelStockSortField.Id;
            if (CurSelCategory != null)
            {
                c.Id = CurSelCategory.Id;
            }
            SelfSelectStockCategory uc = null;
            await Task.Run(() =>
            {
                uc = _selfSelService.SaveSelfSelectCategory(c);

                //刷新分类 设置选择

            }).ConfigureAwait(true);

            LoadCategories();

            CurSelCategory = Categories.Where(p => p.Id == uc.Id).FirstOrDefault();
        }

        public DelegateCommand NewCommand { set; get; }
        private void New()
        {
            CurSelCategory = null;
            if (Categories != null && Categories.Count > 0)
            {
                Sort = Categories.Last().SortNum + 1;
            }
        }

        public DelegateCommand DeleteCommand { set; get; }
        private async void Delete()
        {
            if (CurSelCategory != null)
            {
                await Task.Run(() =>
                {
                    _selfSelService.DelSelfSelectCategory(CurSelCategory);
                  
                }).ConfigureAwait(true);
                LoadCategories();
            }
        }


        //排序字段数据
        ObservableCollection<StockSortField> _stockSortFields;
        public ObservableCollection<StockSortField> StockSortFields
        {
            set
            { SetProperty(ref _stockSortFields, value, nameof(StockSortFields)); }
            get
            { return _stockSortFields; }
        }


        //当前选择的排序字段
        StockSortField _curSelStockSortField;
        public StockSortField CurSelStockSortField
        {
            set
            { SetProperty(ref _curSelStockSortField, value, nameof(CurSelStockSortField)); }
            get
            { return _curSelStockSortField; }
        }


        /// <summary>
        /// 分类列表排序字段数据
        /// </summary>
        public class StockSortField
        {
           public int Id { set; get; }
           public string Name { set; get; }
        }

    }
}
