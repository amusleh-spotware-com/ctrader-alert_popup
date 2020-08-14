using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace cAlgo.API.Alert.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class EnumToDescription : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                FieldInfo field = value.GetType().GetField(value.ToString());

                if (field != null)
                {
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attributes.Length > 0 && !String.IsNullOrEmpty(attributes[0].Description))
                    {
                        return attributes[0].Description;
                    }
                }

                return value.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}