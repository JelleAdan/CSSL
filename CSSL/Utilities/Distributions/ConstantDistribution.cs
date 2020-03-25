using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class ConstantDistribution : Distribution
    {
        public ConstantDistribution(int constant) : base(constant, 0)
        {
            Constant = constant;
        }

        public ConstantDistribution(double constant) : base(constant, 0)
        {
            Constant = constant;
        }

        public double Constant { get; }

        public override double Next()
        {
            return Constant;
        }
    }
}
