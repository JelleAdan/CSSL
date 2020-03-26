using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    class WeightedStatistic : StatisticBase
    {
        public WeightedStatistic(string name) : base(name)
        {
        }

        public override void Collect(double value)
        {
            throw new NotImplementedException("Unweighted value collection not supported. Use an instance of Statistic instead.");
        }

        public override void Collect(double value, double weight)
        {
            count++;
            sumx += weight * value;
            sumxx += weight * value * value;
            sumw += weight;
            if (value > max)
            {
                max = value;
            }
            if (value < min)
            {
                min = value;
            }
        }
    }
}
