using System;

namespace CSSL.Utilities.Distributions
{
    public class LogNormalDistribution : Distribution
    {

        public LogNormalDistribution(double mu, double sigma) : base(Math.Exp(mu + (sigma * sigma) / 2), (Math.Exp(sigma * sigma) - 1) * Math.Exp(2 * mu + sigma * sigma))
        {
            Sigma = sigma;
            Mu = mu;
        }

        public double Sigma { get; }
        public double Mu { get; }

        public override double Next()
        {
            return Math.Exp(rnd.NextGaussian() * Sigma + Mu);
        }
    }
}
