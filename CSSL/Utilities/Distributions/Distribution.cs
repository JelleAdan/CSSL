using CSSL.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Utilities.Distributions
{
    [Serializable()]
    public abstract class Distribution
    {
        protected ExtendedRandom rnd;

        public static Random rndStatic = null;

        public double Mean { get; }

        public double Variance { get; }

        public Distribution(double mean, double variance)
        {
            Mean = mean;
            Variance = variance;
            rnd = new ExtendedRandom(Settings.SeedGenerator.Next());
        }

        public abstract double Next();
    }
}
