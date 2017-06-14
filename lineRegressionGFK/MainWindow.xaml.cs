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

namespace lineRegressionGFK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
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
    }
}
