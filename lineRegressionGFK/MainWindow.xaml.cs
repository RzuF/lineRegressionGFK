using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using lineRegressionGFK.VM;
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

        private void InfoFrame_OnMouseLeave(object sender, MouseEventArgs e)
        {            
            _canManipulateChart = true;
        }

        private void InfoFrame_OnMouseEnter(object sender, MouseEventArgs e)
        {
            _canManipulateChart = false;
        }
    }
}
