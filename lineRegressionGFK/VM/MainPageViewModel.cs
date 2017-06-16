using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using lineRegressionGFK.Annotations;
using lineRegressionGFK.Models;
using Brush = System.Windows.Media.Brush;
using Clipboard = System.Windows.Clipboard;
using Color = System.Windows.Media.Color;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Size = System.Windows.Size;

namespace lineRegressionGFK.VM
{
    /// <summary>
    /// MainPage ViewModel for separating View from handling communication with models. Avoid code-behind approach. Whole project is based upon Bindings - controls with properties, one is changed the other changes too. Communicate via INotifyPropertyChanged interface implementation and sending signal if changed
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Private Variables

        /// <summary>
        /// Variable holds actual RenderSize of Chart Canvas
        /// </summary>
        private Size _renderSize;

        #endregion        

        #region Private Properties

        /// <summary>
        /// Property holds reference to actual MainWindow using this ViewModel
        /// </summary>
        public MainWindow MainWindow { get; set; }

        /// <summary>
        /// Property holds information about current maximum value of Xs collection from all DataSets, initially set to 100
        /// </summary>
        public double MaxXValue { get; private set; } = 100;
        /// <summary>
        /// Property holds information about current maximum value of Ys collection from all DataSets, initially set to 100
        /// </summary>
        public double MaxYValue { get; private set; } = 100;
        /// <summary>
        /// Property holds information about current minimum value of Xs collection from all DataSets, initially set to -100
        /// </summary>
        public double MinXValue { get; private set; } = -100;
        /// <summary>
        /// Property holds information about current minimum value of Ys collection from all DataSets, initially set to -100
        /// </summary>
        public double MinYValue { get; private set; } = -100;

        #endregion

        #region Private Helper Methods   

        /// <summary>
        /// Helper method for adding point to currently selected DataSet. Checks if point has one or more of extreme values. Forceses to update chart lines if not specified otherwise.
        /// </summary>
        /// <param name="xValue">X coordinate of Point</param>
        /// <param name="yValue">Y coordinate of Point</param>
        /// <param name="notUpdate">Specifies if UpdateAllChartElements() is fired. Default false.</param>
        private void AddPointToPointsCollection(double xValue, double yValue, bool notUpdate = false)
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

            if(!notUpdate)
                UpdateAllChartElements(true);
        }

        /// <summary>
        /// Helper method for building Horizontal List of ChartLine objects which will be drawn in View.
        /// </summary>
        void UpdateHorizontalLines()
        {
            List<ChartLine> newList = new List<ChartLine>();
            for (int i = 0; ;i++)
            {
                if (_renderSize.Height == 0)
                    break;
                var max = MaxYValue > -MinYValue ? MaxYValue : -MinYValue;
                double position = i * LineHeightDelta;
                if (i * LineHeightDelta > max && position * Scale > _renderSize.Height)
                    break;
                newList.Add(new ChartLine() {PositionFromBeggining = position, Size = _renderSize.Width * 2 > max * 2 * Scale ? _renderSize.Width * 2 : max * 2 * Scale, Opacity = LineHorizontalOpacity, StringValue = $"{-i * LineHeightDelta}"});
                if (i != 0)
                    newList.Add(new ChartLine() { PositionFromBeggining = -position, Size = _renderSize.Width * 2 > max * 2 * Scale ? _renderSize.Width * 2 : max * 2 * Scale, Opacity = LineHorizontalOpacity, StringValue = $"{i * LineHeightDelta}" });
            }
            HorizontalLinesCollection = newList;
        }

        /// <summary>
        /// Helper method for building Vertical List of ChartLine objects which will be drawn in View.
        /// </summary>
        void UpdateVerticalLines()
        {
            List<ChartLine> newList = new List<ChartLine>();
            for (int i = 0; ; i++)
            {
                if (_renderSize.Width == 0)
                    break;
                var max = MaxXValue > -MinXValue ? MaxXValue : -MinXValue;
                double position = i * LineWidthDelta;
                if (i * LineWidthDelta > max && position * Scale > _renderSize.Width)
                    break;
                newList.Add(new ChartLine() { PositionFromBeggining = position, Size = _renderSize.Height*2 > max * 2 * Scale ? _renderSize.Height * 2 : max * 2 * Scale, Opacity = LineVerticalOpacity, StringValue = $"{i * LineWidthDelta}" });
                if(i!=0)
                    newList.Add(new ChartLine() { PositionFromBeggining = -position, Size = _renderSize.Height * 2 > max * 2 * Scale ? _renderSize.Height * 2 : max * 2 * Scale, Opacity = LineVerticalOpacity, StringValue = $"{-i * LineWidthDelta}" });
            }
            VerticalLinesCollection = newList;
        }

        /// <summary>
        /// Helper method for updating horizontal and vertical chart lines and calculate optimum delta between lines.
        /// </summary>
        /// <param name="newDelta">If false, no new parameters for delta are calculated. Default false</param>
        void UpdateAllChartElements(bool newDelta = false)
        {
            if (newDelta)
            {
                LineWidthDelta = Math.Round((MaxXValue - MinXValue) / 200.0, 0) * 10;
                LineHeightDelta = Math.Round((MaxYValue - MinXValue) / 200.0, 0) * 10;
                if (LineWidthDelta > 100)
                    LineWidthDelta = Math.Round((MaxXValue - MinXValue) / 2000.0, 0) * 100;
                if (LineHeightDelta > 100)
                    LineHeightDelta = Math.Round((MaxYValue - MinYValue) / 2000.0, 0) * 100;
            }

            UpdateVerticalLines();
            UpdateHorizontalLines();
        }

        #endregion

        #region Window Properties

        /// <summary>
        /// Property holds information about MainWindow Title
        /// </summary>
        public string Title => "Projekt 10 - Regresja liniowa - Błahut, Górski, Gałat";
        /// <summary>
        /// Property holds information about MainWindow minimum Height and Width
        /// </summary>
        public int MinSize => 630;

        #endregion

        #region Chart Properties    

        /// <summary>
        /// Property holds DataSet objects. Initially specified with one DataSet object. ObservableCollection type notify View if any change in collection.
        /// </summary>
        public ObservableCollection<DataSet> DataSetsOfPoints { get; set; } = new ObservableCollection<DataSet>()
        {
            new DataSet()
            {
                Id = 1
            }
        };        

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string LineColorLabelText => "Pick line color";
        private Color _lineColor = Colors.White;
        /// <summary>
        /// Property holds information about chart lines color
        /// </summary>
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
        /// <summary>
        /// Property holds information about chart lines brush with color specified from LineColor property
        /// </summary>
        public Brush LineBrush => new SolidColorBrush(_lineColor);

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string BackgroundColorLabelText { get; } = "Pick background color";
        private Color _backgroundColor = Colors.Black;
        /// <summary>
        /// Property holds information about background color
        /// </summary>
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
        /// <summary>
        /// Property holds information about background brush with color specified from BackgroundColor property
        /// </summary>
        public Brush BackgroundBrush => new SolidColorBrush(_backgroundColor);                

        private List<ChartLine> _horizontalLinesCollection = new List<ChartLine>();
        /// <summary>
        /// Property holds horizontal List of ChartLine object which will be drawn in View
        /// </summary>
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
        /// <summary>
        /// Property holds vertical List of ChartLine object which will be drawn in View
        /// </summary>
        public List<ChartLine> VerticalLinesCollection
        {
            get { return _verticalLinesCollection; }
            set
            {
                _verticalLinesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerticalLinesCollection)));
            }
        }

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string YDeltaLabelText { get; set; } = "Height line delta: ";
        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string XDeltaLabelText { get; set; } = "Width line delta: ";

        private double _lineHeightDelta = 10;
        /// <summary>
        /// Property holds information about how high should be space between each horizontal line. Default 10.
        /// </summary>
        public double LineHeightDelta
        {
            get { return _lineHeightDelta; }
            set
            {
                _lineHeightDelta = value;
                UpdateHorizontalLines();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineHeightDelta)));
            }
        }
        private double _lineWidthDelta = 10;
        /// <summary>
        /// Property holds information about how wide should be space between each vertical line. Default 10.
        /// </summary> 
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
        /// <summary>
        /// Property holds information about opacity of each vertical line. Default 0.5
        /// </summary>
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
        /// <summary>
        /// Property holds information about opacity of each horizontal line. Default 0.5
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string ScaleLabelText { get; } = "Scale";
        private double _scale = 4;
        /// <summary>
        /// Property holds information about currenty used scale. Changing fire also UpdateAllChartElements() method. Default 4
        /// </summary>
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

        private int _currentIndexOfDataset;
        /// <summary>
        /// Property holds information about currenty selected DataSet
        /// </summary>
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

        /// <summary>
        /// Property holds reference to currently selected DataSet
        /// </summary>
        public DataSet CurrentDataSet => DataSetsOfPoints[_currentIndexOfDataset];

        #endregion

        #region Menu Properties

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string XLabelText { get; } = "X: ";
        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string YLabelText { get; } = "Y: ";

        private double _currentXValue;
        /// <summary>
        /// Property holds information about current X value in corresponding TextBox.
        /// </summary>
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
        /// <summary>
        /// Property holds information about current X value in corresponding TextBox.
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string AddButtonText => "Add";
        private ICommand _addButtonCommand;
        /// <summary>
        /// Command for adding point to DataSet. Reset CurrentXValue and CurrentYValue properties to default value 0. Use lazy initialization.
        /// </summary>
        public ICommand AddButtonCommand => _addButtonCommand ?? new RelayCommand((obj) =>
        {
            AddPointToPointsCollection(CurrentXValue, CurrentYValue);

            CurrentXValue = CurrentYValue = 0;
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string FromFileText => "From file...";
        private ICommand _fromFileCommand;
        /// <summary>
        /// Command for loading points from specified file. Load points to new DataSet. Use lazy initialization.
        /// </summary>
        public ICommand FromFileCommand => _fromFileCommand ?? new RelayCommand((obj) =>
        {
            var saveFileDialog = new OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1
            };

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
                            AddPointToPointsCollection(x, y, true);
                        }
                    }

                    if (CurrentDataSet.PointsCollection.Count == 0)
                    {
                        DeleteDataSetCommand.Execute(null);
                        return;
                    }

                    UpdateAllChartElements(true);
                }
            }
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string SaveAsText => "Save chart as...";
        private ICommand _saveAsCommand;
        /// <summary>
        /// Command for saving chart as picture. Use lazy initialization.
        /// </summary>
        public ICommand SaveAsCommand => _saveAsCommand ?? new RelayCommand((obj) =>
        {
            var saveFileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = "png files (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var renderTargetBitmap = new RenderTargetBitmap((int) MainWindow.ChartGrid.ActualWidth,
                    (int) MainWindow.ChartGrid.ActualHeight, 96, 96, PixelFormats.Default);
                renderTargetBitmap.Render(MainWindow.ChartGrid);
                using (var saveStream = saveFileDialog.OpenFile())
                {
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    encoder.Save(saveStream);
                }
            }
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string CopyToClipboardText => "Copy chart to clipboard";
        private ICommand _copyToClipboardCommand;
        /// <summary>
        /// Command for saving chart to clipboard. Use lazy initialization.
        /// </summary>
        public ICommand CopyToClipboardCommand => _copyToClipboardCommand ?? new RelayCommand((obj) =>
        {          
            var renderTargetBitmap = new RenderTargetBitmap((int) MainWindow.ChartGrid.ActualWidth,
                (int) MainWindow.ChartGrid.ActualHeight, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(MainWindow.ChartGrid);
            Clipboard.SetImage(renderTargetBitmap);
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string ClearAllText => "Clear all DataSets";
        private ICommand _clearAllCommand;
        /// <summary>
        /// Command for clearing all DataSets. Restore default application state. Use lazy initialization.
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string ClearSingleText => "Clear current DataSet";
        private ICommand _clearSingleCommand;
        /// <summary>
        /// Command for clearing single DataSet. Restore extreme values to actual state. Use lazy initialization.
        /// </summary>
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

            if (CurrentIndexOfDataSet == -1)
                CurrentIndexOfDataSet = 0;
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string AddDataSetText => "Add new DataSet";
        private ICommand _addDataSetCommand;
        /// <summary>
        /// Command for adding new clear DataSet. Use lazy initialization.
        /// </summary>
        public ICommand AddDataSetCommand => _addDataSetCommand ?? new RelayCommand((obj) =>
        {
            DataSetsOfPoints.Add(new DataSet()
            {
                Id = DataSetsOfPoints.Last().Id + 1
            });
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string DeleteDataSetText => "Delete current DataSet";
        private ICommand _deleteDataSetCommand;
        /// <summary>
        /// Command for deleting current DataSet. If it is the last set, proper MessageBox is fired. Use lazy initialization.
        /// </summary>
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

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string PrintText => "Print Chart";
        private ICommand _printCommand;
        /// <summary>
        /// Command for printing chart image. Use lazy initialization.
        /// </summary>
        public ICommand PrintCommand => _printCommand ?? new RelayCommand((obj) =>
        {
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(MainWindow.ChartGrid, "Chart");
            }
        });

        /// <summary>
        /// Property holds text to display in Label
        /// </summary>
        public string ToCenterText { get; } = "Center chart";
        private ICommand _toCenterCommand;
        /// <summary>
        /// Command for centering chart. Use lazy initialization.
        /// </summary>
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

        #endregion
    }
}
