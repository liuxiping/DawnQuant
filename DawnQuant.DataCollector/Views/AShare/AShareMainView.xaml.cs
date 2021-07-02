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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DawnQuant.DataCollector.Views.AShare
{
    /// <summary>
    /// AShareView.xaml 的交互逻辑
    /// </summary>
    public partial class AShareMainView : UserControl
    {
        public AShareMainView()
        {
            InitializeComponent();
        }

        private void hmMain_SelectedItemChanged(object sender, DevExpress.Xpf.WindowsUI.HamburgerMenuSelectedItemChangedEventArgs e)
        {
            HamburgerMenuNavigationButton btn = e.NewItem as HamburgerMenuNavigationButton ;

            if (btn != null)
            {
               foreach(FrameworkElement frameworkElement in _gdContent.Children)
                {
                    frameworkElement.Visibility = Visibility.Collapsed;
                }

                if (btn.Name == "_btnIncrement")
                {
                    _increment.Visibility = Visibility.Visible;
                }
                else if (btn.Name == "_btnHistory")
                {
                    _history.Visibility = Visibility.Visible;
                }
                else
                {
                    _gdContent.Children[0].Visibility = Visibility.Visible;
                }
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hmMain.SelectedItem = _btnIncrement;
        }
    }
}
