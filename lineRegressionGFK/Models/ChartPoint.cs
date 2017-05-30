using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lineRegressionGFK.Models
{
    public class ChartPoint
    {
        private double _x;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double XForChart => _x + 10;
        // TODO: Make cooridnated dependent of thr rest of the points

        private double _y;
        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double YForChart => _y + 10;
        // TODO: Make cooridnated dependent of thr rest of the points
    }
}
