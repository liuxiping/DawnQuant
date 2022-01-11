using DawnQuant.App.ViewModels.AShare.THSDataAnalysis.THSIndex;
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

namespace DawnQuant.App.Views.AShare.THSDataAnalysis.THSIndex
{
    /// <summary>
    /// THSIndustryIndexView.xaml 的交互逻辑
    /// </summary>
    public partial class THSIndexView : UserControl
    {
        public THSIndexView()
        {
            InitializeComponent();
            DataContext = new THSIndexViewModel();
        }


        /// <summary>
        /// 填充序列号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _gcTHSIndexList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;
        }

        private void _gcStockList_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;
        }

       

        private void _thsIndexView_KeyDown(object sender, KeyEventArgs e)
        {
           

            if (e.Key == Key.W)
            {
                _gcTHSIndexList.View.MovePrevRow();
              
            }

            if (e.Key == Key.S)
            {
                _gcTHSIndexList.View.MoveNextRow();
            
            }

            if (e.Key == Key.NumPad2 || e.Key == Key.NumPad0)
            {
                _gcStockList.View.MoveNextRow();
            }

            if (e.Key == Key.NumPad8 || e.Key == Key.NumPad1)
            {
                _gcStockList.View.MovePrevRow();
                
            }
        }
    }
}
