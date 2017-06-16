using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lineRegressionGFK.Models
{
    /// <summary>
    /// Model of single polynomial chart part. Whole polynomial chart is built from lines.
    /// </summary>
    public class ChartPolynomialPart
    {
        /// <summary>
        /// Property hold information about start X cordinate
        /// </summary>
        public double XStart { get; set; }
        /// <summary>
        /// Property hold information about start Y coordinate
        /// </summary>
        public double YStart { get; set; }
        /// <summary>
        /// Property hold information about end X coordinate
        /// </summary>
        public double XEnd { get; set; }
        /// <summary>
        /// Property hold information about end Y coordinate.
        /// </summary>
        public double YEnd { get; set; }

        /// <summary>
        /// Property hold information about angle required to transform line.
        /// </summary>
        public double Alpha => -Math.Atan2(YEnd - YStart,XEnd - XStart) * 180.0 / Math.PI;
    }        
    
}
