using System;
using System.Windows.Data;
using System.Windows.Media;

namespace DawnQuant.App.Converter
{
    public class VolumeFillColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var obj = value as StockPlotDataItem;
            //return obj != null && obj.Open < obj.Close ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green
            //   );
            var obj = (bool)value;
            return obj ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
