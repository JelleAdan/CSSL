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

        public double Probability(double a, double b)
        {
            if (a > b)
            {
                throw new ArgumentException("a must be less than b");
            }
            return CumulativeDensity(Standardize(b)) - CumulativeDensity(Standardize(a));
        }

        public double Standardize(double x)
        {
            return (x - Mean) / Sigma;
        }

        public double CumulativeDensity(double z)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;
            
            int sign = z < 0 ? -1 : 1;
            double x = Math.Abs(z) / Math.Sqrt(2.0);
            
            // A&S (Handbook of Mathematical Functions, by Abramowitz and Stegun) formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);
                
            return 0.5 * (1.0 + sign * y);
        }
    }
}
