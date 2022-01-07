using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.SubjectAndHot;
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

namespace DawnQuant.App.Views.AShare.SubjectAndHot
{
    /// <summary>
    /// SubjectAndHotView.xaml 的交互逻辑
    /// </summary>
    public partial class SubjectAndHotView : UserControl
    {
        public SubjectAndHotView()
        {
            InitializeComponent();
            DataContext = new SubjectAndHotViewModel();
        }

        private SubjectAndHotViewModel Model
        {
            get
            {
                return (SubjectAndHotViewModel)DataContext;
            }
        }

        private void _btnAddSubjectAndHotStockCategoryMgr_Click(object sender, RoutedEventArgs e)
        {
            SubjectAndHotStockCategoryMgrWindow mgrWindow = new SubjectAndHotStockCategoryMgrWindow();

            mgrWindow.ShowDialog();

            //刷新自选股分类
           Model.RefreshSubjectAndHotStockCategories();
        }


        private void _btnAddStock_Click(object sender, RoutedEventArgs e)
        {
            if (Model.CurSelCategory == null)
            {
                DXMessageBox.Show("题材概念分类不能为空！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }


            AddSubjectAndHotStockWindow addSubjectAndHotStockWindow = new AddSubjectAndHotStockWindow();

            addSubjectAndHotStockWindow.Model.StockItemAdded +=
            (item) =>
            {
                Model.Stocks.Insert(0, item);

            };

            addSubjectAndHotStockWindow.Model.SubjectAndHotContext= new SubjectAndHotContext()
            {
                Stocks = Model.Stocks,
                Category = Model.CurSelCategory
            };


            addSubjectAndHotStockWindow.ShowDialog();
        }

        

        private void txtCategorySearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
           // _tvSubjectAndHotStockCategoryList.SearchString = txtCategorySearchString.Text;

        }

        private void _gcSubjectAndHotStockCategoryList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void _gcSubjectAndHotStockList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void _gcFocusStockList_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

       

        private void _subjectAndHotView_KeyDown(object sender, KeyEventArgs e)
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

            if (e.Key == Key.W  )
            {
                _gcFocusStockList.View.MovePrevRow();
              
            }

            if (e.Key == Key.S  )
            {
                _gcFocusStockList.View.MoveNextRow();
             
            }

            if (e.Key == Key.NumPad0 )
            {
                _gcSubjectAndHotStockList.View.MoveNextRow();
              
             
            }

            if (e.Key == Key.NumPad1)
            {
                _gcSubjectAndHotStockList.View.MovePrevRow();
            }

            if (e.Key == Key.Q)
            { 
                _gcSubjectAndHotStockCategoryList.View.MovePrevRow();
            }
            if(e.Key==Key.A)
            {
                _gcSubjectAndHotStockCategoryList.View.MoveNextRow();

            }
        }

        private void _miImportSubjectFromTHS_Click(object sender, RoutedEventArgs e)
        {
            var win = new ImportSubjectFromTHSWindow();

            win.ViewModel.CategoryId = Model.CurSelCategory.Id;

            if (win.ShowDialog() == true)
            {

                Model.RefreshSubjectAndHotStockCategories();
            }

        }
    }
}
