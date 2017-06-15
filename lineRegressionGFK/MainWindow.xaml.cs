using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using lineRegressionGFK.VM;
using System.Windows.Forms;
using lineRegressionGFK.Models;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace lineRegressionGFK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _lastMousePositionChartGrid;
        private Point _lastMousePositionInfoFrame;

        private bool _manipulationStartedChartGrid;
        private bool _manipulationStartedInfoFrame;
        private bool _canManipulateChart = true;

        public MainWindow()
        {
            InitializeComponent();
            (Resources["MainPageViewModel"] as MainPageViewModel).MainWindow = this;

#if DEBUG
            ObservableCollection<ChartPoint> mockCollection = new ObservableCollection<ChartPoint>()
            {
                new ChartPoint() {X = 0, Y = 0},
                new ChartPoint() {X = 10, Y = 10},
                new ChartPoint() {X = 20, Y = 40},
                new ChartPoint() {X = 30, Y = 30},
                new ChartPoint() {X = 40, Y = 50},
                new ChartPoint() {X = 50, Y = 60},
                new ChartPoint() {X = 60, Y = 70},
                new ChartPoint() {X = 70, Y = 65},
            };

            foreach (var chartPoint in mockCollection)
            {
                (Resources["MainPageViewModel"] as MainPageViewModel)?.AddPointToPointsCollection((int)chartPoint.X, (int)chartPoint.Y);
            }
#endif            
        }

        private void ChartGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_canManipulateChart)
            {
                _manipulationStartedInfoFrame = true;
                _lastMousePositionInfoFrame = e.GetPosition(sender as System.Windows.Controls.Label);
            }
            else
            {
                _manipulationStartedChartGrid = true;
                _lastMousePositionChartGrid = e.GetPosition(sender as Grid);
            }            
        }

        private void ChartGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _manipulationStartedChartGrid = false;
            _manipulationStartedInfoFrame = false;
        }

        private void ChartGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _manipulationStartedChartGrid = false;
            _manipulationStartedInfoFrame = false;
        }

        private void ChartGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_manipulationStartedChartGrid)
            {
                var mousePosition = e.GetPosition(sender as Grid);

                TranslateTransform translateTransform =
                    new TranslateTransform((mousePosition.X - _lastMousePositionChartGrid.X),
                        (mousePosition.Y - _lastMousePositionChartGrid.Y));
                _lastMousePositionChartGrid = mousePosition;
                TransformGroup transformGroup = new TransformGroup();
                TransformGroup tg = (TransformGroup) ChartGridContainer.RenderTransform;
                foreach (Transform t in tg.Children)
                    if (t is TranslateTransform)
                    {
                        translateTransform.X += (t as TranslateTransform).X;
                        translateTransform.Y += (t as TranslateTransform).Y;
                    }
                transformGroup.Children.Add(translateTransform);

                ChartGridContainer.RenderTransform = transformGroup;
            }

            if (_manipulationStartedInfoFrame)
            {
                var mousePosition = e.GetPosition(sender as System.Windows.Controls.Label);

                TranslateTransform translateTransform = new TranslateTransform((mousePosition.X - _lastMousePositionInfoFrame.X), (mousePosition.Y - _lastMousePositionInfoFrame.Y));
                _lastMousePositionInfoFrame = mousePosition;
                TransformGroup transformGroup = new TransformGroup();
                TransformGroup tg = (TransformGroup)InfoFrameLabel.RenderTransform;
                foreach (Transform t in tg.Children)
                    if (t is TranslateTransform)
                    {
                        translateTransform.X += (t as TranslateTransform).X;
                        translateTransform.Y += (t as TranslateTransform).Y;
                    }
                transformGroup.Children.Add(translateTransform);

                InfoFrameLabel.RenderTransform = transformGroup;
            }
        }

        private void InfoFrame_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {                      
            
        }

        private void InfoFrame_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _manipulationStartedInfoFrame = false;
        }

        private void InfoFrame_OnMouseLeave(object sender, MouseEventArgs e)
        {            
            _canManipulateChart = true;
        }

        private void InfoFrame_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_manipulationStartedInfoFrame)
                return;

            
        }

        private void InfoFrame_OnMouseEnter(object sender, MouseEventArgs e)
        {
            _canManipulateChart = false;
        }
    }
}
