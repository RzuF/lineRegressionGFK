using System;
using System.Globalization;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IValueConverter interface implemenation. Converts double to string and backwards.
    /// </summary>
    class Int32ToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return ((int?)value)?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int output;
            int.TryParse((string)value, out output);

            if (output < 0)
                output *= -1;

            return output;
        }
    }
}
