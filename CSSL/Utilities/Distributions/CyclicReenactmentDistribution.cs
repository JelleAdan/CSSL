using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class CyclicReenactmentDistribution : Distribution
    {
        public CyclicReenactmentDistribution(double[] records) : base(records.Average(), records.Variance())
        {
            Records = records;
            index = 0;
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
                index = 0;
                return Records[index++];
            }
        }

        public double NextFromRange(double lower, double upper)
        {
            double sample = Next();
            while (sample < lower || sample > upper)
            {
                sample = Next();
            }
            return sample;
        }

        public double Min => Records.Min();

        public double Max => Records.Max();

        public double Range => Max - Min;

        public double RangeMean(double lower, double upper)
        {
            var tmp = Records.Where(x => x >= lower && x <= upper).ToArray();
            if (tmp.Length == 0)
            {
                return 0;
            }
            return tmp.Sum() / tmp.Length;
        }

        public double RangeVariance(double lower, double upper)
        {
            var tmp = Records.Where(x => x >= lower && x <= upper).ToArray();
            if (tmp.Length == 0)
            {
                return 0;
            }
            var sampleMean = tmp.Sum() / tmp.Length;
            // return (tmp.Sum(x => Math.Pow(x, 2)) / tmp.Length - Math.Pow(sampleMean, 2)) / (tmp.Length - 1);
            return tmp.Sum(x => Math.Pow(x, 2)) / tmp.Length - Math.Pow(sampleMean, 2);
        }

        // public double Probability(double value)
        // {
        //     if (value < Min || value > Max)
        //     {
        //         return 0;
        //     }
        //     else if (Records.Count(x => x == value) == 0)
        //     {
        //         // TODO Interpolation?
        //     }
        //     else
        //     {
        //         return Records.Count(x => x == value) / Records.Length;
        //     }
        // }
    }
}
