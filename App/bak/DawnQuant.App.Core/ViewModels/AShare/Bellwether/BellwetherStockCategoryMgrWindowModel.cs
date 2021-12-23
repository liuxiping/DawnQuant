using System.Collections.ObjectModel;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;

namespace DawnQuant.App.Core.ViewModels.AShare.Bellwether
{
    class BellwetherStockCategoryMgrWindowModel
    {
        private readonly BellwetherService _bellwetherService;
        private readonly IPassportProvider _passportProvider;


        public BellwetherStockCategoryMgrWindowModel(BellwetherService bellwetherService,
            IPassportProvider passportProvider)
        {

            _bellwetherService = bellwetherService;
            _passportProvider = passportProvider;

            //加载龙头股类目
            LoadCategories();
        }


        /// <summary>
        /// 加载龙头股分类分类
        /// </summary>
        private void LoadCategories()
        {

            Categories = _bellwetherService.GetBellwetherStockCategories(_passportProvider.UserId);
            if (CurSelCategory == null && Categories != null && Categories.Count > 0)
            {
                CurSelCategory = Categories.First();
            }
        }



        //所有龙头股分类
        public ObservableCollection<BellwetherStockCategory> Categories { get;  set; }   
        

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        BellwetherStockCategory _curSelCategory;
        public BellwetherStockCategory CurSelCategory
        {
            set
            {
                _curSelCategory = value;
                OnCurSelCategoryChange(value);
            }
            get
            { return _curSelCategory; }
        }

        private void OnCurSelCategoryChange(BellwetherStockCategory category)
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

        public void Save()
        {
            BellwetherStockCategory c = new BellwetherStockCategory();
            c.Name = Name;
            c.Desc = Desc;
            c.SortNum = Sort;
            c.UserId = _passportProvider.UserId;
            c.CreateTime = DateTime.Now;
            if (CurSelCategory != null)
            {
                c.Id = CurSelCategory.Id;
            }

            var uc = _bellwetherService.SaveBellwetherCategory(c);

            LoadCategories();

            CurSelCategory = Categories.Where(p => p.Id == uc.Id).FirstOrDefault();

        }

        public void New()
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

        public void Delete()
        {
            if (CurSelCategory != null)
            {
                _bellwetherService.DelBellwetherStockCategory(CurSelCategory);
                LoadCategories();

            }
        }
    }
}
