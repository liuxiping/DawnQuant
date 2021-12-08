using DawnQuant.App.Models.AShare.StrategyMetadata;
using DawnQuant.App.ViewModels.AShare.StockStrategy;
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

namespace DawnQuant.App.Views.AShare.StockStrategy
{
    /// <summary>
    /// SelectScopeView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectScopeView : UserControl
    {
        public SelectScopeView()
        {
            InitializeComponent();
        }

        public SelectScopeViewModel Model {
            set { DataContext = value; }
            get { return (SelectScopeViewModel)this.DataContext; }
        }

        private void _treeAll_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           

            if(_treeAll.SelectedItem is SelectScopeMetadata scope)
            {
                Model.AddSelectScopeCommand.Execute(scope);
            }
        }


        private void _btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_treeAll.SelectedItem is SelectScopeMetadata scope)
            {
                Model.AddSelectScopeCommand.Execute(scope);
            }
        }

        
    }
}
