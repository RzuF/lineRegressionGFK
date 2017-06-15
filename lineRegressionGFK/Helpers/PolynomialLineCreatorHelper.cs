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
        public static List<ChartPolynomialPart> Create(Func<double, double> polynomial, double start, double end, double step)
        {
            double range = end - start;
            List<ChartPolynomialPart> chartPolynomialParts = new List<ChartPolynomialPart>
            {
                new ChartPolynomialPart()
                {
                    XStart = start,
                    YStart = polynomial(start),

                }
            };

            for (double i = start+step; i <= end; i += step)
            {
                double currentValueOfPolynomialStep = polynomial(i);
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
    }
}
