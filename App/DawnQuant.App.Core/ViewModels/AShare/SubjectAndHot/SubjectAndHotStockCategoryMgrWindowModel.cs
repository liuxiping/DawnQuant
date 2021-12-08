using System.Collections.ObjectModel;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;

namespace DawnQuant.App.Core.ViewModels.AShare.SubjectAndHot
{
    class SubjectAndHotStockCategoryMgrWindowModel
    {
        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly IPassportProvider _passportProvider;


        public SubjectAndHotStockCategoryMgrWindowModel(SubjectAndHotService subjectAndHotService,
            IPassportProvider passportProvider)
        { 
            _subjectAndHotService = subjectAndHotService;
            _passportProvider = passportProvider;

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
        /// 加载龙头股分类分类
        /// </summary>
        private void LoadCategories()
        {
            Categories = _subjectAndHotService.GetSubjectAndHotStockCategories(_passportProvider.UserId);

            if(CurSelCategory==null && Categories!=null && Categories.Count>0)
            {
                CurSelCategory = Categories.First();
            }
        }



        //所有龙头股分类
        public ObservableCollection<SubjectAndHotStockCategory> Categories { set; get; }
        

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        SubjectAndHotStockCategory _curSelCategory;
        public SubjectAndHotStockCategory CurSelCategory
        {
            set
            {
                _curSelCategory=value;
                    OnCurSelCategoryChange(value);
                

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

        public string Name { set; get; }


        public string Desc { set; get; }


        public int Sort { set; get; } = 1;



        private void Save()
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

            var uc = _subjectAndHotService.SaveSubjectAndHotCategory(c);

            LoadCategories();

            CurSelCategory = Categories.Where(p => p.Id == uc.Id).FirstOrDefault();

        }

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

        private void Delete()
        {
            if (CurSelCategory != null)
            {
                _subjectAndHotService.DelSubjectAndHotStockCategory(CurSelCategory);
                LoadCategories();

            }
        }
    }
}
