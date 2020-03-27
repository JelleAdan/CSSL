using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    public class Statistic : StatisticBase
    {
        public Statistic(string name) : base(name)
        {
        }

        public override void Collect(double value)
        {
            count++;
            sumx += value;
            sumxx += value * value;
            sumw++;
            if (value > max)
            {
                max = value;
            }
            if (value < min)
            {
                min = value;
            }
        }

        public override void Collect(double value, double weight)
        {
            throw new NotImplementedException("Weighted value collection not supported. Use an instance of WeightedStatistic instead.");
        }
    }
}
