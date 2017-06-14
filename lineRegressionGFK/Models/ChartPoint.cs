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
    public class ChartPoint : INotifyPropertyChanged
    {
        private double _x;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double XForChart { get; set; }
        // TODO: Make cooridnated dependent of thr rest of the points

        private double _y;
        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double YForChart { get; set; }
        // TODO: Make cooridnated dependent of thr rest of the points
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
