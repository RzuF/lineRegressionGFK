using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        #region Private Variables

        private Size _renderSize;

        #endregion        

        #region Private Properties

        public MainWindow MainWindow { get; set; }
        public double MaxXValue { get; private set; } = 100;
        public double MaxYValue { get; private set; } = 100;
        public double MinXValue { get; private set; } = -100;
        public double MinYValue { get; private set; } = -100;

        #endregion

        #region Private Helper Methods   

        private void AddPointToPointsCollection(double xValue, double yValue)
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

            LineWidthDelta = (MaxXValue - MinXValue) / 20.0;
            LineHighDelta = (MaxYValue - MinXValue) / 20.0;
            if (LineWidthDelta > 100)
                LineWidthDelta = Math.Round((MaxXValue - MinXValue) / 2000.0, 0) * 100;
            if (LineHighDelta > 100)
                LineHighDelta = Math.Round((MaxYValue - MinYValue) / 2000.0, 0) * 100;

            UpdateAllChartElements();
        }

        void UpdateHorizontalLines()
        {
            List<ChartLine> _newList = new List<ChartLine>();
            int i = 0;
            for (i = 0; ;i++)
            {
                if (_renderSize.Height == 0)
                    break;
                var max = MaxYValue > -MinYValue ? MaxYValue : -MinYValue;
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
                var max = MaxXValue > -MinXValue ? MaxXValue : -MinXValue;
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
            UpdateVerticalLines();
            UpdateHorizontalLines();
        }

        #endregion

        #region Chart Properties    

        public ObservableCollection<DataSet> DataSetsOfPoints { get; set; } = new ObservableCollection<DataSet>()
        {
            new DataSet()
            {
                Id = 1
            }
        };        

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
                if(value != -1)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDataSet)));
            }
        }

        public DataSet CurrentDataSet => DataSetsOfPoints[_currentIndexOfDataset];

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

        #endregion

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
                    DataSetsOfPoints.Add(new DataSet()
                    {
                        Id = DataSetsOfPoints.Last().Id + 1
                    });
                    CurrentIndexOfDataSet++;
                    string readFile = saveStream.ReadToEnd();
                    var fileLines = readFile.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var fileLine in fileLines)
                    {
                        var xy = fileLine.Replace(",", ".").Split(new[] { " ", ";", "\t" },
                            StringSplitOptions.RemoveEmptyEntries);
                        double x, y;
                        if (xy.Length == 2 && double.TryParse(xy[0], NumberStyles.Any, new CultureInfo("en-US"), out x) && double.TryParse(xy[1], NumberStyles.Any, new CultureInfo("en-US"), out y))
                        {
                            AddPointToPointsCollection(x, y);
                        }
                    }
                }
            }
        });

        public string SaveAsText => "Save chart as...";
        private ICommand _saveAsCommand;

        public ICommand SaveAsCommand => _saveAsCommand ?? new RelayCommand((obj) =>
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog() { RestoreDirectory = true };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var renderTargetBitmap = new RenderTargetBitmap((int)MainWindow.ChartGrid.RenderSize.Width,
                    (int)MainWindow.ChartGrid.RenderSize.Height, 96d, 85d, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(MainWindow.ChartGrid);
                using (var saveStream = saveFileDialog.OpenFile())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    encoder.Save(saveStream);
                }
            }
        });

        public string ClearAllText => "Clear all DataSets";
        private ICommand _clearAllCommand;

        public ICommand ClearAllCommand => _clearAllCommand ?? new RelayCommand((obj) =>
        {
            MaxXValue = 100;
            MaxYValue = 100;
            MinXValue = -100;
            MinYValue = -100;
            DataSetsOfPoints = new ObservableCollection<DataSet>()
            {
                new DataSet()
                {
                    Id = 1
                }
            };
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataSetsOfPoints)));
            CurrentIndexOfDataSet = 0;
        });

        public string ClearSingleText => "Clear current DataSet";
        private ICommand _clearSingleCommand;

        public ICommand ClearSingleCommand => _clearSingleCommand ?? new RelayCommand((obj) =>
        {
            DataSetsOfPoints[CurrentIndexOfDataSet] = new DataSet()
            {
                Id = DataSetsOfPoints[CurrentIndexOfDataSet].Id
            };

            MaxXValue = 100;
            MaxYValue = 100;
            MinXValue = -100;
            MinYValue = -100;

            foreach (var dataSetsOfPoint in DataSetsOfPoints)
            {
                if (dataSetsOfPoint.MaxXValue > MaxXValue)
                    MaxXValue = dataSetsOfPoint.MaxXValue;

                if (dataSetsOfPoint.MinYValue < MinYValue)
                    MinYValue = dataSetsOfPoint.MinYValue;

                if (dataSetsOfPoint.MaxYValue > MaxYValue)
                    MaxYValue = dataSetsOfPoint.MaxYValue;

                if (dataSetsOfPoint.MinXValue < MinXValue)
                    MinXValue = dataSetsOfPoint.MinXValue;
            }
        });

        public string AddDataSetText => "Add new DataSet";
        private ICommand _addDataSetCommand;

        public ICommand AddDataSetCommand => _addDataSetCommand ?? new RelayCommand((obj) =>
        {
            DataSetsOfPoints.Add(new DataSet()
            {
                Id = DataSetsOfPoints.Last().Id + 1
            });
        });

        public string DeleteDataSetText => "Delete current DataSet";
        private ICommand _deleteDataSetCommand;

        public ICommand DeleteDataSetCommand => _deleteDataSetCommand ?? new RelayCommand((obj) =>
        {
            if (DataSetsOfPoints.Count > 1)
                DataSetsOfPoints.RemoveAt(CurrentIndexOfDataSet);
            else
            {
                System.Windows.MessageBox.Show(MainWindow, "Cannot delete all datasets", "Error", MessageBoxButton.OK);
            }
        });

        private ICommand _sizeChangedCommand;

        public ICommand SizeChangedCommand => _sizeChangedCommand ?? new RelayCommand((obj) =>
        {
            _renderSize = (Size)obj;
        });

        private ICommand _radioChangedCommand;

        public ICommand RadioChangedCommand => _radioChangedCommand ?? new RelayCommand((obj) =>
        {

        });

        public string PrintText => "Print Chart";
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
            TranslateTransform translateTransform = new TranslateTransform(_renderSize.Width / 2, _renderSize.Height / 2);

            transformGroup.Children.Add(translateTransform);

            (obj as Grid).RenderTransform = transformGroup;

            Scale = Scale;
        });

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
