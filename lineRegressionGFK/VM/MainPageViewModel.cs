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

        void UpdateAllChartElements()
        {
            //UpdateChartPoints();
            // TODO
            UpdateVerticalLines();
            UpdateHorizontalLines();
        }

        #endregion

        #region Chart Properties    

        public ObservableCollection<DataSet> DataSetsOfPoints { get; set; } = new ObservableCollection<DataSet>()
        {
            new DataSet()
            {

            }
        };

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

            DataSetsOfPoints[CurrentIndexOfDataSet].AddPointToPointsCollection(xValue, yValue);

            UpdateAllChartElements();
        }

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
                UpdateAllChartElements();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Scale)));
            }
        }

        private int _currentIndexOfDataset = 0;

        public int CurrentIndexOfDataSet
        {
            get { return _currentIndexOfDataset; }
            set
            {
                _currentIndexOfDataset = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIndexOfDataSet)));
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
                                //PointsCollection.Clear();
                                DataSetsOfPoints.Add(new DataSet());
                                CurrentIndexOfDataSet++;
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
                        DataSetsOfPoints.Clear();
                        DataSetsOfPoints.Add(new DataSet()
                        {
                            
                        });
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
