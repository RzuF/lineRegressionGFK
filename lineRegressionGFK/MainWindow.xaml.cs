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
        /// <summary>
        /// Private variable for last mouse position for ChartGrid translation
        /// </summary>
        private Point _lastMousePositionChartGrid;
        /// <summary>
        /// Private variable for last mouse position for InfoFrame translation
        /// </summary>
        private Point _lastMousePositionInfoFrame;

        /// <summary>
        /// Indicator if manipulation has started for ChartGrid
        /// </summary>
        private bool _manipulationStartedChartGrid;
        /// <summary>
        /// Indicator if manipulation has started for InforFrame
        /// </summary>
        private bool _manipulationStartedInfoFrame;
        /// <summary>
        /// Indicator if manipulation can be started for ChartGrid. False if mouse is in InfoFrame area. Deafult true
        /// </summary>
        private bool _canManipulateChart = true;

        /// <summary>
        /// Constructor initializing components and setting itself to ViewModel
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            (Resources["MainPageViewModel"] as MainPageViewModel).MainWindow = this;   
        }

        /// <summary>
        /// Code-behind event method. Fired if left mouse button has been clicked in ChartGrid (and InfoFrame) area. Sets indicator of manipulation started.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
        private void ChartGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_canManipulateChart)
            {
                _manipulationStartedInfoFrame = true;
                _lastMousePositionInfoFrame = e.GetPosition(sender as Label);
            }
            else
            {
                _manipulationStartedChartGrid = true;
                _lastMousePositionChartGrid = e.GetPosition(sender as Grid);
            }            
        }

        /// <summary>
        /// Code-behind event method. Fired if left mouse button has been released. Ends manipulation.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
        private void ChartGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _manipulationStartedChartGrid = false;
            _manipulationStartedInfoFrame = false;
        }

        /// <summary>
        /// Code-behind event method. Fired if mouse leave ChartGrid area. Ends manipulation.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
        private void ChartGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _manipulationStartedChartGrid = false;
            _manipulationStartedInfoFrame = false;
        }

        /// <summary>
        /// Code-behind event method. Fired if mouse has moved. Due to indicator of which manipulation is happening, applies translate to a proper object.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Code-behind event method. Fired if mouse leave InfoFrame area. Informs that InfoFrame cannot be manipulated.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
        private void InfoFrame_OnMouseLeave(object sender, MouseEventArgs e)
        {            
            _canManipulateChart = true;
        }

        /// <summary>
        /// Code-behind event method. Fired if mouse enter InfoFrame area. Informs that InfoFrame must be manipulated.
        /// </summary>
        /// <param name="sender">Object that send this event</param>
        /// <param name="e"></param>
        private void InfoFrame_OnMouseEnter(object sender, MouseEventArgs e)
        {
            _canManipulateChart = false;
        }
    }
}
