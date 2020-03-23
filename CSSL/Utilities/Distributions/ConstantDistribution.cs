using System;
using System.Collections.Generic;
using System.Text;

namespace CSSL.Utilities.Distributions
{
    public class ConstantDistribution : Distribution
    {
        public ConstantDistribution(int constant) : base(constant, 0)
        {
            this.constant = constant;
        }
        public ConstantDistribution(double constant) : base(constant, 0)
        {
            this.constant = constant;
        }

        private double constant { get; }

        public override double Next()
        {
            return constant;
        }
    }
}
