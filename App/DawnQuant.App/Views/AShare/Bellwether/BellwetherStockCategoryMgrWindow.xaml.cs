using DawnQuant.App.ViewModels.AShare.Bellwether;
using DawnQuant.App.ViewModels.AShare.SelfSelStock;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
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

namespace DawnQuant.App.Views.AShare.Bellwether
{
    /// <summary>
    /// SelfSelectStockCategoryMgrView.xaml 的交互逻辑
    /// </summary>
    public partial class BellwetherStockCategoryMgrWindow : DXWindow
    {
        public BellwetherStockCategoryMgrWindow()
        {
            InitializeComponent();
            DataContext = new BellwetherStockCategoryMgrWindowModel();

        }




        private void _btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _gcBellwetherCategoryStockList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void DXWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
