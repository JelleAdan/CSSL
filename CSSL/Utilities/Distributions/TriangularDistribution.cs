using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class TriangularDistribution : Distribution
    {
        public TriangularDistribution(double lowerLimit, double upperLimit, double mode) : base((lowerLimit + upperLimit + mode) / 3, (lowerLimit * lowerLimit + upperLimit * upperLimit + mode * mode - lowerLimit * upperLimit - lowerLimit * mode - upperLimit * mode) / 18)
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            Mode = mode;
        }

        public double LowerLimit { get; }

        public double UpperLimit { get; }

        public double Mode { get; }

        public override double Next()
        {
            double u = rnd.NextDouble();
            double v = rnd.NextDouble();
            if (u > v)
            {
                double tmp = v;
                v = u;
                u = tmp;
            }
            return (1 - Mode) * u + Mode * v;
        }
    }
}
