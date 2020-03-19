using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Utilities.Distributions
{
    public class ExponentialDistribution : Distribution
    {
        public ExponentialDistribution(double rate) : base(1 / rate, 1 / rate / rate)
        {
            Rate = rate;
        }

        public double Rate { get; }

        public override double Next()
        {
            return -Math.Log(rnd.NextNonzeroDouble()) * Mean;
        }
    }
}
