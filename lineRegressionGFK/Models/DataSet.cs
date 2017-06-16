using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Helpers;

namespace lineRegressionGFK.Models
{
    /// <summary>
    /// Model of single set of data. Holds information of points, regressions and style.
    /// </summary>
    public class DataSet : INotifyPropertyChanged
    {
        #region Brushes

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string PointColorLabelText { get; } = "Pick point color";
        private Color _pointColor = Colors.White;
        /// <summary>
        /// Property holds information about chart points color
        /// </summary>
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
        /// <summary>
        /// Property holds information about chart points brush with color specified from PointColor property
        /// </summary>
        public Brush PointBrush => new SolidColorBrush(_pointColor);

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string RegressionLineColorLabelText { get; } = "Pick line color";
        private Color _regressionLineColor = Colors.White;
        /// <summary>
        /// Property holds information about regression line color
        /// </summary>
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
        /// <summary>
        /// Property holds information about regression line brush with color specified from RegressionLineColor property
        /// </summary>
        public Brush RegressionLineBrush => new SolidColorBrush(_regressionLineColor);

        #endregion

        #region Point Radius

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string PointRadiusLabelText { get; } = "Point size";
        private int _pointRadius = 10;
        /// <summary>
        /// Property holds information about radius of each point
        /// </summary>
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

        private bool _isChangePending = false;

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string EllipseTypePointLabelText { get; } = "Ellipse";
        private bool _ellipseTypePoint = true;
        /// <summary>
        /// Property holds information if ellipse shape is selected. Default true
        /// </summary>
        public bool EllipseTypePoint
        {
            get { return _ellipseTypePoint; }
            set
            {
                if (_isChangePending)
                {
                    _isChangePending = false;
                    return;
                }
                if (value == false && _ellipseTypePoint == false && _rectangleTypePoint == false)
                {
                    _isChangePending = true;
                    return;
                }
                _ellipseTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EllipseTypePoint)));
            }
        }

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string RectangleTypePointLabelText { get; } = "Rectangle";
        private bool _rectangleTypePoint = false;
        /// <summary>
        /// Property holds information if rectangle shape is selected. Default false
        /// </summary>
        public bool RectangleTypePoint
        {
            get { return _rectangleTypePoint; }
            set
            {
                if (_isChangePending)
                {
                    _isChangePending = false;
                    return;
                }
                if (value == false && _ellipseTypePoint == false && _diamondTypePoint == false)
                {
                    _isChangePending = true;
                    return;
                }
                _rectangleTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RectangleTypePoint)));
            }
        }

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string DiamondTypePointLabelText { get; } = "Diamond";
        private bool _diamondTypePoint = false;
        /// <summary>
        /// Property holds information if diamond shape is selected. Default false
        /// </summary>
        public bool DiamondTypePoint
        {
            get { return _diamondTypePoint; }
            set
            {
                if(_isChangePending)
                {
                    _isChangePending = false;
                    return;
                }
                if (value == false && _ellipseTypePoint == false && _rectangleTypePoint == false)
                {
                    _isChangePending = true;
                    return;
                }
                _diamondTypePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DiamondTypePoint)));
            }
        }

        #endregion

        #region Points Collection

        private ObservableCollection<ChartPoint> _pointsCollection = new ObservableCollection<ChartPoint>();
        /// <summary>
        /// Property holds list of points in single DataSet object. ObservableCollection type notify View if any change in collection.
        /// </summary>
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
        /// <summary>
        /// Property holds information about linear regression of current set of points
        /// </summary>
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
        /// <summary>
        /// Property holds information about polynomial regression of current set of points and specified coefficient
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string LinearRegressionTypeLabelText { get; } = "Linear";
        private bool _linearRegressionType = true;
        /// <summary>
        /// Property holds information if linear regression is currently selected. Default true.
        /// </summary>
        public bool LinearRegressionType
        {
            get
            {
                return _linearRegressionType;
            }
            set
            {
                if (_isChangePending)
                {
                    _isChangePending = false;
                    return;
                }
                if (value == false && _polynomialRegressionType == false)
                {
                    _isChangePending = true;
                    return;
                }
                _linearRegressionType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LinearRegressionType)));
            }
        }

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string PolynomialRegressionTypeLabelText { get; } = "Polynomial";
        private bool _polynomialRegressionType = false;
        /// <summary>
        /// Property holds information if polynomial regression is currently selected. Default false
        /// </summary>
        public bool PolynomialRegressionType
        {
            get
            {
                return _polynomialRegressionType;
            }
            set
            {
                if (_isChangePending)
                {
                    _isChangePending = false;
                    return;
                }
                if (value == false && _linearRegressionType == false)
                {
                    _isChangePending = true;
                    return;
                }
                _polynomialRegressionType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolynomialRegressionType)));
            }
        }

        #endregion

        #region Regression Properties

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string PolynomialCoefficientLabelText { get; } = "Coefficient:";
        private int _polynomialCoefficient = 3;
        /// <summary>
        /// Property holds information about currently specified coefficient. Updated fire UpdatePolynomial() method. Default 3
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string StepLabelText { get; } = "Step";
        /// <summary>
        /// Property holds information about currently specified step for graphical representation. Updated fire UpdatePolynomial() method. Default 1
        /// </summary>
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

        /// <summary>
        /// Property holds information about current maximum value of Xs collection in this DataSet, initially set to 100
        /// </summary>
        public double MaxXValue { get; private set; } = 100;
        /// <summary>
        /// Property holds information about current maximum value of Ys collection in this DataSet, initially set to 100
        /// </summary>
        public double MaxYValue { get; private set; } = 100;
        /// <summary>
        /// Property holds information about current minimum value of Xs collection in this DataSet, initially set to -100
        /// </summary>
        public double MinXValue { get; private set; } = -100;
        /// <summary>
        /// Property holds information about current minimum value of Ys collection in this DataSet, initially set to -100
        /// </summary>
        public double MinYValue { get; private set; } = -100;

        #endregion

        #region Update Methods

        /// <summary>
        /// Method for adding point to this DataSet. Update linear and polynomial regression data.
        /// </summary>
        /// <param name="xValue">X coordinate of Point</param>
        /// <param name="yValue">Y coordinate of Point</param>
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

        /// <summary>
        /// Helper method for updating data of polynomial regression. Take no effect if there is unsuffcient number of points (orderOfPolynomial+1).
        /// </summary>
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

        #region Indetification Properties

        /// <summary>
        /// Property holds information about id of this DataSet
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string Name => $"DataSet {Id}";

        #endregion

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
