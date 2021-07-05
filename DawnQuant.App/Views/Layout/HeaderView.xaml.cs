using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace DawnQuant.App.Views.Layout
{
    /// <summary>
    /// RightHeaderView.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderView : UserControl
    {
        public HeaderView()
        {
            InitializeComponent();
        }

        private void _miDawnloadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadDataWindow loadDataWindow = new DownloadDataWindow();
                loadDataWindow.IsCreateFromLogin = false;
                loadDataWindow.ShowDialog();
                DXMessageBox.Show("下载数据成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                DXMessageBox.Show(ex.Message, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void _miDelData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataPath = Path.Combine(Environment.CurrentDirectory, "Data");
                Directory.Delete(dataPath, true);
                DXMessageBox.Show("删除本地数据成功！", "温馨提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                DXMessageBox.Show(ex.Message, "温馨提示", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
    }
}
