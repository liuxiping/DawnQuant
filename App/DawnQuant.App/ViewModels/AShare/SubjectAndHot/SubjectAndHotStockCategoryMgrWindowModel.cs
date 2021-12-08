using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DawnQuant.Passport;
using DevExpress.Mvvm;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    class SubjectAndHotStockCategoryMgrWindowModel:ViewModelBase
    {
        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly IPassportProvider _passportProvider;


        public SubjectAndHotStockCategoryMgrWindowModel()
        {

            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();


            NewCommand = new DelegateCommand(New);
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);

            LoadCategories();
        }



        /// <summary>
        /// 加载龙头股分类分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<SubjectAndHotStockCategory> categories = null;
            await Task.Run(() =>
            {
                categories = _subjectAndHotService.GetSubjectAndHotStockCategories(_passportProvider.UserId);

            }).ConfigureAwait(true);
            Categories = categories;
            if (CurSelCategory == null && Categories != null && Categories.Count > 0)
            {
                CurSelCategory = Categories.First();
            }
        }

        //所有龙头股分类
        ObservableCollection<SubjectAndHotStockCategory> _categories;
        public ObservableCollection<SubjectAndHotStockCategory> Categories
        {
            set
            { 
                SetProperty(ref _categories, value, nameof(Categories));
            }
            get
            { return _categories; }
        }

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        SubjectAndHotStockCategory _curSelCategory;
        public SubjectAndHotStockCategory CurSelCategory
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

        private void OnCurSelCategoryChange(SubjectAndHotStockCategory category)
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

        int _sort=1;
        public int Sort
        {
            set
            { SetProperty(ref _sort, value, nameof(Sort)); }
            get
            { return _sort; }
        }


        public DelegateCommand SaveCommand { set; get; }
        private async void Save()
        {
            SubjectAndHotStockCategory c = new SubjectAndHotStockCategory();
            c.Name = Name;
            c.Desc = Desc;
            c.SortNum = Sort;
            c.UserId = _passportProvider.UserId;
            c.CreateTime = DateTime.Now;
            if (CurSelCategory != null)
            {
                c.Id = CurSelCategory.Id;
            }

            SubjectAndHotStockCategory uc = null;

            await Task.Run(() =>
            {
                uc = _subjectAndHotService.SaveSubjectAndHotCategory(c);
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
            else
            {
                Sort = 1;
            }
        }

        public DelegateCommand DeleteCommand { set; get; }
        private async void Delete()
        {
            if (CurSelCategory != null)
            {
                await Task.Run(() => {
                    _subjectAndHotService.DelSubjectAndHotStockCategory(CurSelCategory);
                }).ConfigureAwait(true);

                LoadCategories();
            }
        }
    }
}
