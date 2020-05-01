using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class ReenactmentDistribution : Distribution
    {
        public ReenactmentDistribution(double[] records) : base(records.Average(), records.Variance())
        {
            Records = records;
            Reset();
        }

        private int index;

        public double[] Records { get; }

        public override double Next()
        {
            if (index < Records.Length)
            {
                return Records[index++];
            }
            else
            {
                throw new Exception("Reenactment distribution records depleted.");
            }
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
