using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace lineRegressionGFK.Helpers
{
    /// <summary>
    /// Helper method for calculating regression coefficients and standard deviation for linear regression
    /// </summary>
    public static class RegressionHelper
    {       
        /// <summary>
        /// Calculates standard deviation for the intercept and the slope of linear regression
        /// </summary>
        /// <param name="x">Array of X set</param>
        /// <param name="y">Array of Y set</param>
        /// <returns>Tuple conatining (stdDevA, stdDevB)</returns>
        public static Tuple<double, double> LinearStdDev(double[] x, double[] y)
        {
            double stdDevSqA = 0, stdDevSqB = 0;
            double stdDevSqY = 0, xSqSum = 0;
            int n = x.Length;

            if (n <= 2) return new Tuple<double, double>(0, 0);
            var line = Polynomial(x, y, 1);

            for (int i = 0; i < n; i++)
            {
                stdDevSqY += Math.Pow(y[i] - line[0] - line[1] * x[i], 2);
                xSqSum += x[i] * x[i];
            }
            double delta = n * xSqSum - x.Sum() * x.Sum();
            stdDevSqA = (n / ((double)n - 2)) * stdDevSqY / delta;
            stdDevSqB = stdDevSqA * (xSqSum / n);
            return new Tuple<double, double>(Math.Sqrt(stdDevSqA), Math.Sqrt(stdDevSqB));
        }

        /// <summary>
        /// Least-Squares fitting the points (x,y) to a curve y : x -> a0+a1*x+a2*x^2+a3*x^3+...+abaseDegree*x^baseDegree
        /// </summary>
        /// <param name="x">Array of X set</param>
        /// <param name="y">Array of Y set</param>
        /// <param name="baseDegree">Specified degree of polynomial</param>
        /// <returns>Best fitting parameters as [a0, a1, a2, ..., abaseDegree] array 'baseDegree + 1' sized</returns>
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

        /// <summary>
        /// Calculates coefficients for orthogonal regression
        /// </summary>
        /// <param name="x">Array of X set</param>
        /// <param name="y">Array of Y set</param>
        /// <returns>Orthogonal coefficients as arary: [a0, a1] -> y = a0 + a1x</returns>
        public static double[] Orthogonal(double[] x, double[] y)
        {
            int size = x.Length;
            double avrX = x.Average();
            double avrY = y.Average();
            double sXX = 0, sXY = 0, sYY = 0;
            for (int i = 0; i < size; i++)
            {
                sXX += Math.Pow(x[i] - avrX, 2);
                sXY += (x[i] - avrX) * (y[i] - avrY);
                sYY += Math.Pow(y[i] - avrY, 2);
            }
            sXX /= size - 1; sXY /= size - 1; sYY /= size - 1;
            double a1 = (sYY - sXX + Math.Sqrt(Math.Pow(sYY - sXX, 2) + 4 * sXY * sXY)) / (2 * sXY);
            double a0 = avrY - a1 * avrX;
            return new double[2] { a0, a1 };
        }
    }
}
