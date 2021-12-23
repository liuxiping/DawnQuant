using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Services.AShare;
using DawnQuant.Passport;
using System.Collections.ObjectModel;


namespace DawnQuant.App.Core.ViewModels.AShare.SelfSelStock
{
    public class StockCategoryMgrWindowModel 
    {

        private readonly StockPlotDataService _stockPlotDataService;
        private readonly SelfSelService _selfSelService;
        private readonly IPassportProvider _passportProvider;


        public StockCategoryMgrWindowModel(StockPlotDataService stockPlotDataService,
           SelfSelService selfSelService,
           IPassportProvider passportProvider)
        {
            _stockPlotDataService = stockPlotDataService;
            _selfSelService = selfSelService;
            _passportProvider = passportProvider;

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
        private  void LoadCategories()
        {

            Categories = _selfSelService.GetSelfSelectStockCategories(_passportProvider.UserId);

            if (CurSelCategory == null && Categories != null && Categories.Count > 0)
            {
                CurSelCategory = Categories[0];
            }

        }

        //所有分类
        public ObservableCollection<SelfSelectStockCategory> Categories { get; private set; }
        

        /// <summary>
        /// 当前选择的分类
        /// </summary>
        SelfSelectStockCategory _curSelCategory;
        public SelfSelectStockCategory CurSelCategory
        {
            set
            {
                _curSelCategory = value;
                    OnCurSelCategoryChange(value);
                
                
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

        public string Name { get;  set; }
        

        public string Desc { get; set; }



        public int Sort { get; set; } = 1;


        public bool IsGroup { get; set; }



        private void Save()
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
            var uc = _selfSelService.SaveSelfSelectCategory(c);

            //刷新分类 设置选择
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
        }

        private void Delete()
        {
            if (CurSelCategory != null)
            {
                _selfSelService.DelSelfSelectCategory(CurSelCategory);
                LoadCategories();

            }
        }


        //排序字段数据
        public ObservableCollection<StockSortField> StockSortFields { get; set; }



        //当前选择的排序字段
        public StockSortField CurSelStockSortField { get; set; }



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
