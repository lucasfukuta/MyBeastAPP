using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyBeast.Converters
{
    public class BoolToHeartIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "heart_icon_filled.png" : "heart_icon.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
