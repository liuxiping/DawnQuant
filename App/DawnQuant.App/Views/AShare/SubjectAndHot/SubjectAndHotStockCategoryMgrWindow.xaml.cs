using DawnQuant.App.ViewModels.AShare.SubjectAndHot;
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
using DevExpress.Xpf.Grid;

namespace DawnQuant.App.Views.AShare.SubjectAndHot
{
    /// <summary>
    /// SelfSelectStockCategoryMgrView.xaml 的交互逻辑
    /// </summary>
    public partial class SubjectAndHotStockCategoryMgrWindow : DXWindow
    {
        public SubjectAndHotStockCategoryMgrWindow()
        {
            InitializeComponent();

            DataContext = new SubjectAndHotStockCategoryMgrWindowModel();
        }

        private void _btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _gcSubjectAndHotCategoryStockList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }
    }
}
