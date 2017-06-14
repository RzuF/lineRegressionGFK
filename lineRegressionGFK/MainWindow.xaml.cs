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
        private Point _lastMousePosition;
        
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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPageViewModel.ManipulationStarted = true;
            _lastMousePosition = e.GetPosition(sender as Grid);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainPageViewModel.ManipulationStarted = false;            
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            MainPageViewModel.ManipulationStarted = false;
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(!MainPageViewModel.ManipulationStarted)
                return;
                        
            var mousePosition = e.GetPosition(sender as Grid);

            TranslateTransform translateTransform = new TranslateTransform((mousePosition.X - _lastMousePosition.X) / (Resources["MainPageViewModel"] as MainPageViewModel).Scale, (mousePosition.Y - _lastMousePosition.Y) / (Resources["MainPageViewModel"] as MainPageViewModel).Scale);
            _lastMousePosition = mousePosition;
            TransformGroup transformGroup = new TransformGroup();
            TransformGroup tg = (TransformGroup)ChartGridContainer.RenderTransform;
            foreach (Transform t in tg.Children)
                if (t is TranslateTransform)
                {
                    translateTransform.X += (t as TranslateTransform).X;
                    translateTransform.Y += (t as TranslateTransform).Y;
                }

            var sliderRBind1 = new System.Windows.Data.Binding
            {
                Source = (Resources["MainPageViewModel"] as MainPageViewModel),
                Path = new PropertyPath("Scale")
            };

            ScaleTransform scaleTransform = new ScaleTransform();
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, sliderRBind1);
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, sliderRBind1);

            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(scaleTransform);

            ChartGridContainer.RenderTransform = transformGroup;
            int x = 0;
        }
    }
}
