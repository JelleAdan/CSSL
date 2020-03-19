using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class NormalDistribution : Distribution
    {
        public NormalDistribution(double mu, double sigma) : base(mu, sigma * sigma)
        {
            Sigma = sigma;
        }

        public double Sigma { get; }

        public override double Next()
        {
            return Mean + Sigma * rnd.NextGaussian();
        }
    }
}
