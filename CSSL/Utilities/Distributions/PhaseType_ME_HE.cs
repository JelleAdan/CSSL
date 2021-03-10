using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class PhaseType_ME_HE : Distribution
    {
        // Implementation of a two-phase type distribution: Mixture Erlang & Hyper Exponential

        public PhaseType_ME_HE(double mean, double std) : base(mean, std)
        {
            c2 = Math.Pow((std / mean), 2);
            p1 = (1.0 + Math.Sqrt((c2 - 1.0) / (c2 + 1.0))) / 2.0;
            p2 = 1.0 - p1;
            mu1 = 2.0 * p1 / mean;
            mu2 = 2.0 * p2 / mean;

        }

        public double c2 { get; }
        public double p1 { get; }
        public double p2 { get; }
        public double mu1 { get; }
        public double mu2 { get; }

        public double exp(double MU)
        {
            return -Math.Log(rnd.NextNonzeroDouble()) / MU;
        }

        public override double Next()
        {
            double x = 0.0;
            double p = 0.0;
            double mu = 0.0;
            int k;

            if (c2 <= 1.0) // Fit mixture of Erlang-(k-1) and Erlang-k
            {
                for (k = 2; 1.0 / k > c2; k++)
                {
                }
                p = (k * c2 - Math.Sqrt(k * (1.0 + c2) - Math.Pow(k, 2) * c2)) / (1.0 + c2);
                mu = (k - p) / Mean;
                for (int l = 1; l < k; l++)
                    x += exp(mu);
                if (rnd.NextNonzeroDouble() > p)
                    x += exp(mu);
            }
            else //fit Hyper-exponential with balanced means
            {
                if (rnd.NextNonzeroDouble() < p1)
                    x = exp(mu1);
                else
                    x = exp(mu2);
            }

            return x;
        }
    }
}
