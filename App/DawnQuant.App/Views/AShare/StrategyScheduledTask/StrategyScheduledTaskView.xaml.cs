using DawnQuant.App.ViewModels.AShare.StrategyScheduledTask;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DawnQuant.App.Views.AShare.StrategyScheduledTask
{
    /// <summary>
    /// ScheduledTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class StrategyScheduledTaskView : UserControl
    {
        public StrategyScheduledTaskView()
        {
            InitializeComponent();
            DataContext = new StrategyScheduledTaskViewModel();
        }

        private void _btnSelStrategy_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            //刷新选择状态

            _treeAllStrategy.UncheckAllNodes();

            foreach (var pnode in _treeAllStrategy.Nodes)
            {
                foreach (var cnode in pnode.Nodes)
                {
                    var s = (UserProfile.StockStrategy)cnode.Content;

                    if (Model.CurSelStockStrategies.Where(p => p.Id == s.Id).Any())
                    {
                        cnode.IsChecked = true;
                    }
                    else
                    {
                        cnode.IsChecked = false;
                    }
                }
            }


            _popStrategy.IsOpen = true;
        }

       

        StrategyScheduledTaskViewModel Model { get { return (StrategyScheduledTaskViewModel)DataContext; } }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Model.CurSelStockStrategies.Clear();
            //获取选择的策略
            foreach (var pnode in _treeAllStrategy.Nodes)
            {
               foreach(var cnode in pnode.Nodes)
                {
                    if(cnode.IsChecked.HasValue && cnode.IsChecked.Value)
                    {
                        Model.CurSelStockStrategies.Add((UserProfile.StockStrategy)cnode.Content);
                    }
                }
            }

            Model.StrategyName = string.Join(';', Model.CurSelStockStrategies?.Select(p => p.Name).ToList());
            _popStrategy.IsOpen = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _popStrategy.IsOpen = false;
        }

        private void gdPopup_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void _treeAllStrategy_LostFocus(object sender, RoutedEventArgs e)
        {
            //_popStrategy.IsOpen = false;
        }

        private void _gcTasks_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            var view = sender as GridControl;
            if (e.Column.FieldName == "#" && e.IsGetData)
                e.Value = view.GetRowVisibleIndexByHandle(
                    view.GetRowHandleByListIndex(e.ListSourceRowIndex)) + 1;

        }

        private void _chkIsJoinClientScheduleTask_Checked(object sender, RoutedEventArgs e)
        {
            _txtClientScheduleTime.Visibility = Visibility.Visible;
            _tpClientScheduleCron.Visibility = Visibility.Visible;
           // Model.ClientScheduleTime = new DateTime(8888, 8, 8, 14, 35, 0);

        }

        private void _chkIsJoinClientScheduleTask_Unchecked(object sender, RoutedEventArgs e)
        {
            _txtClientScheduleTime.Visibility = Visibility.Collapsed;
            _tpClientScheduleCron.Visibility = Visibility.Collapsed;
        }
    }
}
