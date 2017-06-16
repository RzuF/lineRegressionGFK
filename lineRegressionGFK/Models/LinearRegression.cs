using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using lineRegressionGFK.Annotations;

namespace lineRegressionGFK.Models
{
    /// <summary>
    /// Model of Linear regression
    /// </summary>
    public class LinearRegression : INotifyPropertyChanged
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
        /// Property holds information about A paremeter of linear regression
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// Property holds information about B parameter of linear regression
        /// </summary>
        public double BParameter { get; set; }
        /// <summary>
        /// Property holds information about standard deviation of A Parameter of linear regression
        /// </summary>
        public double StdA { get; set; }
        /// <summary>
        /// Property holds information about standard deviation of B Parameter of linear regression
        /// </summary>
        public double StdB { get; set; }
        /// <summary>
        /// Property holds information about text to display in frame
        /// </summary>
        public string InfoString => $"F(x) = {AParameter}x + ({BParameter})\nStd(a) = {StdA}; Std(b)={StdB}";

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
