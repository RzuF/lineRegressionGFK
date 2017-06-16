using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Converts passed X and Y coordinate to string.
    /// </summary>
    class PositionToStringMultiConverter : IMultiValueConverter
    {        
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = value.Where(x => x is double).Cast<double>().ToList();
            return values.Count() != 2 ? DependencyProperty.UnsetValue : $"F({values[0]}) = {values[1]}";
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
