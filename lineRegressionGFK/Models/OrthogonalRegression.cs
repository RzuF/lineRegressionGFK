using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Models;

namespace lineRegressionGFK.Models
{ 
    /// <summary>
    /// Model of Linear regression
    /// </summary>
    public class OrthogonalRegression : INotifyPropertyChanged
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
        /// Property holds information about A paremeter of orthogonal regression
        /// </summary>
        public double AParameter { get; set; }
        /// <summary>
        /// Property holds information about B parameter of orthogonal regression
        /// </summary>
        public double BParameter { get; set; }
        public string InfoString => $"F(x) = {AParameter}x + ({BParameter})";

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
