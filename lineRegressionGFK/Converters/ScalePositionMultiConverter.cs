using System;
using System.Globalization;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Converts passed position coordinate and scale to proper position on chart
    /// </summary>
    public class ScalePositionMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
                return (double) values[0] * (double) values[1];
            else
                return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
