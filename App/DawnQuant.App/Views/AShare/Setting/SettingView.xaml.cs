﻿using DevExpress.Xpf.Editors;
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

namespace DawnQuant.App.Views.AShare.Setting
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
        }

       

        private void _lbSetting_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {

            var item = (ListBoxEditItem)_lbSetting.SelectedItem;

            if (item != null)
            {
                foreach (FrameworkElement frameworkElement in _gdContent.Children)
                {
                    frameworkElement.Visibility = Visibility.Collapsed;
                }

                if (item.Name == "_lbeiReadyToBuyStockJobSetting")
                {
                    //_readyToBuyStockJobSettingView.Visibility = Visibility.Visible;
                }
            }
            
        }
    }
}
