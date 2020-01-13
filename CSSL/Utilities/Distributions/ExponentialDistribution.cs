using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Utilities.Distributions
{
    public class ExponentialDistribution : Distribution
    {
        public ExponentialDistribution(double mean, double variance) : base(mean, variance)
        {
        }

        public override double Next()
        {
            return -Math.Log(rnd.NextNonzeroDouble()) * Mean;
        }
    }
}
