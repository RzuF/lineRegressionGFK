using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    public class StringToDoubleConverter: IValueConverter
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
