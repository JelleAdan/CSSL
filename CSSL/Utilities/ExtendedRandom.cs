using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Utilities
{
    public class ExtendedRandom : Random
    {
        public double NextNonzeroDouble()
        {
            double sample = NextDouble();
            while (sample == 0)
            {
                sample = NextDouble();
            }
            return sample;
        }
    }
}
