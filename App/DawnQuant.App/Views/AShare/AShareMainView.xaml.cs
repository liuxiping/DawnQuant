using DawnQuant.App.Job;
using DawnQuant.App.Utils;
using DawnQuant.App.ViewModels.AShare;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using Quartz;
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
using Autofac;
using DawnQuant.App.Services.AShare;
using DawnQuant.Passport;
using System.Runtime.InteropServices;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.POCO;
using DawnQuant.App.Views.AShare.SelfSelStock;
using DawnQuant.App.Views.AShare.StockStrategy;
using DawnQuant.App.Views.AShare.StrategyScheduledTask;
using DawnQuant.App.Views.AShare.Bellwether;
using DawnQuant.App.Views.AShare.SubjectAndHot;
using DawnQuant.App.Views.AShare.Setting;

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
            DataContext = new AShareMainViewModel();
           


        }

        public AShareMainViewModel Model { get { return (AShareMainViewModel)DataContext; } }

        private void _aShareMain_Loaded(object sender, RoutedEventArgs e)
        {
            _hmAShare.SelectedItem = _btnSubjectAndHot;
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
            else if (btn.Name == "_btnBellwether")
            {
                _bellwetherView.Visibility = Visibility.Visible;
            }
            else if (btn.Name == "_btnSubjectAndHot")
            {
                _subjectAndHotView.Visibility = Visibility.Visible;
            }
            else if (btn.Name == "_btnSetting")
            {
                _settingView.Visibility = Visibility.Visible;
            }
            else if(btn.Name == "_btnTHSDataAnalysis")
            {
                _thsDataAnalysisView.Visibility = Visibility.Visible;
            }
            else
            {
                _gdContent.Children[0].Visibility = Visibility.Visible;
            }
        }


        public static void SendActivatorMessage()
        {

        }


    }


    [Guid("45FD942D-1AD5-48E7-B139-4E1FB9F1F060"), ComVisible(true)]
    public class DawnQuantAppNotificationActivator : ToastNotificationActivator
    {
        public override void OnActivate(string arguments, Dictionary<string, string> data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AShareMainView.SendActivatorMessage();
            });
        }
    }

}
