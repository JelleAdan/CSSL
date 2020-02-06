using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    interface IStatistic
    {
        double Average();

        double StandardDeviation();

        double Variance();

        double Min();

        double Max();

        int Count();
    }
}
