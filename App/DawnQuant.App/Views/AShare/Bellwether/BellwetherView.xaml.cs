using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.Bellwether;
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
    /// BellwetherView.xaml 的交互逻辑
    /// </summary>
    public partial class BellwetherView : UserControl
    {
        public BellwetherView()
        {
            InitializeComponent();
            DataContext = new BellwetherViewModel();

        }

        private BellwetherViewModel Model
        {
            get
            {
                return (BellwetherViewModel)DataContext;
            }
        }

        private void _btnAddBellwetherStockCategoryMgr_Click(object sender, RoutedEventArgs e)
        {
            BellwetherStockCategoryMgrWindow mgrWindow = new BellwetherStockCategoryMgrWindow();

            mgrWindow.ShowDialog();

            //刷新自选股分类
            Model.RefreshBellwetherStockCategories();
        }


        private void _btnAddStock_Click(object sender, RoutedEventArgs e)
        {

            if(Model.CurSelCategory==null)
            {
                DXMessageBox.Show("龙头股分类不能为空！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            AddBellwetherStockWindow addBellwetherStockWindow = new AddBellwetherStockWindow();

            addBellwetherStockWindow.Model.StockItemAdded +=
            (item) =>
            {
                Model.Stocks.Insert(0, item);

            };

            addBellwetherStockWindow.Model.BellwetherContext= new BellwetherContext()
            {
                Stocks = Model.Stocks,
                Category = Model.CurSelCategory
            };


            addBellwetherStockWindow.ShowDialog();
        }



        private void _gcBellwetherStockCategoryList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex( e.ListSourceRowIndex)) + 1;
               

        }

        private void _gcBellwetherStockList_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void _gcRelateStockList_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void _bellwetherView_KeyDown(object sender, KeyEventArgs e)
        {
            ///删除当前自选股
            if (e.Key == Key.Delete)
            {
                Model.DelStockItemCommand.Execute(null);
            }
            //添加自选股
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {

                _btnAddStock_Click(null, null);

            }

            if (e.Key == Key.W )
            {
                _gcBellwetherStockList.View.MovePrevRow();
            }

            if (e.Key == Key.S )
            {
                _gcBellwetherStockList.View.MoveNextRow();
            }

            if (e.Key == Key.NumPad0)
            {
                _gcRelateStockList.View.MoveNextRow();
            }

            if (e.Key == Key.NumPad1)
            {
                _gcRelateStockList.View.MovePrevRow();
            }
        }
    }
}
