using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels.AShare.SubjectAndHot
{
    public class ImportSubjectFromTHSIndustryWindowModel : ViewModelBase
    {

        private readonly SubjectAndHotService _subjectAndHotService;
        private readonly SelfSelService  _selfSelService;

        public ImportSubjectFromTHSIndustryWindowModel()
        {
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();

            AddIndustryCommand = new DelegateCommand<Industry>(AddIndustry);
            DelAddedIndustryCommand=new DelegateCommand<Industry>(DelAddedIndustry);
            SaveCommand=new DelegateCommand(Save);

            Initialize();
        }

        private void Initialize()
        {
            LoadIndustries();
        }

        /// <summary>
        /// 加载行业
        /// </summary>
        private async void LoadIndustries()
        {
            ObservableCollection<Industry> i = null;

            await Task.Run(() =>
             {
                 i = _subjectAndHotService.GetAllInustries();

             }).ConfigureAwait(true);

            Industries = i;
        }

        public SubjectAndHotStockCategory Category {set;get;}


        ObservableCollection<Industry> industries;
        public ObservableCollection<Industry> Industries
        {
            get { return industries; }
            set
            {
                SetProperty(ref industries, value, nameof(Industries));
            }
        }

        Industry _curSelAddedIndustry;
        public Industry CurSelAddedIndustry
        {
            get { return _curSelAddedIndustry; }
            set
            {
                SetProperty(ref _curSelAddedIndustry, value, nameof(CurSelAddedIndustry));
            }
        }

        /// <summary>
        /// 添加的行业
        /// </summary>
        ObservableCollection<Industry> _addedIndustries;
        public ObservableCollection<Industry> AddedIndustries
        {
            get { return _addedIndustries; }
            set
            {
                SetProperty(ref _addedIndustries, value, nameof(AddedIndustries));
            }
        }


        public DelegateCommand<Industry> AddIndustryCommand { get; set; }
        private void AddIndustry(Industry industry)
        {
            if (AddedIndustries == null)
            {
                AddedIndustries = new ObservableCollection<Industry>();
            }
            if (!Industries.Contains(industry))
            {
                AddedIndustries.Add(industry);
            }
        }

        public DelegateCommand<Industry> DelAddedIndustryCommand { get; set; }
        private void DelAddedIndustry(Industry industry)
        {
            if (industry != null)
            {
                AddedIndustries.Remove(industry);
            }
            
        }

        public DelegateCommand SaveCommand { get; set; }
        private void Save()
        {
            if (AddedIndustries != null && AddedIndustries.Count > 0)
            {
                var industries = AddedIndustries.Select(p => p.Id).ToList();

                _subjectAndHotService.ImportSubjectAndHotStocksByIndustries(Category.Id, industries);
            }
        }

    }
}
