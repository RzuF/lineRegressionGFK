using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Converts size parts to Size object.
    /// </summary>
    public class SizePartsToSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = value.Where(x => x is double).Cast<double>().ToList();
            return values.Count() != 2 ? DependencyProperty.UnsetValue : new Size(values[1], values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
