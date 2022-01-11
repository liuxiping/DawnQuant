using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DawnQuant.App.Converter
{
    [ValueConversion(typeof(PlotData),typeof(SolidColorBrush))]
    public class StockPlotColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string p = parameter.ToString();

            var data = value as PlotData;

            if (p == "Open")
            {
                return data != null && data.Open > data.PreClose ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else if (p == "Close")
            {
                return data != null && data.Close > data.PreClose ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else if (p == "High")
            {
                return data != null && data.High > data.PreClose ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else if (p == "Low")
            {
                return data != null && data.Low > data.PreClose ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else if (p == "Gain")
            {
                return data != null && data.Gain >= 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else if (p == "Volume")
            {
                return data != null && data.Close >= data.Open ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
