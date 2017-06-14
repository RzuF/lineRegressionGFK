using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lineRegressionGFK.Models
{
    public class ChartRegressionLine
    {
        public double AParameter { get; set; }

        public double BParameter { get; set; }
        public double AlteredAParameter { get; set; }

        public double Alpha => -Math.Abs(Math.Atan(AParameter) * 180.0 / Math.PI);

        public double XTransform { get; set; }
        public double YTransform { get; set; }

        public double Size { get; set; }

        public double OffsetX => Size / -2 + 1;

        public Color Color { get; set; } = Colors.White;
        public Brush Brush => new SolidColorBrush(Color);
    }
}
