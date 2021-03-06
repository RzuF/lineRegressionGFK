﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace lineRegressionGFK.Converters
{
    /// <summary>
    /// IMultiValueConverter interface implementation. Choose currently proper string based on passed bools.
    /// </summary>
    public class InfoStringByBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool linear = (bool) values[3];
            bool orthogonal = (bool)values[4];
            if (linear)
                return values[0];
            if (orthogonal)
                return values[1];
            return values[2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
