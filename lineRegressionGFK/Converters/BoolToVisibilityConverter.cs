using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IValueConverter interface implemenation. Converts bool to Visibility enum.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool?).Value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
