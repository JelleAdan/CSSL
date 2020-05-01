using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class EmpericalDistribution : Distribution
    {
        public EmpericalDistribution(double[] records) : base(records.Average(), records.Variance())
        {
            Records = records;
        }

        public double[] Records { get; }

        public override double Next()
        {
            return Records[rnd.Next(0, Records.Length)];
        }
    }
}
