using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lineRegressionGFK.Models
{
    public class ChartPolynomialPart
    {
        public double XStart { get; set; }
        public double YStart { get; set; }
        public double XEnd { get; set; }
        public double YEnd { get; set; }

        public double YStartForChart => YStart - 5;
        public double Alpha => -Math.Atan2(YEnd - YStart,XEnd - XStart) * 180.0 / Math.PI;
        public double Size => Math.Sqrt(Math.Pow(XEnd - XStart, 2) + Math.Pow(YEnd - XEnd, 2));
        public Color Color { get; set; } = Colors.White;
        public Brush Brush => new SolidColorBrush(Color);
    }        
    
}
