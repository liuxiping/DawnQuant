using DawnQuant.App.ViewModels.AShare.SelfSelStock;
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

namespace DawnQuant.App.Views.AShare.SelfSelStock
{
    /// <summary>
    /// SelfSelStockContentView.xaml 的交互逻辑
    /// </summary>
    public partial class SelfSelStockContentView : UserControl
    {
        public SelfSelStockContentView()
        {
            InitializeComponent();
           
        }

        private void btnGroup_Checked(object sender, RoutedEventArgs e)
        {
            //dgStockItem.GroupColumnDescriptions.Add(
            // new Syncfusion.UI.Xaml.Grid.GroupColumnDescription { ColumnName = "Industry" });
            //dgStockItem.ExpandAllGroup();
            //btnGroup.Content = "取消分组";
        }

        private void btnGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            //dgStockItem.GroupColumnDescriptions.Clear();
            //btnGroup.Content = "按行业分组";

        }

        private void MvxWpfView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private SelfSelStockContentViewModel Model { get { return (SelfSelStockContentViewModel)DataContext; } }

        //private void ChartTrackBallBehavior_PositionChanged(object sender, Syncfusion.UI.Xaml.Charts.PositionChangedEventArgs e)
        //{
        //    if (e.CurrentPointInfos.Count <= 0)
        //    {
        //        return;
        //    }

        //    CurSelStockItemInfo infos = new CurSelStockItemInfo();

        //    StockTradeData tradeData = e.CurrentPointInfos.Where(p => p.Series.Name == "cscandle").Select(p => p.Item).SingleOrDefault()
        //         as StockTradeData;

        //    if (tradeData != null)
        //    {
        //        infos.TradeDateTime = tradeData.TradeDateTime;
        //        infos.PreClose = tradeData.PreClose;
        //        infos.Open = tradeData.Open;
        //        infos.Close = tradeData.Close;
        //        infos.High = tradeData.High;
        //        infos.Low = tradeData.Low;

        //        infos.Amount = tradeData.Amount;
        //        infos.Volume = tradeData.Volume;
        //    }

        //    SMAPlotData ma5 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA5").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma5 != null)
        //    {
        //        infos.MA5 = ma5.Price;
        //    }

        //    SMAPlotData ma10 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA10").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma10 != null)
        //    {
        //        infos.MA10 = ma10.Price;
        //    }

        //    SMAPlotData ma20 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA20").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma20 != null)
        //    {
        //        infos.MA20 = ma20.Price;
        //    }


        //    SMAPlotData ma30 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA30").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma30 != null)
        //    {
        //        infos.MA30 = ma30.Price;
        //    }

        //    SMAPlotData ma60 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA60").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma60 != null)
        //    {
        //        infos.MA60 = ma60.Price;
        //    }

        //    SMAPlotData ma120 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA120").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma120 != null)
        //    {
        //        infos.MA120 = ma120.Price;
        //    }

        //    SMAPlotData ma250 = e.CurrentPointInfos.Where(p => p.Series.Name == "ssMA250").Select(p => p.Item).SingleOrDefault()
        //        as SMAPlotData;
        //    if (ma250 != null)
        //    {
        //        infos.MA250 = ma250.Price;
        //    }

        //    激活命令
        //    Model.TrackPositionChangedCommand.Execute(infos);
        //}
    }
}
