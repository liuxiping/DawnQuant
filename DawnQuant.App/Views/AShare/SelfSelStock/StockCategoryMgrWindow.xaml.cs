using DawnQuant.App.ViewModels.AShare.SelfSelStock;
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

namespace DawnQuant.App.Views.AShare.SelfSelStock
{
    /// <summary>
    /// SelfSelectStockCategoryMgrView.xaml 的交互逻辑
    /// </summary>
    public partial class StockCategoryMgrWindow : DXWindow
    {
        public StockCategoryMgrWindow()
        {
            InitializeComponent();

            DataContext = new StockCategoryMgrWindowModel();
        }


       

        private void _btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
