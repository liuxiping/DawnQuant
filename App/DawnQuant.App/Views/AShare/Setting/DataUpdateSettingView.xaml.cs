using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DawnQuant.App.Views.AShare.Setting
{
    /// <summary>
    /// Interaction logic for DataUpdateConfigView.xaml
    /// </summary>
    public partial class DataUpdateSettingView : UserControl
    {
        public DataUpdateSettingView()
        {
            InitializeComponent();

            DataContext = new DataUpdateSettingViewModel();
            ViewModel.CategoriesLoaded += ViewModel_CategoriesLoaded;
        }

        
        private void ViewModel_CategoriesLoaded()
        {
            _spSelfSelCategories.Children.Clear();
            if(ViewModel.Categories != null)
            {
                foreach (var category in ViewModel.Categories)
                {

                    CheckBox check=new CheckBox();
                    check.Width = 150;
                    check.Content = category.Name;
                    check.DataContext = category;
                    check.Padding=new Thickness(5);
                    check.Margin=new Thickness(5);
                    check.IsChecked= IsSelfSelectStockCategoryNeedUpdate(category.Id);
                    _spSelfSelCategories.Children.Add(check);
                }
            }
        }

        private bool IsSelfSelectStockCategoryNeedUpdate(long id)
        {
            var ids = App.AShareSetting?.DataUpdateSetting?.SelfSelCategories;
            if(ids!=null)
            { 
                return ids.Contains(id);
            }
            else
            {
                return false;
            }
        }

        DataUpdateSettingViewModel ViewModel
        {
            get
            {
                return (DataUpdateSettingViewModel)DataContext;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //加载自选分类
           
        }


        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<long> ids = new List<long>();
            foreach(var chk in _spSelfSelCategories.Children)
            {
                CheckBox cb= (CheckBox)chk;

                if(cb.IsChecked.HasValue && cb.IsChecked.Value)
                {

                    ids.Add(((SelfSelectStockCategory)cb.DataContext).Id);
                }
            }

            ViewModel.UpdateSelfSelStockCategoryIds = ids;

            ViewModel.Save();

        }
    }
}
