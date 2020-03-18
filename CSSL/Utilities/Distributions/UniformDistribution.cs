using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class UniformDistribution : Distribution
    {
        public UniformDistribution(double lowerBound, double upperBound) : base(0.5 * (lowerBound + upperBound), 1 / 12 * Math.Pow((upperBound - lowerBound), 2))
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public double LowerBound { get; }

        public double UpperBound { get; }

        public override double Next()
        {
            return rnd.NextDouble() * (UpperBound - LowerBound) + LowerBound;
        }
    }
}
