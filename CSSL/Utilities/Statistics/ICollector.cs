using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Statistics
{
    interface ICollector
    {
        void Collect(double value);

        void Collect(double value, double weight);

        void Reset();
    }
}
