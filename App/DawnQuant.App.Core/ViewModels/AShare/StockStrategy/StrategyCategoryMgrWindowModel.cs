using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.App.Core.Utils;
using DawnQuant.Passport;
using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.ViewModels.AShare.StockStrategy
{
    public class StrategyCategoryMgrWindowModel
    {
        private readonly StockStrategyService  _stockStrategyService;
        private readonly IPassportProvider _passportProvider;


        public StrategyCategoryMgrWindowModel(StockStrategyService stockStrategyService,
            IPassportProvider passportProvider)
        {
            _stockStrategyService = stockStrategyService;
            _passportProvider = passportProvider;

           

            Task.Run(() =>
            {
                LoadCategories();
            });
        }

        private void LoadCategories()
        {
            StockStrategyCategories = _stockStrategyService.GetStockStrategyCategoriesByUser(_passportProvider.UserId);
            if(CurSelStockStrategyCategory==null && StockStrategyCategories != null && StockStrategyCategories.Count>0)
            {
                CurSelStockStrategyCategory = StockStrategyCategories[0];

            }

        }

        //所有分类
        public ObservableCollection<StockStrategyCategory> StockStrategyCategories { get; private set; }
        

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        StockStrategyCategory _curSelStockStrategyCategory;
        public StockStrategyCategory CurSelStockStrategyCategory
        {
            set
            {

                _curSelStockStrategyCategory = value;
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

        public string Name { set; get; }
        

        public string Desc { set; get; }


        public int Sort { set; get; } = 1;
        

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

        private void New()
        {
            CurSelStockStrategyCategory = null;
            if(StockStrategyCategories!=null && StockStrategyCategories.Count>0)
            {
                Sort = StockStrategyCategories.Last().SortNum + 1;
            }
        }

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
