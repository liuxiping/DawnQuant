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
using System.Windows.Shapes;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.SubjectAndHot;
using DevExpress.Xpf.Core;


namespace DawnQuant.App.Views.AShare.SubjectAndHot
{
    /// <summary>
    /// Interaction logic for MergeSubjectAndHotWindow.xaml
    /// </summary>
    public partial class MergeSubjectAndHotWindow : ThemedWindow
    {
        public MergeSubjectAndHotWindow()
        {
            InitializeComponent();
            DataContext = new MergeSubjectAndHotWindowModel();
        }

        public MergeSubjectAndHotWindowModel ViewModel
        {
            get { return (MergeSubjectAndHotWindowModel)DataContext; }
        }

        private void _treeAll_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_treeAll.SelectedItem is SubjectAndHotStockCategory category)
            {
                
                    ViewModel.AddCategoryCommand.Execute(category);
                

            }
        }

        private void _btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_treeAll.SelectedItem is SubjectAndHotStockCategory category)
            {
                ViewModel.AddCategoryCommand.Execute(category);

            }
        }

        private void _btnOK_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveCommand.Execute(null);
            this.DialogResult = true;
            this.Close();
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
