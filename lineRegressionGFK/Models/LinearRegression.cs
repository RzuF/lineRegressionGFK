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
    public class LinearRegression : INotifyPropertyChanged
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

        public double AParameter { get; set; }
        public double BParameter { get; set; }
        public double StdA { get; set; }
        public double StdB { get; set; }
        public string InfoString => $"F(x) = {AParameter}x + {BParameter}\nStd(a) = {StdA}; Std(b)={StdB}";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
