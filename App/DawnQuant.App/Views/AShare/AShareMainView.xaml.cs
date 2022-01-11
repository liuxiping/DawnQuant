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
using DawnQuant.App.Views.AShare.THSDataAnalysis;

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
            _hmAShare.SelectedItem = _btnSelfSelStock;
        }


        SelfSelStockView _selfSelStockView =null;
        StockStrategyView _stockStrategyView = null;
        StrategyScheduledTaskView _strategyScheduledTaskView = null;
        BellwetherView _bellwetherView = null;
        SubjectAndHotView _subjectAndHotView = null;
        SettingView _settingView = null;
        THSDataAnalysisView _thsDataAnalysisView = null;
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

            //自选分类
            if (btn.Name == "_btnSelfSelStock")
            {
                if(_selfSelStockView==null)
                {
                    _selfSelStockView=new SelfSelStockView();
                    _gdContent.Children.Add(_selfSelStockView);
                }
                _selfSelStockView.Visibility = Visibility.Visible;

            }
            //策略
            else if (btn.Name == "_btnStockStrategy")
            {
                if (_stockStrategyView == null)
                {
                    _stockStrategyView = new  StockStrategyView();
                    _gdContent.Children.Add(_stockStrategyView);
                }

                _stockStrategyView.Visibility = Visibility.Visible;
            }
            //策略任务计划
            else if (btn.Name == "_btnScheduledTask")
            {
                if (_strategyScheduledTaskView == null)
                {
                    _strategyScheduledTaskView = new  StrategyScheduledTaskView();
                    _gdContent.Children.Add(_strategyScheduledTaskView);
                }
                _strategyScheduledTaskView.Visibility = Visibility.Visible;
            }
            //龙头股
            else if (btn.Name == "_btnBellwether")
            {
                if (_bellwetherView == null)
                {
                    _bellwetherView = new  BellwetherView();
                    _gdContent.Children.Add(_bellwetherView);
                }
                _bellwetherView.Visibility = Visibility.Visible;
            }
            //题材热点
            else if (btn.Name == "_btnSubjectAndHot")
            {
                if (_subjectAndHotView == null)
                {
                    _subjectAndHotView = new  SubjectAndHotView();
                    _gdContent.Children.Add(_subjectAndHotView);
                }
                _subjectAndHotView.Visibility = Visibility.Visible;
            }
            else if (btn.Name == "_btnSetting")
            {
                if (_settingView == null)
                {
                    _settingView = new  SettingView();
                    _gdContent.Children.Add(_settingView);
                }
                _settingView.Visibility = Visibility.Visible;
            }
            else if(btn.Name == "_btnTHSDataAnalysis")
            {
                if (_thsDataAnalysisView == null)
                {
                    _thsDataAnalysisView = new  THSDataAnalysisView();
                    _gdContent.Children.Add(_thsDataAnalysisView);
                }
                _thsDataAnalysisView.Visibility = Visibility.Visible;
            }
            else
            {
                if (_gdContent.Children.Count > 0)
                {
                    _gdContent.Children[0].Visibility = Visibility.Visible;
                }
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
