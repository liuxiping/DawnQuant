using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DawnQuant.App.Converter
{
    /// <summary>
    /// 将日期转换为 2020-9-25 的样式
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class LongDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;

            return dateTime.ToString("yyyy-MM-dd");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
