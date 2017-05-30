using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Models;
using MathNet.Numerics;

namespace lineRegressionGFK.VM
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        #region Chart Properties

        public ObservableCollection<ChartPoint> PointsCollection { get; } = new ObservableCollection<ChartPoint>();

        #endregion

        #region Menu Properties

        public string XLabelText { get; } = "X: ";
        public string YLabelText { get; } = "Y: ";

        private int _currentXValue;

        public int CurrentXValue
        {
            get { return _currentXValue; }
            set
            {
                _currentXValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentXValue)));
            }
        }
        private int _currentYValue;

        public int CurrentYValue
        {
            get { return _currentYValue; }
            set
            {
                _currentYValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentYValue)));
            }
        }

        public string  linear1 { get; set; }

        #region Button Properties

            public string AddButtonText => "Add";
            private ICommand _addButtonCommand;

            public ICommand AddButtonCommand => _addButtonCommand ?? new RelayCommand((object obj) =>
            {
                PointsCollection.Add(new ChartPoint()
                {
                    X = CurrentXValue,
                    Y = CurrentYValue
                });

                CurrentXValue = CurrentYValue = 0;

                if(PointsCollection.Count > 1)
                {
                    var line = Fit.Line(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray());
                }
            });

            public string FromFileText => "From file...";
            private ICommand _fromFileCommand;

            public ICommand FromFileCommand => _fromFileCommand ?? new RelayCommand((obj) =>
            {
                // TODO: Import points from file
            });

        #endregion



        #endregion

        #region Public Methods

        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region INotifyPropertyChanged Interface Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
