using DawnQuant.App.ViewModels.AShare.Common;
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

namespace DawnQuant.App.Views.AShare.Common
{
    /// <summary>
    /// StatisticsWindowView.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticsWindowView : DXWindow
    {
        public StatisticsWindowView()
        {
            InitializeComponent();
        }

       /// <summary>
       /// model
       /// </summary>
        public StatisticsWindowViewModel Model
        {
            get
            {
                return (StatisticsWindowViewModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }
    }
}
