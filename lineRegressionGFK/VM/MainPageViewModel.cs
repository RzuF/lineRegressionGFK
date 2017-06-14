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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Models;
using MathNet.Numerics;

namespace lineRegressionGFK.VM
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public static MainPageViewModel Instance { get; private set; }
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
        public int? MaxXValue { get; private set; } = 100;
        public int? MaxYValue { get; private set; } = 100;
        public int? MinXValue { get; private set; } = 0;
        public int? MinYValue { get; private set; } = 0;

        #endregion

        #region Private Helper Methods

        #if DEBUG
        public void AddPointToPointsCollection(int xValue, int yValue)
        #else
        private void AddPointToPointsCollection(int xValue, int yValue)
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
            UpdateChartPoints();
        }

        void UpdateChartPoints()
        {
            List<ChartPoint> _newList = new List<ChartPoint>();

            foreach (var chartPoint in PointsCollection)
            {
                chartPoint.XForChart = (int)(chartPoint.X / MaxXValue * (_renderSize.Width - 20)) + 10;
                chartPoint.YForChart = (int)(chartPoint.Y / MaxYValue * (_renderSize.Height - 20)) + 10;
                _newList.Add(chartPoint);
            }

            PointsCollection = _newList;
        }

        void UpdateHorizontalLines()
        {
            List<ChartLine> _newList = new List<ChartLine>();
            double currentHight = 0;
            for (int i = 0; ;i++)
            {
                double position = i * LineHighDelta / MaxYValue.Value * (_renderSize.Height - 20) + 10;
                if (position > _renderSize.Height)
                    break;
                _newList.Add(new ChartLine() {PositionFromBeggining = position, Size = _renderSize.Width, Opacity = LineHorizontalOpacity});
            }
            HorizontalLinesCollection = _newList;
        }

        void UpdateVerticalLines()
        {
            List<ChartLine> _newList = new List<ChartLine>();
            double currentHight = 0;
            for (int i = 0; ; i++)
            {
                double position = i * LineWidthDelta / MaxXValue.Value * (_renderSize.Width - 20) + 10;
                if (position > _renderSize.Width)
                    break;
                _newList.Add(new ChartLine() { PositionFromBeggining = position, Size = _renderSize.Height, Opacity = LineVerticalOpacity});
            }
            VerticalLinesCollection = _newList;
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

        public Brush BackgroundBrush => new SolidColorBrush(_backgroundColor);

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

        public double ChartGridHeight { get; set; }
        public double ChartGridWidth { get; set; }

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
                AddPointToPointsCollection(CurrentXValue, CurrentYValue);

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

            public string SaveAsText => "From file...";
            private ICommand _saveAsCommand;            

            public ICommand SaveAsCommand => _saveAsCommand ?? new RelayCommand((obj) =>
                {                
                    var saveFileDialog = new SaveFileDialog() {RestoreDirectory = true};

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {                    
                        var renderTargetBitmap = new RenderTargetBitmap((int)MainWindow.ChartCanvas.RenderSize.Width, (int)MainWindow.ChartCanvas.RenderSize.Height, 96d, 85d, PixelFormats.Pbgra32);
                        renderTargetBitmap.Render(MainWindow.ChartCanvas);
                        using (var saveStream = saveFileDialog.OpenFile())
                        {
                            var encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                            encoder.Save(saveStream);
                        }
                    }
                });

            private ICommand _sizeChangedCommand;

            public ICommand SizeChangedCommand => _sizeChangedCommand ?? new RelayCommand((obj) =>
            {
                RenderSize = (Size) obj;
                UpdateAllChartElements();
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
