using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Helpers;
using lineRegressionGFK.Models;
using MathNet.Numerics;
using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace lineRegressionGFK.VM
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public static MainPageViewModel Instance { get; private set; }
        public static bool ManipulationStarted { get; set; } = false;
        private Size _renderSize;

        public Size RenderSize
        {
            get { return _renderSize; }
            set
            {
                _renderSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RenderSize)));
            }
        }

        #region Private Properties

        public MainWindow MainWindow { get; set; }
        public double? MaxXValue { get; private set; } = 100;
        public double? MaxYValue { get; private set; } = 100;
        public double? MinXValue { get; private set; } = -100;
        public double? MinYValue { get; private set; } = -100;

        #endregion

        #region Private Helper Methods

        #if DEBUG
        public void AddPointToPointsCollection(double xValue, double yValue)
#else
        private void AddPointToPointsCollection(double xValue, double yValue)
#endif
        {
            if (MaxXValue == null || xValue > MaxXValue)
                MaxXValue = xValue;

            if (MinXValue == null || xValue < MinXValue)
                MinXValue = xValue;

            if (MaxYValue == null || yValue > MaxYValue)
                MaxYValue = yValue;

            if (MinYValue == null || yValue < MinYValue)
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
                    GraphicRepresentation = PolynomialLineCreatorHelper.Create(regressionCoefficients, MinXValue.Value, MaxXValue.Value, Step),
                    AParameter = regressionCoefficients[1],
                    BParameter = regressionCoefficients[0],
                    StdA = regressionStd.Item1,
                    StdB = regressionStd.Item2
                };

                UpdatePolynomial();
            }

            UpdateAllChartElements();
        }

        void UpdatePolynomial()
        {            
            if(PointsCollection.Count < PolynomialCoefficient + 1)
                return;
            var polynomialRegressionCoefficients = Regression.Polynomial(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray(), PolynomialCoefficient);
            //var polynomialRegressionCoefficients = Fit.Polynomial(PointsCollection.Select(x => x.X).ToArray(), PointsCollection.Select(x => x.Y).ToArray(), PolynomialCoefficient);            
            //polynomialRegressionCoefficients = new double[] {0, 1};
            RegressionPolynomial = new PolynomialRegression()
            {
                Coefficients = polynomialRegressionCoefficients,
                GraphicRepresentation = PolynomialLineCreatorHelper.Create(polynomialRegressionCoefficients, MinXValue.Value, MaxXValue.Value, Step)
            };
        }

        void UpdateChartPoints()
        {
            List<ChartPoint> _newList = new List<ChartPoint>();

            foreach (var chartPoint in PointsCollection)
            {                
                chartPoint.XForChart = chartPoint.X - (double)PointRadius / Scale / 2.0;
                chartPoint.YForChart = -chartPoint.Y + 15 / Scale - (double)PointRadius / Scale / 2.0;
                _newList.Add(chartPoint);
            }

            if(PointsCollection.Count <= _newList.Count)
                PointsCollection = _newList;
        }

        void UpdateHorizontalLines()
        {
            List<ChartLine> _newList = new List<ChartLine>();
            int i = 0;
            for (i = 0; ;i++)
            {
                if (_renderSize.Height == 0)
                    break;
                var max = MaxYValue.Value > -MinYValue.Value ? MaxYValue.Value : -MinYValue.Value;
                double position = i * LineHighDelta;
                if (i * LineHighDelta > max && position * Scale > _renderSize.Height)
                    break;
                _newList.Add(new ChartLine() {PositionFromBeggining = position, Size = _renderSize.Width * 2 > max * 2 * Scale ? _renderSize.Width * 2 : max * 2 * Scale, Opacity = LineHorizontalOpacity, StringValue = $"{-i * LineHighDelta}"});
                if (i != 0)
                    _newList.Add(new ChartLine() { PositionFromBeggining = -position, Size = _renderSize.Width * 2 > max * 2 * Scale ? _renderSize.Width * 2 : max * 2 * Scale, Opacity = LineHorizontalOpacity, StringValue = $"{i * LineHighDelta}" });
            }
            HorizontalLinesCollection = _newList;
        }

        void UpdateVerticalLines()
        {
            List<ChartLine> _newList = new List<ChartLine>();
            for (int i = 0; ; i++)
            {
                if (_renderSize.Width == 0)
                    break;
                var max = MaxXValue.Value > -MinXValue.Value ? MaxXValue.Value : -MinXValue.Value;
                double position = i * LineWidthDelta;
                if (i * LineWidthDelta > max && position * Scale > _renderSize.Width)
                    break;
                _newList.Add(new ChartLine() { PositionFromBeggining = position, Size = _renderSize.Height*2 > max * 2 * Scale ? _renderSize.Height * 2 : max * 2 * Scale, Opacity = LineVerticalOpacity, StringValue = $"{i * LineWidthDelta}" });
                if(i!=0)
                    _newList.Add(new ChartLine() { PositionFromBeggining = -position, Size = _renderSize.Height * 2 > max * 2 * Scale ? _renderSize.Height * 2 : max * 2 * Scale, Opacity = LineVerticalOpacity, StringValue = $"{-i * LineWidthDelta}" });
            }
            VerticalLinesCollection = _newList;
        }

        void UpdateRegressionLine()
        {
            RegressionLine = new ChartRegressionLine()
            {
                AParameter = RegressionLine.AParameter,
                BParameter = RegressionLine.BParameter,
                Size = _renderSize.Width*2,
                YTransform = RegressionLine.BParameter * Scale +15
            };
        }

        void UpdateAllChartElements()
        {
            UpdateChartPoints();
            UpdateVerticalLines();
            UpdateHorizontalLines();
        }

        #endregion

        #region Chart Properties

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

        public string LineColorLabelText { get; } = "Pick line color";
        private Color _lineColor = Colors.White;
        public Color LineColor
        {
            get { return _lineColor; }
            set
            {
                _lineColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineColor)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineBrush)));
            }
        }

        public Brush LineBrush => new SolidColorBrush(_lineColor);

        public string BackgroundColorLabelText { get; } = "Pick background color";
        private Color _backgroundColor = Colors.Black;
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundBrush)));
            }
        }

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

        public Brush BackgroundBrush => new SolidColorBrush(_backgroundColor);

        public string PointRadiusLabelText { get; } = "Point size";
        private int _pointRadius = 10;
        public int PointRadius
        {
            get { return _pointRadius; }
            set
            {
                _pointRadius = value;
                UpdateChartPoints();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointRadius)));
            }
        }

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

        private List<ChartPoint> _pointsCollection = new List<ChartPoint>();
        public List<ChartPoint> PointsCollection
        {
            get { return _pointsCollection; }
            set
            {
                _pointsCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PointsCollection)));
            }
        }

        private List<ChartLine> _horizontalLinesCollection = new List<ChartLine>();
        public List<ChartLine> HorizontalLinesCollection
        {
            get { return _horizontalLinesCollection; }
            set
            {
                _horizontalLinesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HorizontalLinesCollection)));
            }
        }

        private List<ChartLine> _verticalLinesCollection = new List<ChartLine>();
        public List<ChartLine> VerticalLinesCollection
        {
            get { return _verticalLinesCollection; }
            set
            {
                _verticalLinesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerticalLinesCollection)));
            }
        }

        public string YDeltaLabelText { get; set; } = "High line delta: ";
        public string XDeltaLabelText { get; set; } = "Width line delta: ";

        private double _lineHighDelta = 10;
        public double LineHighDelta
        {
            get { return _lineHighDelta; }
            set
            {
                _lineHighDelta = value;
                UpdateHorizontalLines();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineHighDelta)));
            }
        }
        private double _lineWidthDelta = 10;    
        public double LineWidthDelta
        {
            get { return _lineWidthDelta; }
            set
            {
                _lineWidthDelta = value;
                UpdateVerticalLines();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineWidthDelta)));
            }
        }

        private double _lineVerticalOpacity = 0.5;

        public double LineVerticalOpacity
        {
            get { return _lineVerticalOpacity; }
            set
            {
                _lineVerticalOpacity = value;
                UpdateVerticalLines();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineVerticalOpacity)));
            }
        }

        private double _lineHorizontalOpacity = 0.5;

        public double LineHorizontalOpacity
        {
            get { return _lineHorizontalOpacity; }
            set
            {
                _lineHorizontalOpacity = value;
                UpdateHorizontalLines();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineHorizontalOpacity)));
            }
        }

        public string ScaleLabelText { get; } = "Scale";
        private double _scale = 3;

        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                //UpdateChartPoints();
                UpdateAllChartElements();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Scale)));
            }
        }        

        private ChartRegressionLine _regressionLine = new ChartRegressionLine();

        public ChartRegressionLine RegressionLine
        {
            get { return _regressionLine; }
            set
            {
                _regressionLine = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RegressionLine)));
            }            
        
        }

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

        #region Menu Properties

        public string XLabelText { get; } = "X: ";
                public string YLabelText { get; } = "Y: ";

                private double _currentXValue;

                public double CurrentXValue
                {
                    get { return _currentXValue; }
                    set
                    {
                        _currentXValue = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentXValue)));
                    }
                }
                private double _currentYValue;

                public double CurrentYValue
                {
                    get { return _currentYValue; }
                    set
                    {
                        _currentYValue = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentYValue)));
                    }
                }

        #region Button Properties

                    public string AddButtonText => "Add";
                    private ICommand _addButtonCommand;

                    public ICommand AddButtonCommand => _addButtonCommand ?? new RelayCommand((object obj) =>
                    {
                        AddPointToPointsCollection(CurrentXValue, CurrentYValue);

                        CurrentXValue = CurrentYValue = 0;                        
                    });

                    public string FromFileText => "From file...";
                    private ICommand _fromFileCommand;

                    public ICommand FromFileCommand => _fromFileCommand ?? new RelayCommand((obj) =>
                    {
                        var saveFileDialog = new OpenFileDialog() { RestoreDirectory = true };

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (var saveStream = new StreamReader(saveFileDialog.OpenFile()))
                            {
                                PointsCollection.Clear();                                
                                string readFile = saveStream.ReadToEnd();
                                var fileLines = readFile.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var fileLine in fileLines)
                                {
                                    var xy = fileLine.Split(new[] {" ", ";", "\t"},
                                        StringSplitOptions.RemoveEmptyEntries);
                                    double x, y;
                                    if (xy.Length == 2 && double.TryParse(xy[0], out x) && double.TryParse(xy[1], out y))
                                    {
                                        AddPointToPointsCollection(x,y);
                                    }
                                }
                            }
                        }
                    });

                    public string SaveAsText => "Save chart as...";
                    private ICommand _saveAsCommand;            

                    public ICommand SaveAsCommand => _saveAsCommand ?? new RelayCommand((obj) =>
                        {                
                            var saveFileDialog = new System.Windows.Forms.SaveFileDialog() {RestoreDirectory = true};

                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {                    
                                var renderTargetBitmap = new RenderTargetBitmap((int)MainWindow.ChartGrid.RenderSize.Width, (int)MainWindow.ChartGrid.RenderSize.Height, 96d, 85d, PixelFormats.Pbgra32);
                                renderTargetBitmap.Render(MainWindow.ChartGrid);
                                using (var saveStream = saveFileDialog.OpenFile())
                                {
                                    var encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                                    encoder.Save(saveStream);
                                }
                            }
                        });

                    public string ClearText => "Clear chart of points";
                    private ICommand _clearCommand;

                    public ICommand ClearCommand => _clearCommand ?? new RelayCommand((obj) =>
                    {
                        PointsCollection = new List<ChartPoint>();
                        RegressionLinear = new LinearRegression();
                        RegressionPolynomial = new PolynomialRegression();
                    });

                    private ICommand _sizeChangedCommand;

                    public ICommand SizeChangedCommand => _sizeChangedCommand ?? new RelayCommand((obj) =>
                    {
                        RenderSize = (Size) obj;
                    });

                    private ICommand _radioChangedCommand;

                    public ICommand RadioChangedCommand => _radioChangedCommand ?? new RelayCommand((obj) =>
                    {
            
                    });

                    public string PrintText  => "Print Chart";
                    private ICommand _printCommand;

                    public ICommand PrintCommand => _printCommand ?? new RelayCommand((obj) =>
                    {
                        var printDialog = new System.Windows.Controls.PrintDialog();
                        if (printDialog.ShowDialog() == true)
                        {
                            printDialog.PrintVisual(MainWindow.ChartGrid, "Chart");
                        }
                    });


        public string ToCenterText { get; } = "Center chart";
                    private ICommand _toCenterCommand;
                    public ICommand ToCenterCommand => _toCenterCommand ?? new RelayCommand((obj) =>
                    {
                        TransformGroup transformGroup = new TransformGroup();
                        TranslateTransform translateTransform = new TranslateTransform(_renderSize.Width/2, _renderSize.Height/2);

                        transformGroup.Children.Add(translateTransform);

                        (obj as Grid).RenderTransform = transformGroup;

                        Scale = Scale;
                    });

        #endregion

        public MainPageViewModel()
                {
                    if (Instance == null)
                        Instance = this;
                }

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
