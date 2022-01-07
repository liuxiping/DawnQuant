using DawnQuant.App.ViewModels.AShare.SubjectAndHot;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DawnQuant.App.Views.AShare.SubjectAndHot
{
    /// <summary>
    /// ImportSubjectFromTHSWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImportSubjectFromTHSWindow : Window
    {
        public ImportSubjectFromTHSWindow()
        {
            InitializeComponent();

            DataContext = new ImportSubjectFromTHSWindowModel();
        }

        public ImportSubjectFromTHSWindowModel ViewModel
        {
            get { return (ImportSubjectFromTHSWindowModel)DataContext; }
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private  async void _btnExtract_Click(object sender, RoutedEventArgs e)
        {
            _btnExtract.IsEnabled = false;

            await ViewModel.Extract();

            this.DialogResult = true;
            this.Close();

            DXMessageBox.Show("提取成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);

           
        }
    }
}
