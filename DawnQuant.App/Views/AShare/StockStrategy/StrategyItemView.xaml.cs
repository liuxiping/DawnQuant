using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using DawnQuant.App.ViewModels.AShare.StockStrategy;
using DevExpress.Xpf.Accordion;
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
using UserProfile = DawnQuant.App.Models.AShare.UserProfile;

namespace DawnQuant.App.Views.AShare.StockStrategy
{
    /// <summary>
    /// StrategyItemView.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyItemView : UserControl
    {
        public StrategyItemView()
        {
            InitializeComponent();
        }

        private void viewStrategyItem_MouseEnter(object sender, MouseEventArgs e)
        {
            _btnDel.Visibility = Visibility.Visible;
            _btnEdit.Visibility = Visibility.Visible;
            _btnExecuteStrategy.Visibility = Visibility.Visible;
        }

        private void viewStrategyItem_MouseLeave(object sender, MouseEventArgs e)
        {
            _btnDel.Visibility = Visibility.Collapsed;
            _btnEdit.Visibility = Visibility.Collapsed;
            _btnExecuteStrategy.Visibility = Visibility.Collapsed;

        }

        AccordionControl _acStrategyList = null;

        private void viewStrategyItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is UserProfile.StockStrategy ss)
            {
                var model = new StrategyItemViewModel();
                model.Strategy = ss;
                DataContext = model;
            }

            _acStrategyList = ControlsSearchUtil.GetParentObject<AccordionControl>(this);

        }

        StrategyItemViewModel Model
        {
            get { return (StrategyItemViewModel)DataContext; }
        }

        private async void _btnDel_Click(object sender, RoutedEventArgs e)
        {
            await Model.DelStrategyCommand.ExecuteAsync(null);

            //更新界面

            UpdateCategoriesIncludeStrategies();
        }

        /// <summary>
        /// 策略编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnEdit_Click(object sender, RoutedEventArgs e)
        {

            SetSelectStockStrategy();
            StrategySetupWindow strategySetupWindow = new StrategySetupWindow();

            //编辑状态初始化参数
            strategySetupWindow.Model.Strategy = Model.Strategy;

            if (strategySetupWindow.ShowDialog() == true)
            {
                UpdateCategoriesIncludeStrategies();
               
            }

           
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        private void UpdateCategoriesIncludeStrategies()
        {
            //更新界面
            StockStrategyView ssv = ControlsSearchUtil.GetParentObject<StockStrategyView>(this);

            long cid = 0;

            if (_acStrategyList.SelectedItem is UserProfile.StockStrategy ss)
            {
                cid = ss.CategoryId;
            }

            if (_acStrategyList.SelectedItem is UserProfile.StockStrategyCategory ssc)
            {
                cid = ssc.Id;
            }

            ssv.Model.LoadCategoriesIncludeStrategies();


            ///展开分类
            for (int index = 0; index < _acStrategyList.Items.Count; index++)
            {
                var ca = (StockStrategyCategory)_acStrategyList.Items[index];
                if (ca.Id == cid)
                {
                    _acStrategyList.SelectedItem = _acStrategyList.Items[index];
                    _acStrategyList.ExpandItem(_acStrategyList.Items[index]);
                    break;
                }
            }

            ///设置选择
            SetSelectStockStrategy();
        }

        private void _btnExecuteStrategy_Click(object sender, RoutedEventArgs e)
        {
            OnExecuteStrategyClick(Model.Strategy);
        }


        public event Action<UserProfile.StockStrategy> ExecuteStrategyClick;
        protected  void OnExecuteStrategyClick(UserProfile.StockStrategy stockStrategy)
        {
            ExecuteStrategyClick?.Invoke(stockStrategy);
        }


        /// <summary>
        /// 设置选择
        /// </summary>
        private void  SetSelectStockStrategy()
        {
            UserProfile.StockStrategy stockStrategy = Model.Strategy;
            if (stockStrategy != null)
            {
                bool isFind = false;
                foreach (var c in _acStrategyList.Items)
                {
                    var tssc = (StockStrategyCategory)c;

                    if (tssc.StockStrategies != null && tssc.StockStrategies.Count > 0)
                    {
                        for (int index = 0; index < tssc.StockStrategies.Count; index++)
                        {
                            var tss = tssc.StockStrategies[index];
                            if (tss.Id == stockStrategy.Id)
                            {
                                _acStrategyList.SelectedItem = tssc.StockStrategies[index];
                                isFind = true;
                                break;
                            }
                        }
                    }
                    if (isFind)
                    {
                        break;
                    }
                }
            }

        }

    }
    
}
