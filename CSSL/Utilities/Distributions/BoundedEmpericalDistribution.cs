using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class BoundedEmpericalDistribution : Distribution
    {
        public BoundedEmpericalDistribution(double[] records, double lower, double upper) : base(records.Average(), records.Variance())
        {
            Records = records;
            Lower = lower;
            Upper = upper;
        }

        public double[] Records { get; }

        public double Lower { get; }

        public double Upper { get; }

        public override double Next()
        {
            double sample = Records[rnd.Next(0, Records.Length)];
            while (sample < Lower || sample > Upper)
            {
                sample = Records[rnd.Next(0, Records.Length)];
            }
            return sample;
        }
    }
}
