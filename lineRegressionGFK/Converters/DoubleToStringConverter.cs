using System;
using System.Globalization;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IValueConverter interface implemenation. Converts double to string and backwards.
    /// </summary>
    public class DoubleToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return ((double?)value)?.ToString() ?? "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double output;
            string alteredInput = ((string) value).Replace(",", ".");
            if (double.TryParse(alteredInput, NumberStyles.Any, new CultureInfo("en-US"), out output))
                return output;
            else
                return 1;
        }
    }
}
