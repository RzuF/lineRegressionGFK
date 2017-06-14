using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lineRegressionGFK.Models
{
    public static class Regression
    {
        //     Least-Squares fitting the points (x,y) to a line y : x -> a*b+b, returning its
        //     best fitting parameters as [a, b] array, where a is the intercept and b the slope.
        public static Tuple<double, double> Linear(double[] x, double[] y)
        {
            double a = 0, b = 0;
            double xSqSum = 0, xTimesYSum = 0;
            int n = x.Length;

            for (int i = 0; i < n; i++)
            {
                xSqSum += x[i] * x[i];
                xTimesYSum += x[i] * y[i];
            }
            double delta = n * xSqSum - x.Sum() * x.Sum();
            a = (n * xTimesYSum - x.Sum() * y.Sum()) / delta;
            b = (xSqSum * y.Sum() - x.Sum() * xTimesYSum) / delta;
            return new Tuple<double, double>(a, b);
        }
        //     Calculates standard deviation for the intercept and the slope of linear regression
        //     Returns (stdDevA, stdDevB)
        public static Tuple<double, double> LinearStdDev(double[] x, double[] y)
        {
            double stdDevSqA = 0, stdDevSqB = 0;
            double stdDevSqY = 0, xSqSum = 0;
            int n = x.Length;
            // If n == 2 -> stdDev doesn't exist -> ...1/(n-2)... Do you prefer NaN in the result? If so remove following line
            if (n <= 2) return new Tuple<double, double>(0, 0);
            var line = Linear(x, y);

            for (int i = 0; i < n; i++)
            {
                stdDevSqY += Math.Pow(y[i] - line.Item2 - line.Item1 * x[i], 2);
                xSqSum += x[i] * x[i];
            }
            double delta = n * xSqSum - x.Sum() * x.Sum();
            stdDevSqA = (n / ((double)n - 2)) * stdDevSqY / delta;
            stdDevSqB = stdDevSqA * (xSqSum / n);
            return new Tuple<double, double>(Math.Sqrt(stdDevSqA), Math.Sqrt(stdDevSqB));
        }
    }
}
