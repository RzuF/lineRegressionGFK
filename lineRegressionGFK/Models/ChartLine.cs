using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lineRegressionGFK.Models
{
    public class ChartLine
    {
        public double PositionFromBeggining { get; set; }

        public double Size { get; set; }
        public Brush LineColor { get; set; }

        public double Opacity { get; set; }
    }
}
