using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Converts passed coordinate and scale to size parameter.
    /// </summary>
    public class CoordinatesToSizeWithScaleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValues = values.Where(x => x is double).Cast<double>().ToList();

            if (doubleValues.Count == 5)
            {
                return
                    Math.Sqrt(Math.Pow(doubleValues[4], 2) * Math.Pow(doubleValues[1] - doubleValues[0], 2) +
                              Math.Pow(doubleValues[4], 2) * Math.Pow(doubleValues[3] - doubleValues[2], 2));
            }
            else
                return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
