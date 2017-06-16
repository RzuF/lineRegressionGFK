using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lineRegressionGFK.Models;

namespace lineRegressionGFK.Helpers
{
    public static class PolynomialLineCreatorHelper
    {
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
