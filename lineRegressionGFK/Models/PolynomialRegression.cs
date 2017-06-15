using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using lineRegressionGFK.Annotations;

namespace lineRegressionGFK.Models
{    
    public class PolynomialRegression : INotifyPropertyChanged
    {
        private List<ChartPolynomialPart> _graphicRepresentation;

        public List<ChartPolynomialPart> GraphicRepresentation
        {
            get { return _graphicRepresentation; }
            set
            {
                _graphicRepresentation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GraphicRepresentation)));
            }
        }

        public double[] Coefficients { get; set; } = new double[0];
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
