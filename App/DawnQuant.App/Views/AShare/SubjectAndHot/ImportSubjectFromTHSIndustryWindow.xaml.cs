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
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.ViewModels.AShare.SubjectAndHot;
using DevExpress.Xpf.Core;


namespace DawnQuant.App.Views.AShare.SubjectAndHot
{
    /// <summary>
    /// Interaction logic for ImportSubjectFromTHSIndustryWindow.xaml
    /// </summary>
    public partial class ImportSubjectFromTHSIndustryWindow : ThemedWindow
    {
        public ImportSubjectFromTHSIndustryWindow()
        {
            InitializeComponent();

            DataContext = new ImportSubjectFromTHSIndustryWindowModel();
        }

        public ImportSubjectFromTHSIndustryWindowModel ViewModel
        {
            get { return (ImportSubjectFromTHSIndustryWindowModel)DataContext; }
        }

        private void _treeAll_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_treeAll.SelectedItem is Industry industry)
            {
                //第三级别才有效
                if (industry.Level==3)
                {
                    ViewModel.AddIndustryCommand.Execute(industry);
                }
               
            }
        }

        private void _btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_treeAll.SelectedItem is Industry industry)
            {
                //第三级别才有效
                if (industry.Level == 3)
                {
                    ViewModel.AddIndustryCommand.Execute(industry);
                }

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
