using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    public class MergeSubjectAndHotWindowModel : ViewModelBase
    {

        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly SelfSelService _selfSelService;

        public MergeSubjectAndHotWindowModel()
        {
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();

            AddCategoryCommand = new DelegateCommand<SubjectAndHotStockCategory>(AddCategory);
            DelAddedCategoryCommand = new DelegateCommand<SubjectAndHotStockCategory>(DelAddedCategory);
            SaveCommand = new DelegateCommand(Save);

            Initialize();
        }

        private void Initialize()
        {
            LoadCategories();
        }

        /// <summary>
        /// 加载分类
        /// </summary>
        private async void LoadCategories()
        {
            ObservableCollection<SubjectAndHotStockCategory> categories = null;

            await Task.Run(() =>
            {
                categories = _subjectAndHotService.GetSubjectAndHotStockCategories();

            }).ConfigureAwait(true);

            Categories = categories;
        }

        public SubjectAndHotStockCategory Category { set; get; }


        ObservableCollection<SubjectAndHotStockCategory> _categories;
        public ObservableCollection<SubjectAndHotStockCategory> Categories
        {
            get { return _categories; }
            set
            {
                SetProperty(ref _categories, value, nameof(Categories));
            }
        }

        SubjectAndHotStockCategory _curSelAddedCategory;
        public SubjectAndHotStockCategory CurSelAddedCategory
        {
            get { return _curSelAddedCategory; }
            set
            {
                SetProperty(ref _curSelAddedCategory, value, nameof(CurSelAddedCategory));
            }
        }

        /// <summary>
        /// 添加的行业
        /// </summary>
        ObservableCollection<SubjectAndHotStockCategory> _addedCategories;
        public ObservableCollection<SubjectAndHotStockCategory> AddedCategories
        {
            get { return _addedCategories; }
            set
            {
                SetProperty(ref _addedCategories, value, nameof(AddedCategories));
            }
        }


        public DelegateCommand<SubjectAndHotStockCategory> AddCategoryCommand { get; set; }
        private void AddCategory(SubjectAndHotStockCategory category)
        {
            if (AddedCategories == null)
            {
                AddedCategories = new ObservableCollection<SubjectAndHotStockCategory>();
            }

            if (!AddedCategories.Contains(category))
            {

                AddedCategories.Add(category);
            }
        }

        public DelegateCommand<SubjectAndHotStockCategory> DelAddedCategoryCommand { get; set; }
        private void DelAddedCategory(SubjectAndHotStockCategory category)
        {
            if (category != null)
            {
                AddedCategories.Remove(category);
            }

        }

        public DelegateCommand SaveCommand { get; set; }
        private void Save()
        {
            if (AddedCategories != null && AddedCategories.Count > 0)
            {
                var categories = AddedCategories.Select(p => p.Id).ToList();

                _subjectAndHotService.MergeSubjectAndHotStockCategory(Category.Id, categories);
            }
        }

    }

}

