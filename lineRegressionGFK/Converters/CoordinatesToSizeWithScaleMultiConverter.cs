using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
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
