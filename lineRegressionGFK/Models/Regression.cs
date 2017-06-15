using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace lineRegressionGFK.Models
{
    public static class Regression
    {
        //     Least-Squares fitting the points (x,y) to a line y : x -> a0+a1*x, returning its
        //     best fitting parameters as [a0, a1] array, where a0 is the intercept and a1 the slope.
        public static Tuple<double, double> Linear(double[] x, double[] y)
        {
            double[] result = Polynomial(x, y, 1);
            return new Tuple<double, double>(result[0], result[1]);
        }
        //     Calculates standard deviation for the intercept and the slope of linear regression
        //     Returns (stdDevA, stdDevB) - rather correct.
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
                stdDevSqY += Math.Pow(y[i] - line.Item1 - line.Item2 * x[i], 2);
                xSqSum += x[i] * x[i];
            }
            double delta = n * xSqSum - x.Sum() * x.Sum();
            stdDevSqA = (n / ((double)n - 2)) * stdDevSqY / delta;
            stdDevSqB = stdDevSqA * (xSqSum / n);
            return new Tuple<double, double>(Math.Sqrt(stdDevSqA), Math.Sqrt(stdDevSqB));
        }
        //     Least-Squares fitting the points (x,y) to a curve y : x -> a0+a1*x+a2*x^2+a3*x^3+...+abaseDegree*x^baseDegree, returning its
        //     best fitting parameters as [a0, a1, a2, ..., abaseDegree] array 'baseDegree + 1' sized.
        public static double[] Polynomial(double[] x, double[] y, int baseDegree)
        {
            int size = x.Length;
            double[] sumsOfXToPower = new double[2 * baseDegree + 1];
            double[] sumsOfYTimesXToPower = new double[baseDegree + 1];

            for (int i = 0; i < 2 * baseDegree + 1; i++)
                for (int j = 0; j < size; j++)
                {
                    sumsOfXToPower[i] += Math.Pow(x[j], i);
                    if (i < baseDegree + 1) sumsOfYTimesXToPower[i] += y[j] * Math.Pow(x[j], i);
                }

            double[,] X = new double[baseDegree + 1, baseDegree + 1];

            for (int i = 0; i < baseDegree + 1; i++)
                for (int j = 0; j < baseDegree + 1; j++)
                    X[i, j] = sumsOfXToPower[i + j];

            Matrix<double> matrixX = Matrix<double>.Build.DenseOfArray(X);
            Vector<double> vectorY = Vector<double>.Build.Dense(sumsOfYTimesXToPower);
            Vector<double> vectorA = matrixX.Solve(vectorY);
            return vectorA.ToArray();
        }
    }
}
