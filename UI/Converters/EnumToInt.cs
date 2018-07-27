using System;
using System.Globalization;
using System.Windows.Data;

namespace cAlgo.API.Alert.UI.Converters
{
    [ValueConversion(typeof(object), typeof(int))]
    public class EnumToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? (int)value : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}