using System;
using System.Globalization;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Converts passed X coordinate, scale and pointRadius to proper Y postion on chart
    /// </summary>
    public class PointYScalePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                var y = (double)values[0];
                var scale = (double)values[1];
                var pointRadius = (int)values[2];
                return -y * scale - pointRadius / 2.0 + 15;
            }
            else
                return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
