using System;
using System.Collections.Generic;
using System.Linq;
using lineRegressionGFK.Models;

namespace lineRegressionGFK.Helpers
{
    /// <summary>
    /// Helper static class for calculation coordinated of each polynomial part.
    /// </summary>
    public static class PolynomialLineCreatorHelper
    {
        /// <summary>
        /// Method for creating parts of any polynomial graphical representation.
        /// </summary>
        /// <param name="coefficients">Coefficients of polynomial. From less significant to the most eg. a0 + a1*x + a2*x*x...</param>
        /// <param name="start">X coordinate of starting point</param>
        /// <param name="end">X coordinate of ending point</param>
        /// <param name="step">How big is single step of line polynomial part</param>
        /// <returns>List of parts for graphical representation</returns>
        public static List<ChartPolynomialPart> Create(double[] coefficients, double start, double end, double step)
        {
            List<ChartPolynomialPart> chartPolynomialParts = new List<ChartPolynomialPart>
            {
                new ChartPolynomialPart()
                {
                    XStart = start,
                    YStart = CalculateValue(start, coefficients),
                }
            };

            for (double i = start+step; i <= end; i += step)
            {
                if (Math.Abs(i) < 0.1 && Math.Abs(i) >= 0)
                {
                    var x = chartPolynomialParts.Last();
                }
                double currentValueOfPolynomialStep = CalculateValue(i, coefficients);
                chartPolynomialParts.Last().XEnd = i;
                chartPolynomialParts.Last().YEnd = currentValueOfPolynomialStep;
                chartPolynomialParts.Add(new ChartPolynomialPart()
                {
                    XStart = i,
                    YStart = currentValueOfPolynomialStep
                });
            }

            if(chartPolynomialParts.Last().XEnd == 0)
                chartPolynomialParts.RemoveAt(chartPolynomialParts.Count-1);

            return chartPolynomialParts;
        }

        /// <summary>
        /// Helper method for calculating Y value of passed X using coefficients array parameter
        /// </summary>
        /// <param name="x">X coordinate of point to calculate Y value</param>
        /// <param name="coefficients">array of polynomial coefficients</param>
        /// <returns>Value of Y corresponding with passed X</returns>
        private static double CalculateValue(double x, double[] coefficients)
        {
            double result = 0;
            for (int i = 0; i < coefficients.Length; i++)
            {
                result += coefficients[i] * Math.Pow(x, i);
            }

            result = Math.Round(result, 5);

            return result;
        }
    }
}
