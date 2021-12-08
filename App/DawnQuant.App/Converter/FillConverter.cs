
using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DawnQuant.App.Converter
{
    public class FillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null && value != null)
            {
                StockTradeData data = value as StockTradeData;
                if (data.Close > data.Open)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.Green);
            }
           
            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
