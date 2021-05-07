using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    /// <summary>
    /// this implementation is based on Marsaglia and Tsang's method, which is also used 
    /// in GSL(Gnu Scientific Library) and Matlab gammd command.
    /// </summary>
    public class GammaDistribution : Distribution
    {
        public GammaDistribution(double mean, double variance) : base(mean, variance)
        {
            Alpha = mean * mean / variance;

            Beta = mean / variance;

            uniform = new UniformDistribution(0, 1);

            normal = new NormalDistribution(0, 1);
        }

        /// <summary>
        /// Shape
        /// </summary>
        public double Alpha { get; private set; }

        /// <summary>
        /// Rate
        /// </summary>
        public double Beta { get; }

        public double CoeffiecientOfVariation => Math.Sqrt(Variance) / Mean;

        private UniformDistribution uniform { get; }

        private NormalDistribution normal { get; }

        public override double Next()
        {
            if (Alpha < 1.0)
            {
                return next(Alpha + 1.0, Beta) * Math.Pow(uniform.Next(), 1.0 / Alpha);
            }
            else
            {
                return next(Alpha, Beta);
            }
        }

        private double next(double a, double b)
        {
            double d = a - 1.0 / 3.0;
            double c = 1.0 / Math.Sqrt(9.0 * d);
            double z, v, p;

            do
            {
                z = normal.Next();
                v = Math.Pow(1.0 + c * z, 3.0);
                p = 0.5 * Math.Pow(z, 2.0) + d - d * v + d * Math.Log(v);
            } while ((z < -1.0 / c) || (Math.Log(uniform.Next()) > p));

            return d * v / b;
        }

        //public override double Next()
        //{
        //    int k = (int)Math.Floor(Alpha);
        //    double a = Alpha - k;
        //    double y = 0;
        //    double e = Math.E;
        //    if (a > 0)
        //    {
        //        while (y == 0)
        //        {
        //            double u = rnd.NextDouble();
        //            double p = u * (e + a) / e;
        //            if (p <= 1)
        //            {
        //                double x = Math.Pow(p, 1 / a);
        //                double u1 = rnd.NextDouble();
        //                if (u1 <= Math.Exp(-x))
        //                { // Accept
        //                    y = x;
        //                }
        //            }
        //            else
        //            {
        //                double x = -Math.Log(p / a);
        //                double u1 = rnd.NextDouble();
        //                if (u1 <= Math.Pow(x, a - 1))
        //                { // Accept
        //                    y = x;
        //                }
        //            }
        //        }
        //    }
        //    double product = 1;
        //    for (int i = 0; i < k; i++)
        //    {
        //        product = product * rnd.NextDouble();
        //    }

        //    double NextValue = (y - Math.Log(product)) / Beta;

        //    if (double.IsPositiveInfinity(NextValue))
        //    {
        //        throw new Exception("Infinite value is sampled from Gamma distribution.");
        //    }

        //    return NextValue;
        //}
    }
}
