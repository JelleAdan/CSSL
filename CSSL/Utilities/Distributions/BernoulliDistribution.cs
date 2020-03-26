using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class BernoulliDistribution : Distribution
    {
        public BernoulliDistribution(double probability) : base(probability, probability * (1 - probability))
        {
            Probability = probability;
        }

        public double Probability { get; }

        public override double Next()
        {
            if (rnd.NextDouble() < Probability)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
