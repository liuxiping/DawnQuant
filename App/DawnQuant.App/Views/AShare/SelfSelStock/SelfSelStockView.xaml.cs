using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.SelfSelStock;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class SelfSelStockView : UserControl
    {
        public SelfSelStockView()
        {
            InitializeComponent();
            DataContext = new SelfSelStockViewModel();

        }

        //private void _btnGroup_Checked(object sender, RoutedEventArgs e)
        //{
        //    _gcStockList.GroupBy("Industry");
        //    _gcStockList.ExpandAllGroups();
        //    var textBlock = (TextBlock)_btnGroup.Content;
        //    textBlock.Text = "\ue60f";
        //    _btnGroup.ToolTip = "取消分组";
        //}

        //private void _btnGroup_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    _gcStockList.ClearGrouping();
        //    var textBlock = (TextBlock)_btnGroup.Content;
        //    textBlock.Text = "\ue61a";
        //    _btnGroup.ToolTip = "按行业分组";
        //}


        SelfSelStockViewModel Model
        {
            get { return (SelfSelStockViewModel)this.DataContext; }
        }


        private void _spCategory_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //第一次
            if (e.OldValue == null)
            {
                CreateCategory(null);
            }
            else
            {
                CreateCategory(Model.CurSelCategory);
            }

        }


        /// <summary>
        /// 创建自选股分类按钮
        /// </summary>
        /// <param name="selCategory"></param>
        private void CreateCategory(SelfSelectStockCategory selCategory)
        {
            //创建分类按钮
            _spCategory.Children.Clear();
            int i = 0;
            foreach (var c in Model.Categories)
            {
                ToggleButton btn = new ToggleButton();
                btn.Content = c.Name;
                btn.Command = Model.CategorySelChangedCommand;
                btn.CommandParameter = c;

                btn.Padding = new Thickness(5, 3, 5, 3);
                btn.Click += BtnCategory_Click;
                if (i == 0)
                {
                    btn.Margin = new Thickness(3, 3, 8, 3);
                    BtnCategory_Click(btn, null);

                    if (selCategory == null)
                    {
                        BtnCategory_Click(btn, null);
                        btn.Command.Execute(btn.CommandParameter);
                    }
                }
                else
                {
                    btn.Margin = new Thickness(0, 3, 8, 3);
                }

                _spCategory.Children.Add(btn);

                if (c.Id == selCategory?.Id)
                {
                    BtnCategory_Click(btn, null);
                    btn.Command.Execute(btn.CommandParameter);
                }
                i++;

            }
        }

        /// <summary>
        /// 只能选择一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btnCur = (ToggleButton)sender;
            foreach (var ctrl in _spCategory.Children)
            {
                ToggleButton btn = (ToggleButton)ctrl;
                btn.IsChecked = false;
            }
            btnCur.IsChecked = true;

            //创建移动分类菜单
            _miCategory.Items.Clear();

            foreach (var c in Model.Categories)
            {
                //  SelfSelectStockCategory selCategory = (SelfSelectStockCategory)btnCur.CommandParameter;
                // if (selCategory.Id != c.Id)
                {
                    MenuItem menuItem = new MenuItem { Header = c.Name };
                    menuItem.Command = Model.MoveToOtherCategoryCommand;
                    menuItem.CommandParameter = c;
                    _miCategory.Items.Add(menuItem);
                }

            }

        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _selfSelStockView_KeyDown(object sender, KeyEventArgs e)
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
                _gcStockList.View.MovePrevRow();
            }

            if (e.Key == Key.S )
            {
                _gcStockList.View.MoveNextRow();
            }

            if(e.Key==Key.NumPad2||e.Key==Key.NumPad0)
            {
                _gcRelateStockList.View.MoveNextRow();
            }

            if (e.Key == Key.NumPad8 || e.Key == Key.NumPad1)
            {
                _gcRelateStockList.View.MovePrevRow();
            }
        }

        private void _selfSelStockView_Loaded(object sender, RoutedEventArgs e)
        {

            _gcStockList.Focus();
        }

        /// <summary>
        /// 导出股票列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _miExportTSCodes_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "Text Document(*.txt)|*.txt|All Files|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                var tsCodes = Model.Stocks?.Select(p => p.TSCode);
                if (tsCodes != null && tsCodes.Count() > 0)
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        StreamWriter sw = new StreamWriter(fs);

                        foreach (var tsCode in tsCodes)
                        {
                            sw.WriteLine(tsCode.Substring(0, 6));
                        }
                        sw.Close();
                    }
                    DXMessageBox.Show("导出股票代码成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        /// <summary>
        /// 自选股分类管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnCategoryMgr_Click(object sender, RoutedEventArgs e)
        {
            StockCategoryMgrWindow mgrWindow = new StockCategoryMgrWindow();

            mgrWindow.ShowDialog();

            //刷新自选股分类
            Model.RefreshSelfSelectStockCategories();
        }


        /// <summary>
        /// 添加股票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnAddStock_Click(object sender, RoutedEventArgs e)
        {

            AddSelfStockWindow addSelfStockWindow = new AddSelfStockWindow();

            addSelfStockWindow.Model.StockItemAdded +=
            (item) =>
            {
                Model.Stocks.Insert(0, item);

            };

            addSelfStockWindow.Model.SelfSelectContext = new SelfSelectContext()
            {
                Stocks = Model.Stocks,
                Category = Model.CurSelCategory
            };


            addSelfStockWindow.ShowDialog();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _miImportTSCodes_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "txt";
            openFileDialog.Filter = "Text Document(*.txt)|*.txt|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                List<string> stocksId = new List<string>();

                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    StreamReader sr = new StreamReader(fs);

                    string stockId;

                    while ((stockId = sr.ReadLine()) != null)

                    {
                        stocksId.Add(stockId);

                    }
                    sr.Close();

                    //导入自选股
                    Model.ImportSelfStocksCommand.Execute(stocksId);
                    DXMessageBox.Show("股票导入成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        #region 填充序号列数据
        private void _gcStockList_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
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
        #endregion
    }
}

