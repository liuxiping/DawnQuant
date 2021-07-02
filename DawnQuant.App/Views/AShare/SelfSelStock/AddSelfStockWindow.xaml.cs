using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.ViewModels.AShare.SelfSelStock;
using DevExpress.Xpf.Core;
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
using System.Windows.Shapes;

namespace DawnQuant.App.Views.AShare.SelfSelStock
{
    /// <summary>
    /// AddSelfStockView.xaml 的交互逻辑
    /// </summary>
    public partial class AddSelfStockWindow : DXWindow
    {
        public AddSelfStockWindow()
        {
            InitializeComponent();

            DataContext = new AddSelfStockWindowModel();
        }


        public AddSelfStockWindowModel Model
        {
            get { return (AddSelfStockWindowModel)DataContext; }
        }

        private void _addSelfSelectStockWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _txtPattern.Focus();
        }


        List<Button> buttons = new List<Button>();
        private void Button_Click(object sender, RoutedEventArgs e)
         {
            Button btn = (Button)sender;
            var textBlock = (TextBlock)((Button)sender).Content;
            textBlock.Text = "已添加";
            
            btn.IsEnabled = false;

            if(!buttons.Contains(btn))
            {
                buttons.Add(btn);
            }
            //触发命令
            Model.AddStockItemCommand.Execute((SelfSelectStock)_gcStockList.SelectedItem);
        }

       
        private void _gcStockList_ItemsSourceChanged(object sender, DevExpress.Xpf.Grid.ItemsSourceChangedEventArgs e)
        {
            //还原按钮状态
           foreach(Button b in buttons)
            {
                b.IsEnabled = true;
                var textBlock = (TextBlock)((Button)b).Content;
                textBlock.Text = "\ue61f";
            }

        }

        

        private void _txtPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void _txtPattern_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Enter && _txtPattern.Text.Length>1)
            {
                Model.PopulateSuggestStockItemsCommand.Execute(_txtPattern.Text);
            }
        }
    }
}
