using DawnQuant.App.ViewModels.AShare;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
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

namespace DawnQuant.App.Views.AShare
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class AShareMainView : UserControl
    {
        public AShareMainView()
        {
            InitializeComponent();
        }

        private void _aShareMain_Loaded(object sender, RoutedEventArgs e)
        {
            _hmAShare.SelectedItem = _btnSelfSelStock;
        }

        private void _hmAShare_SelectedItemChanged(object sender, DevExpress.Xpf.WindowsUI.HamburgerMenuSelectedItemChangedEventArgs e)
        {
            HamburgerMenuNavigationButton btn = e.NewItem as HamburgerMenuNavigationButton;

            if (btn != null)
            {
                foreach (FrameworkElement frameworkElement in _gdContent.Children)
                {
                    frameworkElement.Visibility = Visibility.Collapsed;
                }

                if (btn.Name == "_btnSelfSelStock")
                {
                     _selfSelStockView.Visibility = Visibility.Visible;
                }
                else if (btn.Name == "_btnStockStrategy")
                {
                   _stockStrategyView.Visibility = Visibility.Visible;
                }
                else if (btn.Name == "_btnScheduledTask")
                {
                    _strategyScheduledTaskView.Visibility = Visibility.Visible;
                }
                else
                {
                    _gdContent.Children[0].Visibility = Visibility.Visible;
                }
            }

        }

    }

}
