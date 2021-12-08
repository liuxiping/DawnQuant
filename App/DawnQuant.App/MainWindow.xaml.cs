using DawnQuant.App.Utils;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using DevExpress.Mvvm.POCO;
using DawnQuant.App.ViewModels;

namespace DawnQuant.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXTabbedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModelSource.Create<MainWindowModel>();
        }

        private void _mainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void _mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowModel model = (MainWindowModel)DataContext;
            model.ShowMainWindow += Model_ShowMainWindow;
        }

        private void Model_ShowMainWindow()
        {
            _biShowMainWindow_ItemClick(null, null);
        }

        private void _biShowMainWindow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.ShowInTaskbar = true;
            WindowState = WindowState.Maximized;
           // this.Show();
            this.Activate();
        }

        private void _biExit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            e.Cancel = true;
        }
    }
}
