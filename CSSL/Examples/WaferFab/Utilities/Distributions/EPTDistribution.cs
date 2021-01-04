using CSSL.Utilities.Distributions;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.ExceptionServices;
using System.Text;

namespace CSSL.Examples.WaferFab.Utilities
{
    [Serializable]
    /// <summary>
    /// WIP-dependent EPT distribution
    /// </summary>
    public class EPTDistribution : Distribution
    {
        // Mean and variance set in base are wrong and should nog be used. This distribution does not have a single mean and variance, since it is WIP-dependent
        public EPTDistribution(WIPDepDistParameters parameters) : base(parameters.Twipmax, parameters.Cwipmax)
        {
            par = parameters;

            Distributions = new Dictionary<int, Distribution>();

            for (int WIP = 1; WIP <= par.UBWIP; WIP++)
            {
                // TEMP
                // if (WIP < par.LBWIP) Distributions.Add(WIP, new GammaDistribution(2 * MeanAtWIP(WIP), VarianceAtWIP(WIP)));
                //else 
                Distributions.Add(WIP, new GammaDistribution(MeanAtWIP(WIP), VarianceAtWIP(WIP)));
            }
        }

        Dictionary<int, Distribution> Distributions { get; }

        public WorkCenter WorkCenter { get; set; }

        private WIPDepDistParameters par { get; }
        

        public override double Next()
        {
            int WIP = WorkCenter.TotalQueueLength;


            if (WIP <= 0)
            {
                throw new Exception("WIP should always be bigger than 0");
            }
            else if (WIP > par.UBWIP)
            {
                return Distributions[par.UBWIP].Next();
            }
            else
            {
                return Distributions[WIP].Next();
            }
        }

        private double MeanAtWIP(int WIP)
        {
            return exponentialFunction(WIP, par.Twipmin, par.Twipmax, par.Tdecay);
        }

        private double VarianceAtWIP(int WIP)
        {
            double Cv = exponentialFunction(WIP, par.Cwipmin, par.Cwipmax, par.Cdecay);
            double mean = MeanAtWIP(WIP);

            return Cv * Cv * mean * mean;
        }
        
        /// <summary>
        /// Returns the value of mean or coefficient of variation at wip level using the fitted exponential function.
        /// </summary>
        /// <param name="w">WIP</param>
        /// <param name="eta">Value at WIP = 1</param>
        /// <param name="theta">Value at WIP max</param>
        /// <param name="lambda">Decay constant</param>
        /// <returns></returns>
        private double exponentialFunction(int w, double eta, double theta, double lambda)
        {
            return theta + (eta - theta) * Math.Exp(-lambda * (w - 1));
        }

        [Serializable]
        public class WIPDepDistParameters
        {
            public string WorkCenter { get; set; }
            public int LBWIP { get; set; }
            public int UBWIP { get; set; }
            public double Twipmin { get; set; }
            public double Twipmax { get; set; }
            public double Tdecay { get; set; }
            public double Cwipmin { get; set; }
            public double Cwipmax { get; set; }
            public double Cdecay { get; set; }

        }
    }
}
