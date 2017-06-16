using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lineRegressionGFK.Annotations;

namespace lineRegressionGFK.Models
{    
    public class PolynomialRegression : INotifyPropertyChanged
    {
        private List<ChartPolynomialPart> _graphicRepresentation;
        /// <summary>
        /// Property holds list of ChartPolynomialParts which are graphical representation of regression
        /// </summary>
        public List<ChartPolynomialPart> GraphicRepresentation
        {
            get { return _graphicRepresentation; }
            set
            {
                _graphicRepresentation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GraphicRepresentation)));
            }
        }

        /// <summary>
        /// Property holds array of coefficient of current polynomial
        /// </summary>
        public double[] Coefficients { get; set; } = new double[0];
        /// <summary>
        /// Property holds information about text to display in frame
        /// </summary>
        public string InfoString
        {
            get
            {
                List<string> expressonStrings = new List<string>();
                for(int i = 0; i < Coefficients.Length; i++)
                {
                    expressonStrings.Add($"{Coefficients[i]}x^{i}");
                }
                return $"F(x) = {string.Join(" + ", expressonStrings)}";
            }
        }

        /// <summary>
        /// Impementation of INotifyPropertyChanged interface, neccessary for proper Binding behaviour.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Impementation of INotifyPropertyChanged interface, neccessary for proper Binding behaviour.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
