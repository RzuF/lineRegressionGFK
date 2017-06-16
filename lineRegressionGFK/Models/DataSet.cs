using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Helpers;

namespace lineRegressionGFK.Models
{
    public class DataSet : INotifyPropertyChanged
    {
        #region Brushes
        public string PointColorLabelText { get; } = "Pick point color";
        private Color _pointColor = Colors.White;
        public Color PointColor
        {
            get { return _pointColor; }
            set
            {
                _pointColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointColor)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointBrush)));
            }
        }

        public Brush PointBrush => new SolidColorBrush(_pointColor);

        public string RegressionLineColorLabelText { get; } = "Pick line color";
        private Color _regressionLineColor = Colors.White;
        public Color RegressionLineColor
        {
            get { return _regressionLineColor; }
            set
            {
                _regressionLineColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegressionLineColor)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegressionLineBrush)));
            }
        }

        public Brush RegressionLineBrush => new SolidColorBrush(_regressionLineColor);

        #endregion

        #region Point Radius
        public string PointRadiusLabelText { get; } = "Point size";
        private int _pointRadius = 10;
        public int PointRadius
        {
            get { return _pointRadius; }
            set
            {
                _pointRadius = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointRadius)));
            }
        }

        #endregion

        #region Point Types
        public string EllipseTypePointLabelText { get; } = "Ellipse";
        private bool _ellipseTypePoint = true;

        public bool EllipseTypePoint
        {
            get { return _ellipseTypePoint; }
            set
            {
                _ellipseTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EllipseTypePoint)));
            }
        }

        public string RectangleTypePointLabelText { get; } = "Rectangle";
        private bool _rectangleTypePoint = false;

        public bool RectangleTypePoint
        {
            get { return _rectangleTypePoint; }
            set
            {
                _rectangleTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RectangleTypePoint)));
            }
        }

        public string DiamondTypePointLabelText { get; } = "Diamond";
        private bool _diamondTypePoint = false;

        public bool DiamondTypePoint
        {
            get { return _diamondTypePoint; }
            set
            {
                _diamondTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DiamondTypePoint)));
            }
        }

        #endregion

        #region Points Collection

        private ObservableCollection<ChartPoint> _pointsCollection = new ObservableCollection<ChartPoint>();
        public ObservableCollection<ChartPoint> PointsCollection
        {
            get { return _pointsCollection; }
            set
            {
                _pointsCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointsCollection)));
            }
        }

        #endregion

        #region Regressions

        private LinearRegression _regressionLinear;

        public LinearRegression RegressionLinear
        {
            get { return _regressionLinear; }
            set
            {
                _regressionLinear = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegressionLinear)));
            }
        }

        private PolynomialRegression _regressionPolynomial;

        public PolynomialRegression RegressionPolynomial
        {
            get { return _regressionPolynomial; }
            set
            {
                _regressionPolynomial = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegressionPolynomial)));
            }
        }

        #endregion

        #region Regression Types

        public string LinearRegressionTypeLabelText { get; } = "Linear";
        private bool _linearRegressionType = true;
        public bool LinearRegressionType
        {
            get
            {
                return _linearRegressionType;
            }
            set
            {
                _linearRegressionType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinearRegressionType)));
            }
        }

        public string PolynomialRegressionTypeLabelText { get; } = "Polynomial";
        private bool _polynomialRegressionType = false;
        public bool PolynomialRegressionType
        {
            get
            {
                return _polynomialRegressionType;
            }
            set
            {
                _polynomialRegressionType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolynomialRegressionType)));
            }
        }

        #endregion

        #region Regression Properties
        public string PolynomialCoefficientLabelText { get; } = "Coefficient:";
        private int _polynomialCoefficient = 3;
        public int PolynomialCoefficient
        {
            get { return _polynomialCoefficient; }
            set
            {
                if (value != _polynomialCoefficient)
                {
                    _polynomialCoefficient = value;
                    UpdatePolynomial();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolynomialCoefficient)));
                }
            }
        }

        public string StepLabelText { get; } = "Step";
        private double _step = 1;

        public double Step
        {
            get { return _step; }
            set
            {
                if (_step != value)
                {
                    _step = value;
                    UpdatePolynomial();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolynomialCoefficient)));
                }
            }
        }

        #endregion

        #region MinMax Properties

        public double MaxXValue { get; private set; } = 100;
        public double MaxYValue { get; private set; } = 100;
        public double MinXValue { get; private set; } = -100;
        public double MinYValue { get; private set; } = -100;

        #endregion

        #region Update Methods
        public void AddPointToPointsCollection(double xValue, double yValue)
        {
            if (xValue > MaxXValue)
                MaxXValue = xValue;

            if (xValue < MinXValue)
                MinXValue = xValue;

            if (yValue > MaxYValue)
                MaxYValue = yValue;

            if (yValue < MinYValue)
                MinYValue = yValue;

            PointsCollection.Add(new ChartPoint()
            {
                X = xValue,
                Y = yValue
            });

            if (PointsCollection.Count > 1)
            {
                var regressionCoefficients = Regression.Polynomial(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray(), 1);
                var regressionStd = Regression.LinearStdDev(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray());

                RegressionLinear = new LinearRegression()
                {
                    GraphicRepresentation = PolynomialLineCreatorHelper.Create(regressionCoefficients, MinXValue, MaxXValue, Step),
                    AParameter = regressionCoefficients[1],
                    BParameter = regressionCoefficients[0],
                    StdA = regressionStd.Item1,
                    StdB = regressionStd.Item2
                };

                UpdatePolynomial();
            }
        }

        void UpdatePolynomial()
        {
            if (PointsCollection.Count < PolynomialCoefficient + 1)
                return;

            var polynomialRegressionCoefficients = Regression.Polynomial(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray(), PolynomialCoefficient);
            RegressionPolynomial = new PolynomialRegression()
            {
                Coefficients = polynomialRegressionCoefficients,
                GraphicRepresentation = PolynomialLineCreatorHelper.Create(polynomialRegressionCoefficients, MinXValue, MaxXValue, Step)
            };
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
