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

        public double NextGaussian()
        {
            // This implementation is based on a Box-Muller transformation.
            // Each call can make two Gaussians for the price of one, this feature can be used to increase performance, but who cares...
            // The other Gaussian is calculated as follows Math.Sqrt(-2.0 * Math.Log(sample1)) * Math.Cos(2.0 * Math.PI * sample2)
            double sample1 = NextDouble();
            double sample2 = NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(sample1)) * Math.Sin(2.0 * Math.PI * sample2);
        }
    }
}
