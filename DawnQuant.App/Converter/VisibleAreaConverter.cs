using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static DawnQuant.App.ViewModels.AShare.Common.StockChartViewModel;

namespace DawnQuant.App.Converter
{

    public class VisibleAreaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string source = parameter?.ToString();
            VisibleArea v = (VisibleArea)value;

            if (source == "F10")
            {
                if (v == VisibleArea.F10)
                {
                    return Visibility.Visible; ;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            else if (source == "Chart")
            {
                if (v == VisibleArea.Chart)
                {
                    return  Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed; ;
                }
            }
            else
            {
                return Visibility.Visible; ;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
