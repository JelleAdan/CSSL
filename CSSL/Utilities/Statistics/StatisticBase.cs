using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    internal abstract class StatisticBase : CollectorBase, IStatistic
    {
        public StatisticBase(string name) : base(name)
        {
            Reset();
        }

        public override void Reset()
        {
            count = 0;
            sumx = 0;
            sumxx = 0;
            sumw = 0;
            max = double.MinValue;
            min = double.MaxValue;
        }

        protected int count;

        protected double sumx;

        protected double sumxx;

        protected double sumw;

        protected double max;

        protected double min;

        public double Average()
        {
            return sumx / sumw;
        }

        public int Count()
        {
            return count;
        }

        public double Max()
        {
            return max;
        }

        public double Min()
        {
            return min;
        }

        public double StandardDeviation()
        {
            return Math.Sqrt(Variance());
        }

        public double Variance()
        {
            return sumxx / sumw - sumx / sumw * sumx / sumw;
        }
    }
}
