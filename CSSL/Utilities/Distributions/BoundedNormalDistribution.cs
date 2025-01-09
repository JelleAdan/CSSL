using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class BoundedNormalDistribution : Distribution
    {
        public BoundedNormalDistribution(double mu, double sigma, double lower, double upper) : base(mu, sigma * sigma)
        {
            if (mu < lower || mu > upper)
            {
                throw new ArgumentException("Mean must be within bounds");
            }
            if (lower >= upper)
            {
                throw new ArgumentException("Lower bound must be less than upper bound");
            }
            Sigma = sigma;
            LowerBound = lower;
            UpperBound = upper;
        }

        public double Sigma { get; }

        public double LowerBound { get; }

        public double UpperBound { get; }

        public override double Next()
        {
            double sample = Mean + Sigma * rnd.NextGaussian();
            while (sample < LowerBound || sample > UpperBound)
            {
                sample = Mean + Sigma * rnd.NextGaussian();
            }
            return sample;
        }
    }
}
