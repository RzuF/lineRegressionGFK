using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using lineRegressionGFK.VM;

namespace lineRegressionGFK.Converters
{
    public class PointPositionScaleConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double position = (double) value;
            var pos = (int)(position / (double)MainPageViewModel.Instance.MaxYValue * (MainPageViewModel.Instance.MainWindow.ChartCanvas.RenderSize.Height-20)) + 10;
            return pos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
