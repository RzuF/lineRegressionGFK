namespace lineRegressionGFK.Models
{
    /// <summary>
    /// Model of single chart line
    /// </summary>
    public class ChartLine
    {
        /// <summary>
        /// Property holds information how far line should be placed
        /// </summary>
        public double PositionFromBeggining { get; set; }
        /// <summary>
        /// Property hold information how big should line be
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// Property hold information about offset
        /// </summary>
        public double OffsetX => Size / -2;
        /// <summary>
        /// Property hold information about string describing position
        /// </summary>
        public string StringValue { get; set; }
        /// <summary>
        /// Property hold information about opacity of line.
        /// </summary>
        public double Opacity { get; set; }
    }
}
