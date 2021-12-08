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

namespace DawnQuant.App.Views.AShare.SelfSelStock
{
    /// <summary>
    /// SelfSelStockView.xaml 的交互逻辑
    /// </summary>
    public partial class SelfSelStockContainerView : UserControl
    {

        public SelfSelStockContainerView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //if (Model.SelfSelCategorys?.Count() > 0)
            //{
            //    foreach (var c in Model.SelfSelCategorys)
            //    {
            //        bool isExist = false;
            //        foreach (var item in tabContainer.Items)
            //        {
            //            var tabItem = (TabItemExt)item;
            //            if (tabItem.Header.ToString() == c.Name)
            //            {
            //                isExist = true;
            //            }
            //        }

            //        if (!isExist)
            //        {
            //            TabItemExt tabItemExt = new TabItemExt();
            //            tabItemExt.Header = c.Name; ;
            //            tabContainer.Items.Add(tabItemExt);
            //        }
            //    }

            //}
        }

        private void tabContent_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmTabContainer") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
    }
}
