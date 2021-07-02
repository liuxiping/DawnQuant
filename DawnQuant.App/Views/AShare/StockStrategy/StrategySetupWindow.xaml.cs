using DawnQuant.App.ViewModels.AShare.StockStrategy;
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
using System.Windows.Shapes;

namespace DawnQuant.App.Views.AShare.StockStrategy
{
    /// <summary>
    /// StrategySetupView.xaml 的交互逻辑
    /// </summary>
    public partial class StrategySetupWindow : DXWindow
    {
        public StrategySetupWindow()
        {
            InitializeComponent();

            DataContext = new StrategySetupWindowModel();
        }

      public  StrategySetupWindowModel Model { get { return (StrategySetupWindowModel)this.DataContext; } }

        private void winStrategySetupView_Loaded(object sender, RoutedEventArgs e)
        {
            Model.InitStrategyExecutorInsDescriptor();
            //初始化数据
            _ucScopeView.Model = Model.ScopeViewModel;
            _ucFactorView.Model = Model.FactorViewModel;
            _ucBasicInfoView.Model = Model.BasicInfoViewModel;
        }

        private void Wizard_Finish(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg="";
            //检查数据有效性
            //选股范围
            if(_ucScopeView.Model.SelectedScopeMetadatas==null ||
                _ucScopeView.Model.SelectedScopeMetadatas.Count<=0)
            {
                msg += "选股范围不能为空\r\n";
            }
            //选股因子
            if (_ucFactorView.Model.SelectedFactors == null ||
                _ucFactorView.Model.SelectedFactors.Count <= 0)
            {
                msg += "选股因子不能为空\r\n";
            }

            
            if(string.IsNullOrEmpty(_ucBasicInfoView.Model.Name))
            {
                msg += "策略名称不能为空\r\n";
            }

            if (_ucBasicInfoView.Model.CurSelStockStrategyCategory==null)
            {
                msg += "策略分类不能为空\r\n";
            }

            if(string.IsNullOrEmpty(msg))
            {
                Model.ScopeViewModel = _ucScopeView.Model;
                Model.FactorViewModel = _ucFactorView.Model;
                Model.BasicInfoViewModel = _ucBasicInfoView.Model;
                Model.FinishCommand.Execute(null);
                this.DialogResult = true;
            }
            else
            {
                DXMessageBox.Show(msg, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Cancel = true;
            }

           
        }
    }
}
