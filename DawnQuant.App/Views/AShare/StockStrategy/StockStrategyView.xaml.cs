using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.StockStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;

namespace DawnQuant.App.Views.AShare.StockStrategy
{
    /// <summary>
    /// StockStrategyView.xaml 的交互逻辑
    /// </summary>
    public partial class StockStrategyView : UserControl
    {
        public StockStrategyView()
        {
            InitializeComponent();

            DataContext = new StockStrategyViewModel();
        }

        private void _btnGroup_Checked(object sender, RoutedEventArgs e)
        {
            _gcStockList.GroupBy("Industry");
            _gcStockList.ExpandAllGroups();
            var textBlock = (TextBlock)_btnGroup.Content;
            textBlock.Text = "\ue60f";
            _btnGroup.ToolTip = "取消分组";
        }

        private void _btnGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            _gcStockList.ClearGrouping();
            var textBlock = (TextBlock)_btnGroup.Content;
            textBlock.Text = "\ue61a";
            _btnGroup.ToolTip = "按行业分组";

        }

        public StockStrategyViewModel Model
        {
            get
            {
                return (StockStrategyViewModel)this.DataContext;
            }

        }

        /// <summary>
        /// 策略选择变更 执行当前策略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _acStrategyList_SelectedItemChanged(object sender, DevExpress.Xpf.Accordion.AccordionSelectedItemChangedEventArgs e)
        {
            if (_acStrategyList.SelectedItem is UserProfile.StockStrategy ss)
            {

                _waiting.Visibility = Visibility.Visible;

                RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)_ssvChart.ActualWidth, (int)_ssvChart.ActualHeight, 96d, 96d, PixelFormats.Default);
                targetBitmap.Render(_ssvChart);
                _imgChart.Source = BitmapFrame.Create(targetBitmap);
                _ssvChart.Visibility = Visibility.Collapsed;
                _imgChart.Visibility = Visibility.Visible;


                await Model.ExecuteStrategyCommand.ExecuteAsync(ss);
                _waiting.Visibility = Visibility.Collapsed;
                _ssvChart.Visibility = Visibility.Visible;
                _imgChart.Visibility = Visibility.Collapsed;

            }
          
        }


        /// <summary>
        /// 策略分类管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnCategoryMgr_Click(object sender, RoutedEventArgs e)
        {
            StrategyCategoryMgrWindow mgrWindow = new StrategyCategoryMgrWindow();
            mgrWindow.ShowDialog();
            Model.LoadCategoriesIncludeStrategies();
        }

        /// <summary>
        /// 策略建立
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void btnNewStrategy_Click(object sender, RoutedEventArgs e)
        {
            StrategySetupWindow strategySetupWindow = new StrategySetupWindow();

        

            if (strategySetupWindow.ShowDialog()== true)
            {
                long cid = 0;

                if (_acStrategyList.SelectedItem is UserProfile.StockStrategy ss)
                {
                    cid = ss.CategoryId;
                }
                
                if(_acStrategyList.SelectedItem is UserProfile.StockStrategyCategory ssc)
                {
                    cid = ssc.Id;
                }
                   

                 Model.LoadCategoriesIncludeStrategies();


                for (int index = 0; index < _acStrategyList.Items.Count; index++)
                {
                    var ca = (StockStrategyCategory)_acStrategyList.Items[index];
                    if (ca.Id == cid)
                    {
                        _acStrategyList.SelectedItem=_acStrategyList.Items[index];
                        _acStrategyList.ExpandItem(_acStrategyList.Items[index]);
                        break;
                    }
                }

            }
        }

        private void StrategySetupWindow_Closed(object sender, EventArgs e)
        {
            //StrategySetupWindow strategySetupWindow = (StrategySetupWindow)sender;
            //if (strategySetupWindow.DialogResult == true)
            //{
            //    long cid = 0;

            //    if (_acStrategyList.SelectedItem is UserProfile.StockStrategy ss)
            //    {
            //        cid = ss.CategoryId;
            //    }

            //    if (_acStrategyList.SelectedItem is UserProfile.StockStrategyCategory ssc)
            //    {
            //        cid = ssc.Id;
            //    }


            //    Model.LoadCategoriesIncludeStrategies();


            //    for (int index = 0; index < _acStrategyList.Items.Count; index++)
            //    {
            //        var ca = (StockStrategyCategory)_acStrategyList.Items[index];
            //        if (ca.Id == cid)
            //        {
            //            _acStrategyList.ExpandItem(_acStrategyList.Items[index]);
            //            break;
            //        }
            //    }

            //}

        }




        /// <summary>
        /// 创建自选股分类菜单按钮
        /// </summary>
        /// <param name="selCategory"></param>
        private void CreateMoveToCategoryMenuItem( )
        {
            _miCategory.Items.Clear();

            foreach (var c in Model.SelfSelectStockCategories)
            {

                MenuItem menuItem = new MenuItem { Header = c.Name };
                menuItem.Command = Model.MoveToOtherCategoryCommand;
                menuItem.CommandParameter = c;
                _miCategory.Items.Add(menuItem);

            }
        }

        private void _stockStrategyView_Loaded(object sender, RoutedEventArgs e)
        {
            CreateMoveToCategoryMenuItem();
            if (_acStrategyList.Items.Count > 0)
            {
                _acStrategyList.SelectedItem = _acStrategyList.Items[0];
                _acStrategyList.ExpandItem(_acStrategyList.Items[0]);
            }
        }

        private void _stockStrategyView_KeyDown(object sender, KeyEventArgs e)
        {
            ///删除当前自选股
            if (e.Key == Key.Delete)
            {
                Model.DelStockItemCommand.Execute(null);
            }

            if (e.Key == Key.W || e.Key == Key.Up)
            {
                _gcStockList.View.MovePrevRow();
            }

            if (e.Key == Key.S || e.Key == Key.Down)
            {
                _gcStockList.View.MoveNextRow();
            }
        }
    }
}
