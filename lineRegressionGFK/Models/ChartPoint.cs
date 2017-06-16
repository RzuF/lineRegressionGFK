using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using lineRegressionGFK.Annotations;

namespace lineRegressionGFK.Models
{
    /// <summary>
    /// Model of single chart point
    /// </summary>
    public class ChartPoint
    {
        /// <summary>
        /// Property hold information about X coordinate
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Property hold information about Y coordinate
        /// </summary>
        public double Y { get; set; }
    }
}
