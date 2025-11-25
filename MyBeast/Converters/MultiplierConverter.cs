using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyBeast.Converters
{
    public class MultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentValue && parameter is int targetValue && targetValue != 0)
            {
                return (double)currentValue / targetValue;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
