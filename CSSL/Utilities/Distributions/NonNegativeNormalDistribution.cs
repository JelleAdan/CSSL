using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class NonNegativeNormalDistribution : Distribution
    {
        public NonNegativeNormalDistribution(double mean, double sigma) : base(mean, sigma * sigma)
        {
            distribution = new NormalDistribution(mean, sigma);
        }

        private NormalDistribution distribution;

        public override double Next()
        {
            double sample = distribution.Next();
            while (sample <= 0)
            {
                sample = distribution.Next();
            }
            return sample;
        }
    }
}
