using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    public class SizePartsToSizeConverter : IMultiValueConverter
    {
        //public static SizePartsToSizeConverter Instance { get; } = new SizePartsToSizeConverter();

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
